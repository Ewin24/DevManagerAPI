using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("PilaAportes", Schema = "nomina")]
public partial class PilaAporte
{
    [Key]
    public Guid Id { get; set; }

    public Guid AsociadoId { get; set; }

    public Guid OrganizationId { get; set; }

    [Precision(3)]
    public DateTime Periodo { get; set; }

    public PilaTipoAportante TipoAportante { get; set; }

    [Precision(18, 2)]
    public decimal IngresoBase { get; set; }

    [Precision(18, 2)]
    public decimal AporteEPS { get; set; }

    [Precision(18, 2)]
    public decimal AportePension { get; set; }

    [Precision(18, 2)]
    public decimal AporteARL { get; set; }

    [Precision(18, 2)]
    public decimal Total { get; set; }

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
