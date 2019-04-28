using System;

namespace NBAGamesNETCoreAPI.Models.RootModels
{
    public class Game
    {
        public int SeasonStageId { get; set; }
        public string GameId { get; set; }
        public bool IsGameActivated { get; set; }
        public int StatusNum { get; set; }
        public string StartTimeEastern { get; set; }
        public DateTime StartTimeUTC { get; set; }
        public string StartDateEastern { get; set; }
        public string Clock { get; set; }
        public Period Period { get; set; }
        public VTeam VTeam { get; set; }
        public HTeam HTeam { get; set; }
    }
}
