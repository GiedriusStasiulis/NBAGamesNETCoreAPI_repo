using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAGamesNETCoreAPI.Models
{
    [FirestoreData]
    public class UpcomingGame
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set;}
        [FirestoreProperty]
        [Column(Order = 1)]
        public string GameId { get; set; }
        [FirestoreProperty]
        [Column(Order = 2)]
        public DateTime GameStartDateTimeUTC { get; set; }
        [FirestoreProperty]
        [Column(Order = 3)]
        public string TeamATriCode { get; set; }
        [FirestoreProperty]
        [Column(Order = 4)]
        public string TeamAFullName { get; set; }
        [FirestoreProperty]
        [Column(Order = 5)]
        public string TeamALogoSrc { get; set; }
        [FirestoreProperty]
        [Column(Order = 6)]
        public string TeamBTriCode { get; set; }
        [FirestoreProperty]
        [Column(Order = 7)]
        public string TeamBFullName { get; set; }
        [FirestoreProperty]
        [Column(Order = 8)]
        public string TeamBLogoSrc { get; set; }
        [FirestoreProperty]
        [Column(Order = 9)]
        public string LastUpdated { get; set; }
    }
}