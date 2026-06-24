using ArenaGameNet.Models;
using System.Collections.Generic;

namespace ArenaGameNet.Services
{
    public class ServerManager
    {
        private List<ServerInfo> servers = new List<ServerInfo>();

        public void AddServer(ServerInfo server)
        {
            servers.Add(server);
        }

        public List<ServerInfo> GetServers()
        {
            return servers;
        }

        public void ClearServers()
        {
            servers.Clear();
        }
    }
}