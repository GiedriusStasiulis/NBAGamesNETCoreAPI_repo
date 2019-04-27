using System.Collections.Generic;

namespace NBAGamesNETCoreAPI.Models.RootModels
{
    public class RootObject
    {
        public int NumGames { get; set; }
        public List<Game> Games { get; set; }
    }
}
