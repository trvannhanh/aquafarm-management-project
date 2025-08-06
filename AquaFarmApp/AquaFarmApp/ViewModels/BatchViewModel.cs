namespace AquaFarmApp.ViewModels
{
    public class BatchViewModel
    {
        public int BatchId { get; set; }
        public string Source { get; set; }
        public string AquaticBreed { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EstimatedHarvestDate { get; set; }
        public string BatchStatus { get; set; }
        public int AssignedAreaCount { get; set; } 
    }
}
