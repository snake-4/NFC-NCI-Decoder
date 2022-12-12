
using System;
using System.Linq;
using System.Text.RegularExpressions;
using NCIDecoder.Structs;

namespace NCIDecoder
{
    class NCIPacketParser
    {
        public static NCIPacket Parse(PacketDirection direction, byte[] data, long timestamp = 0)
        {
            switch (NCIPacket.DecodeMessageType(data[0]))
            {
                case NCIPacketType.Data:
                    return new NCIDataPacket(direction, data, timestamp);
                case NCIPacketType.ControlNotification:
                case NCIPacketType.ControlCommand:
                case NCIPacketType.ControlResponse:
                    return new NCIControlPacket(direction, data, timestamp);
                default:
                    return new NCIUnknownPacket(direction, data, timestamp);
            }
        }
    }

    class NCIControlPacket : NCIPacket
    {
        public byte GroupID { get; protected set; }
        public byte OpcodeID { get; protected set; }
        public byte PayloadLength { get; protected set; }
        public byte[] Payload { get; protected set; }

        public string GetControlOpcodeTypeString()
        {
            string retVal = "";
            switch (Utils.NCIEnumMemberOrRFU<GroupTypes>(GroupID))
            {
                case GroupTypes.NCI_Core:
                    retVal += Utils.NCIEnumMemberOrRFU<NCI_CoreOpcodes>(OpcodeID).ToString();
                    break;
                case GroupTypes.RF_Management:
                    retVal += Utils.NCIEnumMemberOrRFU<RF_ManagementOpcodes>(OpcodeID).ToString();
                    break;
                case GroupTypes.NFCEE_Management:
                    retVal += Utils.NCIEnumMemberOrRFU<NFCEE_ManagementOpcodes>(OpcodeID).ToString();
                    break;
                case GroupTypes.Proprietary:
                    retVal += "GID_Proprietary";
                    break;
                case GroupTypes.RFU:
                    retVal += "GID_RFU";
                    break;
            }
            switch (MessageTypeDecoded)
            {
                case NCIPacketType.ControlCommand:
                    retVal += "_CMD";
                    break;
                case NCIPacketType.ControlResponse:
                    retVal += "_RSP";
                    break;
                case NCIPacketType.ControlNotification:
                    retVal += "_NTF";
                    break;
            }

            return retVal;
        }

        public NCIControlPacket(PacketDirection direction, byte[] data, long timestamp = 0)
           : base(direction, data, timestamp)
        {
            this.GroupID = (byte)(RawData[0] & 0b0000_1111);
            this.OpcodeID = (byte)(RawData[1] & 0b0011_1111);
            this.PayloadLength = RawData[2];
            this.Payload = RawData.Skip(3).Take(this.PayloadLength).ToArray();
        }
    }

    class NCIDataPacket : NCIPacket
    {
        public byte ConnID { get; protected set; }
        public byte PayloadLength { get; protected set; }
        public byte[] Payload { get; protected set; }

        public NCIDataPacket(PacketDirection direction, byte[] data, long timestamp = 0)
           : base(direction, data, timestamp)
        {
            this.ConnID = (byte)(RawData[0] & 0b0000_1111);
            this.PayloadLength = RawData[2];
            this.Payload = RawData.Skip(3).Take(this.PayloadLength).ToArray();
        }
    }

    class NCIUnknownPacket : NCIPacket
    {
        public NCIUnknownPacket(PacketDirection direction, byte[] data, long timestamp = 0)
            : base(direction, data, timestamp)
        {
            //Hmm
        }
    }

    abstract class NCIPacket
    {
        protected NCIPacket(PacketDirection direction, byte[] data, long timestamp)
        {
            this.PacketDirection = direction;
            this.RawData = data;
            this.Timestamp = timestamp;
            this.PacketBoundaryFlag = (byte)((RawData[0] & 0b0001_0000) >> 4);
            this.MessageTypeDecoded = DecodeMessageType(RawData[0]);
        }

        public static NCIPacketType DecodeMessageType(byte firstByte)
        {
            byte mt = (byte)((firstByte & 0b1110_0000) >> 5);
            return Utils.NCIEnumMemberOrRFU<NCIPacketType>(mt);
        }

        public byte PacketBoundaryFlag { get; protected set; }
        public NCIPacketType MessageTypeDecoded { get; protected set; }
        public PacketDirection PacketDirection { get; protected set; }
        public long Timestamp { get; protected set; }
        public byte[] RawData { get; protected set; }
    }
}
