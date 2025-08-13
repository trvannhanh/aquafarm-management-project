using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace AquaFarmApp.ViewModels
{
    public class CreateEnvLogViewModel 
    {
        [Required(ErrorMessage = "Required")]
        [Range(-20,50)]
        public double Temperature { get; set; }
        [Required(ErrorMessage = "Must be between 0 and 14")]
        [Range(0, 14)]
        public double PhLevel { get; set; }
        [Required(ErrorMessage = "Must be between 0 and 100")]
        [Range(0, 100)]
        public double OxygenLevel { get; set; }
        [Required(ErrorMessage = "Must be between 0 and 14")]
        [Range(0, 100)]
        public double Salinity { get; set; }

        public DateTime RecordedAt { get; set; }

        public string Note { get; set; }
        
        public int AreaId { get; set; }
    }
}
