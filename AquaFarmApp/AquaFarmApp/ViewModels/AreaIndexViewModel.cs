using AquaFarmApp.Models;
using System.ComponentModel.DataAnnotations;

namespace AquaFarmApp.ViewModels
{
    public class AreaIndexViewModel
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string AreaStatus { get; set; }
        public string TypeOfWater { get; set; }
        public int? EnvCheck { get; set; } 
        public bool? EnvWarn { get; set; }
    }
}
