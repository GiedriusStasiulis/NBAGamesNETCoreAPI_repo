using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAGamesNETCoreAPI.Models.RootModels
{
    [FirestoreData]
    public class GameToFirestore
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        [FirestoreProperty]
        [Column(Order = 1)]
        public string GameId { get; set; }
        [Column(Order = 2)]
        public DateTime GameStartDateTimeUTC { get; set; }
        [FirestoreProperty]
        [Column(Order = 3)]
        public string GameDateUTC { get; set; }
        [FirestoreProperty]
        [Column(Order = 4)]
        public string GameStartTimeUTC { get; set; }
        [FirestoreProperty]
        [Column(Order = 5)]
        public string TeamATriCode { get; set; }
        [FirestoreProperty]
        [Column(Order = 6)]
        public string TeamAFullName { get; set; }
        [FirestoreProperty]
        [Column(Order = 7)]
        public string TeamALogoSrc { get; set; }
        [FirestoreProperty]
        [Column(Order = 8)]
        public string TeamBTriCode { get; set; }
        [FirestoreProperty]
        [Column(Order = 9)]
        public string TeamBFullName { get; set; }
        [FirestoreProperty]
        [Column(Order = 10)]
        public string TeamBLogoSrc { get; set; }
        [FirestoreProperty]
        [Column(Order = 11)]
        public string LastUpdated { get; set; }
        [FirestoreProperty]
        [Column(Order = 12)]
        public int OrderNo { get; set; }
        [FirestoreProperty]
        [Column(Order = 13)]
        public int StatusNum { get; set; }
        [FirestoreProperty]
        [Column(Order = 14)]
        public string TeamAScore { get; set; }
        [FirestoreProperty]
        [Column(Order = 15)]
        public string TeamBScore { get; set; }
    }
}
