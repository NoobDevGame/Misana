using Misana.Core.Communication.Messages;

namespace Misana.Core.Communication
{
    public class WorldInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public WorldInformation(WorldInformationMessage message)
        {
            Id = message.WorldId;
            Name = message.Name;
        }
    }
}