using Google.Cloud.Firestore;

namespace NBAGamesNETCoreAPI.Models
{
    [FirestoreData]
    public class FinishedGame
    {
        //For finished games
        [FirestoreProperty]
        private string WinnerTeam { get; set; }
        [FirestoreProperty]
        private int WinByPoints { get; set; }
    }
}