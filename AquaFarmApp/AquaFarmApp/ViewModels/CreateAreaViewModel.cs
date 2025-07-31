using AquaFarmApp.Models;
using System.ComponentModel.DataAnnotations;

namespace AquaFarmApp.ViewModels
{
    public class AreaCreateModel
    {
        [Required(ErrorMessage = "Area name is required")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters")]
        public string AreaName { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string AreaStatus { get; set; }

        [Required(ErrorMessage = "Size is required")]
        [Range(1, 10000, ErrorMessage = "Size must be between 1 and 10000")]
        public double AreaSize { get; set; }

        [Required(ErrorMessage = "Type of water is required")]
        public string TypeOfWater { get; set; }
    }
}
