namespace Application.Interfaces;

using Application.DTOs.Organos;

/// <summary>
/// Servicio de gestión de órganos de administración, asambleas y votación
/// Ley 79 art.26-45
/// </summary>
public interface IOrganoService
{
    // ========= Órganos =========
    Task<OrganoDto> CreateOrganoAsync(CreateOrganoDto dto);
    Task<OrganoDto?> GetOrganoByIdAsync(Guid id);
    Task<List<OrganoDto>> GetOrganosByOrganizationAsync(Guid organizationId);
    Task<List<OrganoDto>> GetOrganosByTypeAsync(Guid organizationId, Domain.Enums.TipoOrgano tipo);
    Task<OrganoDto> UpdateOrganoAsync(Guid id, UpdateOrganoDto dto);
    Task<bool> DeleteOrganoAsync(Guid id);

    // ========= Miembros =========
    Task<MiembroOrganoDto> AsignarMiembroAsync(AsignarMiembroDto dto);
    Task<List<MiembroOrganoDto>> GetMiembrosByOrganoAsync(Guid organoId);
    Task<MiembroOrganoDto> UpdateMiembroAsync(Guid id, UpdateMiembroDto dto);
    Task<bool> RemoveMiembroAsync(Guid id);

    // ========= Actas =========
    Task<ActaDto> CreateActaAsync(CreateActaDto dto);
    Task<ActaDto?> GetActaByIdAsync(Guid id);
    Task<List<ActaDto>> GetActasByOrganoAsync(Guid organoId);

    // ========= Asambleas =========
    Task<AsambleaDto> ConvocarAsambleaAsync(ConvocarAsambleaDto dto);
    Task<AsambleaDto?> GetAsambleaByIdAsync(Guid id);
    Task<List<AsambleaDto>> GetAsambleasByOrganizationAsync(Guid organizationId);
    Task<AsambleaDto> RegistrarAsistenciaAsync(Guid id, RegistrarAsistenciaDto dto);
    Task<AsambleaDto> CerrarAsambleaAsync(Guid id, CerrarAsambleaDto dto);

    // ========= Voto =========
    Task<VotoDto> EmitirVotoAsync(EmitirVotoDto dto);
    Task<ResultadoVotacionDto> GetResultadosAsync(Guid asambleaId);
    Task<bool> HaVotadoAsync(Guid asambleaId, Guid asociadoId);
}
