using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AquaFarmApp.ViewModels
{


    public class CreateBatchViewModel
    {
        public int BatchId { get; set; }

        [Required(ErrorMessage = "Source is required")]
        [StringLength(50, ErrorMessage = "Source cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Source only allows letters, numbers, and spaces")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Aquatic Breed is required")]
        [StringLength(50, ErrorMessage = "Aquatic Breed cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Aquatic Breed only allows letters, numbers, and spaces")]
        public string AquaticBreed { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Estimated Harvest Date is required")]
        [DataType(DataType.Date)]
        public DateTime EstimatedHarvestDate { get; set; }

        [Required(ErrorMessage = "Total Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total Quantity must be greater than 0")]
        public int TotalQuantity { get; set; }

        public List<AreaBatchInput> AreaBatches { get; set; } = new List<AreaBatchInput>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EstimatedHarvestDate <= StartDate)
            {
                yield return new ValidationResult(
                    "Estimated Harvest Date must be after Start Date.",
                    new[] { nameof(EstimatedHarvestDate) });
            }
        }
    }

    
}