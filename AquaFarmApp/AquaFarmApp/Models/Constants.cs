namespace AquaFarmApp.Models
{
    public enum BatchStatus
    {
        Avail,
        NotAvail,
        InProgress,
        Harvested
    }

    public enum AreaStatus
    {
        Avail,
        NotAvail,
        HealthSecured
    }

    public enum WaterType
    {
        Freshwater,
        Brackish,
        Saltwater,
        Recirculated,
        Treated
    }

    public enum TransType
    {
        Import,
        Export
    }
}