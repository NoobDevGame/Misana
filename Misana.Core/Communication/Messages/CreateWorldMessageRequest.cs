using System.Runtime.InteropServices;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    [MessageDefinition( ResponseType = typeof(CreateWorldMessageResponse))]
    public struct CreateWorldMessageRequest
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;

        public CreateWorldMessageRequest(string name)
        {
            Name = name;
        }
    }

    [MessageDefinition(IsResponse = true)]
    public struct CreateWorldMessageResponse
    {
        public bool Result;
        public int Id;

        public CreateWorldMessageResponse(bool result, int id)
        {
            Result = result;
            Id = id;
        }
    }
}