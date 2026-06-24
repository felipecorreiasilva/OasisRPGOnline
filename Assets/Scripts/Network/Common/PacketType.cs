namespace Oasis.Network.Common
{
    public enum PacketType : ushort
    {
        COMMON_PING = 0x0001,

        // Login (0x01xx)
        LOGIN_REQUEST = 0x0100,
        LOGIN_ACCEPTED = 0x0101,
        LOGIN_DENIED = 0x0102,

        // Character (0x02xx)
        CHAR_LIST_REQUEST = 0x0200,
        CHAR_LIST = 0x0201,
        CHAR_SELECT = 0x0202,
        CHAR_CREATE = 0x0203,
        CHAR_CREATE_SUCCESS = 0x0204,
        CHAR_CREATE_FAILED = 0x0205,
        ZONE_SERVER_INFO = 0x0206,

        // Map (0x03xx)
        MAP_ENTER_REQUEST = 0x0300,
        MAP_ENTER_ACCEPTED = 0x0301
    }
}   