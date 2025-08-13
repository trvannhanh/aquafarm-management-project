using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AquaFarmApp.ViewModels
{
    public class CreateHealthCheckViewModel
    {
        [Required(ErrorMessage = "Required")]
        public string DiseaseSigns { get; set; }

        public string HealthStatus { get; set; }

        public string Notes { get; set; }

        public int AreaBatchId { get; set; }
    }
}
