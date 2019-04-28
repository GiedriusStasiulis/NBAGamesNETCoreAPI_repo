using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAGamesNETCoreAPI.Models
{
    public class GuessFromAndroid
    {
        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }
        [Column(Order = 1)]
        public string UserId { get; set; }
        [Column(Order = 2)]
        public string GameId { get; set; }
        [Column(Order = 3)]
        public string SelTeam { get; set; }
        [Column(Order = 4)]
        public int ByPts { get; set; }
    }
}