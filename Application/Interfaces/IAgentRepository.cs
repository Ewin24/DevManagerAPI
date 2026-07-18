namespace Application.Interfaces;

/// <summary>
/// Repositorio para acciones del agente (auditoría)
/// </summary>
public interface IAgentRepository
{
    Task<Guid> CreateActionAsync(
        Guid organizationId,
        string actionType,
        string description,
        string inputData,
        string outputData,
        string status,
        Guid? executedByUserId = null);

    Task UpdateActionStatusAsync(
        Guid actionId,
        string status,
        Guid? approvedByUserId = null);

    Task<Domain.Entities.Agent.AgentAction?> GetActionByIdAsync(
        Guid actionId,
        Guid organizationId);
}
