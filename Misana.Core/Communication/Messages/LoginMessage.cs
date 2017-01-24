using System.Runtime.InteropServices;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    [MessageDefinition(ResponseType = typeof(LoginResponseMessage))]
    public struct LoginRequestMessage
    {

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;

        public LoginRequestMessage(string name)
        {
            Name = name;
        }
    }

    [MessageDefinition(IsResponse = true)]
    public struct LoginResponseMessage
    {
        public int Id;

        public LoginResponseMessage(int id)
        {
            Id = id;
        }
    }
}