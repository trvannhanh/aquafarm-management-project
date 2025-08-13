namespace AquaFarmApp.ViewModels
{
    public class LiveStockTranViewModel
    {
        public int TransactionId { get; set; }
        public int AreaBatchId { get; set; }
        public int UserId { get; set; }
        public string? AreaName { get; set; }
        public string? BatchName { get; set; }
        public DateTime TransDate { get; set; }
        public int Quantity { get; set; }
        public string TransType { get; set; }
        public string Reason { get; set; }
        public string? Note { get; set; }
    }
}
