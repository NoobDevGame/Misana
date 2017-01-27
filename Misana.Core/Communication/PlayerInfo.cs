namespace Misana.Core.Communication
{
    public class PlayerInfo
    {
        public PlayerInfo(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public int Id { get; set; }
    }
}