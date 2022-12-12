namespace NCIDecoder.Structs
{
    public enum PacketDirection
    {
        CPUToNFCC,
        NFCCToCPU
    };

    public enum NCIPacketType
    {
        Data = 0b0000_0000,
        ControlCommand = 0b0000_0001,
        ControlResponse = 0b0000_0010,
        ControlNotification = 0b0000_0011,
        RFU
    };

    public enum GroupTypes
    {
        NCI_Core = 0b0000_0000,
        RF_Management = 0b0000_0001,
        NFCEE_Management = 0b0000_0010,
        Proprietary = 0b0000_1111,
        RFU
    };

    public enum NCI_CoreOpcodes
    {
        CORE_RESET = 0b0000_0000,
        CORE_INIT = 0b0000_0001,
        CORE_SET_CONFIG = 0b0000_0010,
        CORE_GET_CONFIG = 0b0000_0011,
        CORE_CONN_CREATE = 0b0000_0100,
        CORE_CONN_CLOSE = 0b0000_0101,
        CORE_CONN_CREDITS = 0b0000_0110,
        CORE_GENERIC_ERROR = 0b0000_0111,
        CORE_INTERFACE_ERROR = 0b0000_1000
    };

    public enum RF_ManagementOpcodes
    {
        RF_DISCOVER_MAP = 0b0000_0000,
        RF_SET_LISTEN_MODE_ROUTING = 0b0000_0001,
        RF_GET_LISTEN_MODE_ROUTING = 0b0000_0010,
        RF_DISCOVER = 0b0000_0011,
        RF_DISCOVER_SELECT = 0b0000_0100,
        RF_INTF_ACTIVATED = 0b0000_0101,
        RF_DEACTIVATE = 0b0000_0110,
        RF_FIELD_INFO = 0b0000_0111,
        RF_T3T_POLLING = 0b0000_1000,
        RF_NFCEE_ACTION = 0b0000_1001,
        RF_NFCEE_DISCOVERY_REQ = 0b0000_1010,
        RF_PARAMETER_UPDATE = 0b0000_1011
    };

    public enum NFCEE_ManagementOpcodes
    {
        NFCEE_DISCOVER = 0b0000_0000,
        NFCEE_MODE_SET = 0b0000_0001
    };
}
