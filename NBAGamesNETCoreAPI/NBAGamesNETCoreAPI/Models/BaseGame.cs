using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace NBAGamesNETCoreAPI.Models
{
    [FirestoreData]
    public class BaseGame
    {
        //General dame details
        [FirestoreProperty]
        [Key]
        private string GameID { get; set; }
        [FirestoreProperty]
        private string GameDate { get; set; }        
        [FirestoreProperty]
        private string TeamATriCode { get; set; }
        [FirestoreProperty]
        private string TeamAFullName { get; set; }
        [FirestoreProperty]
        private string TeamALogoSrc { get; set; }
        [FirestoreProperty]
        private string TeamBTriCode { get; set; }
        [FirestoreProperty]
        private string TeamBFullName { get; set; }
        [FirestoreProperty]
        private string TeamBLogoSrc { get; set; }   
    }
}
