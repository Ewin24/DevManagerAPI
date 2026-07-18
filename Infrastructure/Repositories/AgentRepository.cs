using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repositorio para acciones del agente (auditoría y HITL)
/// </summary>
public class AgentRepository : IAgentRepository
{
    private readonly DevManagerDbContext _context;

    public AgentRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateActionAsync(
        Guid organizationId,
        string actionType,
        string description,
        string inputData,
        string outputData,
        string status,
        Guid? executedByUserId = null)
    {
        var action = new Domain.Entities.Agent.AgentAction
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            ActionType = actionType,
            Description = description,
            InputData = inputData,
            OutputData = outputData,
            Status = status,
            ExecutedByUserId = executedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.AgentActions.AddAsync(action);
        await _context.SaveChangesAsync();

        return action.Id;
    }

    public async Task UpdateActionStatusAsync(
        Guid actionId,
        string status,
        Guid? approvedByUserId = null)
    {
        var action = await _context.AgentActions
            .FirstOrDefaultAsync(a => a.Id == actionId);

        if (action == null)
            throw new InvalidOperationException($"AgentAction con Id {actionId} no encontrada");

        action.Status = status;
        action.ApprovedByUserId = approvedByUserId;
        action.ApprovedAt = approvedByUserId.HasValue ? DateTime.UtcNow : null;

        await _context.SaveChangesAsync();
    }

    public async Task<Domain.Entities.Agent.AgentAction?> GetActionByIdAsync(
        Guid actionId,
        Guid organizationId)
    {
        return await _context.AgentActions
            .FirstOrDefaultAsync(a => a.Id == actionId && a.OrganizationId == organizationId);
    }
}
