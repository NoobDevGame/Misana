using System;
using System.Runtime.InteropServices;

namespace Misana.Network.Messages
{
    internal struct GetMessageIDMessageRequest
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public readonly string TypeName;

        public GetMessageIDMessageRequest(string typeName)
        {
            TypeName = typeName;
        }

        public GetMessageIDMessageRequest(Type type)
        {
            TypeName = type.AssemblyQualifiedName;
        }
    }

    internal struct GetMessageIDMessageResponse
    {
        public readonly int TypeId;
        public readonly bool Result;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public readonly string TypeName;

        public GetMessageIDMessageResponse(bool result ,int id,string typeName)
        {
            Result = result;
            TypeId = id;
            TypeName = typeName;
        }
    }
}