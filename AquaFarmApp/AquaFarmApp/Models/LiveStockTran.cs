using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

public partial class LiveStockTran
{
    [Key]
    [Column("transaction_id")]
    public int TransactionId { get; set; }

    [Column("trans_type")]
    [StringLength(10)]
    [Unicode(false)]
    public string TransType { get; set; } = null!;

    [Column("reason")]
    [StringLength(255)]
    public string Reason { get; set; } = null!;

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("trans_date", TypeName = "datetime")]
    public DateTime TransDate { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string? Note { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("areabatch_id")]
    public int AreabatchId { get; set; }

    [ForeignKey("AreabatchId")]
    [InverseProperty("LiveStockTrans")]
    public virtual AreaBatch? Areabatch { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("LiveStockTrans")]
    public virtual User? User { get; set; }

}
