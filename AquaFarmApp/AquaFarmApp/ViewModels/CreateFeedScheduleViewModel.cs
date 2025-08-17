using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AquaFarmApp.ViewModels
{
    public class CreateFeedScheduleViewModel
    {
        [Required]
        public string FeedType { get; set; }
        [Required]
        public float FeedCost{ get; set; }
        [Required]
        public int QuantityKg { get; set; }
        public DateTime FeedTime { get; set; }
        public string Note { get; set; }
        public int AreaBatchId { get; set; }
        public int UserId { get; set; }
    }
  
}
