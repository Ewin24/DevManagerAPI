using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("Compensaciones", Schema = "nomina")]
public partial class Compensacion
{
    [Key]
    public Guid Id { get; set; }

    public Guid AsociadoId { get; set; }

    public Guid OrganizationId { get; set; }

    [Precision(3)]
    public DateTime Periodo { get; set; }

    public CompensacionModelo Modelo { get; set; }

    [Precision(18, 2)]
    public decimal ValorBase { get; set; }

    [Precision(18, 2)]
    public decimal ValorCalculado { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [Precision(3)]
    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    [Precision(3)]
    public DateTime? DeletedAt { get; set; }

    public Guid? DeletedByUserId { get; set; }
}
