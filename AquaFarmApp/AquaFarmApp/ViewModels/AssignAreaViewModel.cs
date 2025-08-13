using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AquaFarmApp.ViewModels
{
    public class AssignAreaViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Batch ID is required")]
        public int BatchId { get; set; }

        public List<AreaBatchInput> AreaBatches { get; set; } = new List<AreaBatchInput>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AreaBatches == null)
            {
                yield return new ValidationResult(
                    "AreaBatches cannot be null.",
                    new[] { nameof(AreaBatches) });
                yield break;
            }

            if (!AreaBatches.Any(ab => ab.Quantity > 0))
            {
                yield return new ValidationResult(
                    "At least one area must be assigned with a valid quantity greater than 0.",
                    new[] { nameof(AreaBatches) });
            }
        }
    }

    public class AreaBatchInput
    {
        [Required(ErrorMessage = "Area ID is required")]
        public int AreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}