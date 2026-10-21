namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.Solidarity;
using Application.Interfaces;
using Domain.Entities.Solidarity;
using Domain.Enums;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio de educación cooperativa. Todas las operaciones se resuelven dentro de la organización del llamante.
/// </summary>
public class EducationService : IEducationService
{
    private const decimal MaxHours = 9999.9m; // decimal(5,1)

    private readonly IEducationRepository _educationRepository;
    private readonly IAssociateRepository _associateRepository;

    public EducationService(IEducationRepository educationRepository, IAssociateRepository associateRepository)
    {
        _educationRepository = educationRepository;
        _associateRepository = associateRepository;
    }

    public async Task<IEnumerable<EducationProgramDto>> GetProgramsAsync(Guid organizationId)
    {
        var programs = await _educationRepository.GetProgramsAsync(organizationId);
        return programs.Select(MapProgram).ToList();
    }

    public async Task<EducationProgramDto> GetProgramByIdAsync(Guid id, Guid organizationId)
    {
        var program = await _educationRepository.GetProgramByIdAsync(id, organizationId)
            ?? throw new NotFoundException("Programa", id);
        return MapProgram(program);
    }

    public async Task<EducationProgramDto> CreateProgramAsync(SaveEducationProgramRequest request, Guid organizationId, Guid createdByUserId)
    {
        var program = new EducationProgram
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };
        Apply(program, request);

        await _educationRepository.CreateProgramAsync(program);
        return MapProgram(program);
    }

    public async Task<EducationProgramDto> UpdateProgramAsync(Guid id, SaveEducationProgramRequest request, Guid organizationId, Guid updatedByUserId)
    {
        var program = await _educationRepository.GetProgramByIdAsync(id, organizationId)
            ?? throw new NotFoundException("Programa", id);

        Apply(program, request);
        program.UpdatedByUserId = updatedByUserId;

        await _educationRepository.UpdateProgramAsync(program);
        return MapProgram(program);
    }

    public async Task DeleteProgramAsync(Guid id, Guid organizationId, Guid deletedByUserId)
    {
        _ = await _educationRepository.GetProgramByIdAsync(id, organizationId)
            ?? throw new NotFoundException("Programa", id);

        await _educationRepository.DeleteProgramAsync(id, organizationId, deletedByUserId);
    }

    public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsAsync(Guid programId, Guid organizationId)
    {
        _ = await _educationRepository.GetProgramByIdAsync(programId, organizationId)
            ?? throw new NotFoundException("Programa", programId);

        var enrollments = await _educationRepository.GetEnrollmentsByProgramAsync(programId, organizationId);
        return enrollments.Select(MapEnrollment).ToList();
    }

    public async Task<EnrollmentDto> EnrollAsync(Guid programId, CreateEnrollmentRequest request, Guid organizationId, Guid createdByUserId)
    {
        _ = await _educationRepository.GetProgramByIdAsync(programId, organizationId)
            ?? throw new NotFoundException("Programa", programId);

        var associate = await _associateRepository.GetByUserIdAsync(request.UserId, organizationId)
            ?? throw new NotFoundException("Asociado", request.UserId);

        if (associate.Status == AssociateStatus.Withdrawn)
            throw new ConflictException("No se puede inscribir a un asociado retirado");

        if (await _educationRepository.EnrollmentExistsAsync(programId, request.UserId, organizationId))
            throw new ConflictException("El asociado ya está inscrito en este programa");

        var enrollment = new EducationEnrollment
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            ProgramId = programId,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };
        ApplyProgress(enrollment, request.Status ?? EnrollmentStatus.Enrolled, request.HoursCompleted, request.CompletionDate);

        await _educationRepository.CreateEnrollmentAsync(enrollment);
        return MapEnrollment((await _educationRepository.GetEnrollmentByIdAsync(enrollment.Id, organizationId))!);
    }

    public async Task<EnrollmentDto> UpdateEnrollmentAsync(Guid enrollmentId, UpdateEnrollmentRequest request, Guid organizationId, Guid updatedByUserId)
    {
        var enrollment = await _educationRepository.GetEnrollmentByIdAsync(enrollmentId, organizationId)
            ?? throw new NotFoundException("Inscripción", enrollmentId);

        ApplyProgress(enrollment, request.Status, request.HoursCompleted, request.CompletionDate);
        enrollment.UpdatedByUserId = updatedByUserId;

        await _educationRepository.UpdateEnrollmentAsync(enrollment);
        return MapEnrollment((await _educationRepository.GetEnrollmentByIdAsync(enrollmentId, organizationId))!);
    }

    #region Helpers

    private static void Apply(EducationProgram program, SaveEducationProgramRequest request)
    {
        var name = request.Name?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 160)
            throw new BusinessValidationException("El nombre es obligatorio y no puede superar 160 caracteres");
        if (!Enum.IsDefined(request.Type))
            throw new BusinessValidationException("Tipo de programa inválido");
        if (request.RequiredHours <= 0 || request.RequiredHours > MaxHours)
            throw new BusinessValidationException("Las horas requeridas deben ser mayores que 0");

        var start = request.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.EndDate.HasValue && request.EndDate.Value < start)
            throw new BusinessValidationException("La fecha de fin no puede ser anterior a la de inicio");

        program.Name = name;
        program.Type = request.Type;
        program.RequiredHours = request.RequiredHours;
        program.StartDate = start;
        program.EndDate = request.EndDate;
    }

    private static void ApplyProgress(EducationEnrollment enrollment, EnrollmentStatus status, decimal hours, DateOnly? completionDate)
    {
        if (!Enum.IsDefined(status))
            throw new BusinessValidationException("Estado de inscripción inválido");
        if (hours < 0 || hours > MaxHours)
            throw new BusinessValidationException("Las horas completadas no pueden ser negativas");
        if (status == EnrollmentStatus.Completed && !completionDate.HasValue)
            throw new BusinessValidationException("Una inscripción completada requiere fecha de finalización");

        enrollment.Status = status;
        enrollment.HoursCompleted = hours;
        enrollment.CompletionDate = status == EnrollmentStatus.Completed ? completionDate : null;
    }

    private static EducationProgramDto MapProgram(EducationProgram p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Type = p.Type,
        RequiredHours = p.RequiredHours,
        StartDate = p.StartDate,
        EndDate = p.EndDate
    };

    private static EnrollmentDto MapEnrollment(EducationEnrollment e) => new()
    {
        Id = e.Id,
        ProgramId = e.ProgramId,
        UserId = e.UserId,
        FullName = e.FullName ?? string.Empty,
        Status = e.Status,
        HoursCompleted = e.HoursCompleted,
        CompletionDate = e.CompletionDate
    };

    #endregion
}
