using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.MicrosoftExtensions;

namespace AquaFarmApp.ViewModels
{
    public class CreateSaleViewModel 
    {
        public DateTime harvestdate { get; set; }
        [Required(ErrorMessage = "Buyer name is required.")]
        public string buyername { get; set; }
        [Required(ErrorMessage = "Must enter price.")]
        [Range(0.1,100000000)]
        public float priceperkg { get; set; }
        [Required(ErrorMessage = "Must enter quantity.")]
        [Range(0.1, 100000000)]
        public int quantitykg { get; set; }
        public float revenue { get; set; }
        public float estimatedcost { get; set; }
        public float profit { get; set; }
        public string note { get; set; }
        public int AreaBatchId { get; set; }
        public int UserId { get; set; }

    }
}