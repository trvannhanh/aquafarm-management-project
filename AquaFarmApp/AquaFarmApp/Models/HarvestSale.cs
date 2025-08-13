using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("HarvestSale")]
public partial class HarvestSale
{
    [Key]
    [Column("sale_id")]
    public int SaleId { get; set; }

    [Column("harvest_date", TypeName = "datetime")]
    public DateTime? HarvestDate { get; set; }

    [Column("buyer_name")]
    [StringLength(50)]
    public string BuyerName { get; set; } = null!;

    [Column("price_per_kg")]
    public double PricePerKg { get; set; }

    [Column("quantity_kg")]
    public double QuantityKg { get; set; }

    [Column("revenue")]
    public double Revenue { get; set; }

    [Column("estimated_cost")]
    public double EstimatedCost { get; set; }

    [Column("profit")]
    public double Profit { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string? Note { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("areabatch_id")]
    public int AreabatchId { get; set; }

    [ForeignKey("AreabatchId")]
    [InverseProperty("HarvestSales")]
    public virtual AreaBatch? Areabatch { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("HarvestSales")]
    public virtual User? User { get; set; }
}
