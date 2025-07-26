using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("Batch")]
public partial class Batch
{
    [Key]
    [Column("batch_id")]
    public int BatchId { get; set; }

    [Column("batch_status")]
    [StringLength(10)]
    [Unicode(false)]
    public string BatchStatus { get; set; } = null!;

    [Column("source")]
    [StringLength(50)]
    public string Source { get; set; } = null!;

    [Column("aquatic_breed")]
    [StringLength(100)]
    public string AquaticBreed { get; set; } = null!;

    [Column("start_date", TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column("estimated_harvest_date", TypeName = "datetime")]
    public DateTime EstimatedHarvestDate { get; set; }

    [InverseProperty("Batch")]
    public virtual ICollection<AreaBatch> AreaBatches { get; set; } = new List<AreaBatch>();
}
