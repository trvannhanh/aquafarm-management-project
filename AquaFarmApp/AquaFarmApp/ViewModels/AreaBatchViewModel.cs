namespace AquaFarmApp.ViewModels
{
    public class AreaBatchViewModel
    {
        public int AreaBatchId { get; set; } // ID của bản ghi AreaBatch (nếu có)
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public int Quantity { get; set; }
        public string AquaticBreed { get; set; }
        public string BatchStatus { get; set; }
    }
}
