using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("Area_Batch")]
public partial class AreaBatch
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("area_id")]
    public int AreaId { get; set; }

    [Column("batch_id")]
    public int BatchId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [ForeignKey("AreaId")]
    [InverseProperty("AreaBatches")]
    public virtual Area? Area { get; set; }

    [ForeignKey("BatchId")]
    [InverseProperty("AreaBatches")]
    public virtual Batch? Batch { get; set; }

    [InverseProperty("Areabatch")]
    public virtual ICollection<FeedSchedule> FeedSchedules { get; set; } = new List<FeedSchedule>();

    [InverseProperty("Areabatch")]
    public virtual ICollection<HarvestSale> HarvestSales { get; set; } = new List<HarvestSale>();

    [InverseProperty("Areabatch")]
    public virtual ICollection<LiveStockTran> LiveStockTrans { get; set; } = new List<LiveStockTran>();

    [InverseProperty("Areabatch")]
    public virtual ICollection<HealthCheck> HealthChecks { get; set; } = new List<HealthCheck>();
}
