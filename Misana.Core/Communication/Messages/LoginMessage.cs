using System.Runtime.InteropServices;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    [MessageDefinition(ResponseType = typeof(LoginMessageResponse))]
    public struct LoginMessageRequest
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;

        public LoginMessageRequest(string name)
        {
            Name = name;
        }
    }

    [MessageDefinition(IsResponse = true)]
    public struct LoginMessageResponse
    {
        public int Id;

        public LoginMessageResponse(int id)
        {
            Id = id;
        }
    }
}