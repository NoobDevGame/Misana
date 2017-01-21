using System;
using System.Runtime.InteropServices;

namespace Misana.Network.Messages
{
    internal struct GetMessageIdMessageRequest
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public readonly string TypeName;

        public GetMessageIdMessageRequest(string typeName)
        {
            TypeName = typeName;
        }

        public GetMessageIdMessageRequest(Type type)
        {
            TypeName = type.AssemblyQualifiedName;
        }
    }

    internal struct GetMessageIdMessageResponse
    {
        public readonly int TypeId;
        public readonly bool Result;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public readonly string TypeName;

        public GetMessageIdMessageResponse(bool result ,int id,string typeName)
        {
            Result = result;
            TypeId = id;
            TypeName = typeName;
        }
    }
}