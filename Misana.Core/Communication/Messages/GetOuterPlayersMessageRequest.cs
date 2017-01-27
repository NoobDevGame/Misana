using System.Runtime.InteropServices;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition]
    public struct GetOuterPlayersMessageRequest
    {

    }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    [MessageDefinition]
    public struct PlayerInfoMessage
    {
        public int PlayerId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;


        public PlayerInfoMessage(int playerId, string name)
        {
            PlayerId = playerId;
            Name = name;
        }
    }
}