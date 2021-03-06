﻿using System.Runtime.InteropServices;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition]
    public struct ReadWorldsMessageRequest
    {

    }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    [MessageDefinition]
    public struct WorldInformationMessage
    {
        public int WorldId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;

        public WorldInformationMessage(int worldId, string name)
        {
            WorldId = worldId;
            Name = name;
        }
    }
}