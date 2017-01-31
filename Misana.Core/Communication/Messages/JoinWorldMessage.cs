using System.Runtime.InteropServices;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition(ResponseType = typeof(JoinWorldMessageResponse))]
    public struct JoinWorldMessageRequest
    {
        public int Id;

        public JoinWorldMessageRequest(int id)
        {
            Id = id;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    [MessageDefinition(IsResponse = true)]
    public struct JoinWorldMessageResponse
    {
        public bool Result;
        public bool HaveWorld;

        public int FirstLocalEntityId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string MapName;

        public JoinWorldMessageResponse(bool result, bool haveWorld, string mapName, int firstLocalEntityId)
        {
            Result = result;
            HaveWorld = haveWorld;
            MapName = mapName;
            FirstLocalEntityId = firstLocalEntityId;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    [MessageDefinition]
    public struct OnJoinWorldMessage
    {
        public int PlayerId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;

        public OnJoinWorldMessage(int playerId, string name)
        {
            PlayerId = playerId;
            Name = name;
        }
    }
}