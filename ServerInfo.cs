namespace ArenaGameNet.Models
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string Map { get; set; }
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
        public bool HasPassword { get; set; }
        public bool IsLan { get; set; }
    }
}