using NCIDecoder.Structs;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCIDecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: ncidecoder.exe <inputFilePath>");
                Console.WriteLine("The input file must be the output of https://github.com/VivoKey/NFCSnoopDecoder");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(args[0]);
                if (lines.Length < 4)
                {
                    throw new Exception("Input file must have more than 3 lines!");
                }

                string NFCSnoopVersion = lines[0].Substring(20);
                Console.WriteLine("NFCSnoop Version: " + NFCSnoopVersion);

                var nciPackets = new List<NCIPacket>();
                long currentTimestamp = 0;
                for (int i = 3; i < lines.Length; i++)
                {
                    var dataLineItemized = lines[i].Split(new char[] { ',' });
                    if (dataLineItemized.Length != 3)
                    {
                        throw new Exception("Incorrect data line at ");
                    }

                    bool isReceived = dataLineItemized[1] == " true";
                    var dir = isReceived ? PacketDirection.NFCCToCPU : PacketDirection.CPUToNFCC;
                    currentTimestamp += long.Parse(dataLineItemized[0]);
                    byte[] data = Convert.FromHexString(dataLineItemized[2].Trim());

                    nciPackets.Add(NCIPacketParser.Parse(dir, data, currentTimestamp));
                }

                //TODO: perhaps output json instead?
                //TODO: support segmented packets, control ones have a max len until the last segmented packet
                foreach (var item in nciPackets)
                {
                    Console.Write($"{item.Timestamp}|{(item.PacketDirection == PacketDirection.NFCCToCPU ? "R" : "S")}|[{item.MessageTypeDecoded}] ");

                    if (item is NCIDataPacket)
                    {
                        var casted = (NCIDataPacket)item;
                        Console.WriteLine($"PayloadLength: {casted.PayloadLength} Payload: {Convert.ToHexString(casted.Payload)}");
                    }
                    else if (item is NCIControlPacket)
                    {
                        var casted = (NCIControlPacket)item;
                        Console.WriteLine($"OpcodeStr: {casted.GetControlOpcodeTypeString()} " +
                            $"Payload({casted.PayloadLength}): {Convert.ToHexString(casted.Payload)}");
                    }
                    else if (item is NCIUnknownPacket)
                    {
                        Console.WriteLine($"RawDataLength: {item.RawData.Length} Payload: {Convert.ToHexString(item.RawData)}");
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error while trying to parse the file! Exception: " + exc);
                return;
            }
        }
    }
}
