using Google.Cloud.Firestore;

namespace NBAGamesNETCoreAPI.Models
{
    [FirestoreData]
    public class DummyData
    {
        [FirestoreProperty]
        public int ID { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public int Age { get; set; }

        [FirestoreProperty]
        public string Updated { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}