using AquaFarmApp.Models;
using System.ComponentModel.DataAnnotations;

namespace AquaFarmApp.ViewModels
{
    public class CreateFarmViewModel
    {
        [Required(ErrorMessage = "Farm name is required")]
        public string FarmName { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string FarmLocation { get; set; }

        [Required(ErrorMessage = "Please specify number of areas")]
        [Range(1, 20)]
        public int AreaTotal { get; set; }

        public List<Area> Areas { get; set; } = new();
    }
}
