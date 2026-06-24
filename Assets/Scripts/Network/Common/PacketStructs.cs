using System.Runtime.InteropServices;

namespace Oasis.Network.Common
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_LOGIN_REQUEST
    {
        public ushort packet_id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)] public string username;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)] public string password;
        public uint client_version;
        public byte client_type;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_LOGIN_ACCEPTED
    {
        public ushort packet_id;
        public uint login_id1;
        public uint login_id2;
        public uint user_id;
        public byte sex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_LOGIN_DENIED
    {
        public ushort packet_id;
        public byte error_code;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_CHAR_LIST_REQUEST
    {
        public ushort packet_id;
        public uint user_id;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_CHAR_LIST
    {
        public ushort packet_id;
        public byte count;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_CHAR_LIST_ENTRY
    {
        public uint char_id;
        public byte char_num;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)] public string name;
        public byte level;
        public byte sex;
        public byte hair;
        public ushort map_id;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_CHAR_SELECT
    {
        public ushort packet_id;
        public byte slot;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_CHAR_CREATE
    {
        public ushort packet_id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)] public string name;
        public byte slot;
        public ushort hair_color;
        public ushort hair_style;
        public uint job;
        public byte sex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_CHAR_CREATE_SUCCESS
    {
        public ushort packet_id;
        public byte result;
        public uint char_id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)] public string name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_CHAR_CREATE_FAILED
    {
        public ushort packet_id;
        public byte error_code;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_ZONE_SERVER_INFO
    {
        public ushort packet_id;
        public uint char_id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)] public string map_name;
        public uint ip;
        public ushort port;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_MAP_ENTER_REQUEST
    {
        public ushort packet_id;
        public uint char_id;
        public ushort map_id;
        public float x;
        public float y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACKET_MAP_ENTER_ACCEPTED
    {
        public ushort packet_id;
        public byte status;
        public ushort map_id;
        public float x;
        public float y;
    }
}