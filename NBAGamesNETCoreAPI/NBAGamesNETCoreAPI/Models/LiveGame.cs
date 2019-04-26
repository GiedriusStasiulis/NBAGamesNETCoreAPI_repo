using Google.Cloud.Firestore;

namespace NBAGamesNETCoreAPI.Models
{
    [FirestoreData]
    public class LiveGame : BaseGame
    {
        //For live games
        [FirestoreProperty]
        private int Period { get; set; }
        [FirestoreProperty]
        private bool IsHalftime { get; set; }
        [FirestoreProperty]
        private bool IsOvertime { get; set; }
        [FirestoreProperty]
        private bool IsStarted { get; set; }
        [FirestoreProperty]
        private bool IsFinished { get; set; }
        [FirestoreProperty]
        private int TeamAScore { get; set; }
        [FirestoreProperty]
        private int TeamBScore { get; set; }
        [FirestoreProperty]
        private string InGameTime { get; set; }
    }
}
