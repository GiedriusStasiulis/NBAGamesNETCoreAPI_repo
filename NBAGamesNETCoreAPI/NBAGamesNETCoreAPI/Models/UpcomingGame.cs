using Google.Cloud.Firestore;

namespace NBAGamesNETCoreAPI.Models
{
    [FirestoreData]
    public class UpcomingGame : BaseGame
    {
        [FirestoreProperty]
        private string GameStartTime { get; set; }

        protected string GetGameTime()
        {
            string gameTime;

            if (GameStartTime == null) { gameTime = "TBD"; }
            else { gameTime = GameStartTime; }

            return gameTime;
        }
    }
}