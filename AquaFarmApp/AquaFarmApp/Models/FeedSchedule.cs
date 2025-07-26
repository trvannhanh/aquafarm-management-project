using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("FeedSchedule")]
public partial class FeedSchedule
{
    [Key]
    [Column("feed_id")]
    public int FeedId { get; set; }

    [Column("feed_type")]
    [StringLength(50)]
    [Unicode(false)]
    public string FeedType { get; set; } = null!;

    [Column("feed_cost")]
    public double FeedCost { get; set; }

    [Column("feed_quantity")]
    public int FeedQuantity { get; set; }

    [Column("feed_time", TypeName = "datetime")]
    public DateTime? FeedTime { get; set; }

    [Column("feed_status")]
    [StringLength(20)]
    [Unicode(false)]
    public string FeedStatus { get; set; } = null!;

    [Column("note")]
    [StringLength(255)]
    public string? Note { get; set; }

    [Column("areabatch_id")]
    public int? AreabatchId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey("AreabatchId")]
    [InverseProperty("FeedSchedules")]
    public virtual AreaBatch? Areabatch { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("FeedSchedules")]
    public virtual User? User { get; set; }
}
