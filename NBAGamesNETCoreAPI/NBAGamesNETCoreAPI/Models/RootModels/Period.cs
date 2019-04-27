namespace NBAGamesNETCoreAPI.Models.RootModels
{
    public class Period
    {
        public int Current { get; set; }
        public int Type { get; set; }
        public int MaxRegular { get; set; }
        public bool IsHalftime { get; set; }
        public bool IsEndOfPeriod { get; set; }
    }
}
