using System.Runtime.InteropServices;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    [MessageDefinition(ResponseType = typeof(ChangeMapMessageResponse))]
    public struct ChangeMapMessageRequest
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;

        public ChangeMapMessageRequest(string name)
        {
            Name = name;
        }
    }

    [MessageDefinition(IsResponse = true)]
    public struct ChangeMapMessageResponse
    {
        public bool Result;

        public ChangeMapMessageResponse(bool result)
        {
            Result = result;
        }
    }
}