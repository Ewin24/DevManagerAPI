namespace Application.Tests.Services;

using Application.DTOs.Agent;
using Application.DTOs.Reports;
using Application.Interfaces;
using Application.Services;
using Domain.Entities.Projects;
using Domain.Entities.Talent;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para ReportsService (Fase 0 — Fundación)
/// </summary>
public class ReportsServiceTests
{
    private readonly Mock<IEmployeeSkillRepository> _employeeSkillRepoMock;
    private readonly Mock<ISkillRepository> _skillRepoMock;
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly Mock<IAssignmentRepository> _assignmentRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IProfileRepository> _profileRepoMock;
    private readonly Mock<IAgentService> _agentServiceMock;
    private readonly Mock<ILogger<ReportsService>> _loggerMock;
    private readonly ReportsService _reportsService;

    public ReportsServiceTests()
    {
        _employeeSkillRepoMock = new Mock<IEmployeeSkillRepository>();
        _skillRepoMock = new Mock<ISkillRepository>();
        _projectRepoMock = new Mock<IProjectRepository>();
        _assignmentRepoMock = new Mock<IAssignmentRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _profileRepoMock = new Mock<IProfileRepository>();
        _agentServiceMock = new Mock<IAgentService>();
        _loggerMock = new Mock<ILogger<ReportsService>>();

        _reportsService = new ReportsService(
            _employeeSkillRepoMock.Object,
            _skillRepoMock.Object,
            _projectRepoMock.Object,
            _assignmentRepoMock.Object,
            _userRepoMock.Object,
            _profileRepoMock.Object,
            _agentServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetSkillsDistributionAsync_ReturnsDistributionData()
    {
        // Arrange
        var orgId = Guid.NewGuid();

        var skillId1 = Guid.NewGuid();
        var skillId2 = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var skills = new List<Skill>
        {
            new() { Id = skillId1, Name = "C#", OrganizationId = orgId },
            new() { Id = skillId2, Name = "SQL", OrganizationId = orgId }
        };

        var users = new List<Domain.Entities.IAM.User>
        {
            new() { Id = userId1, OrganizationId = orgId, Email = "user1@test.com" },
            new() { Id = userId2, OrganizationId = orgId, Email = "user2@test.com" }
        };

        var user1Skills = new List<EmployeeSkill>
        {
            new() { Id = Guid.NewGuid(), UserId = userId1, SkillId = skillId1, Level = 4, OrganizationId = orgId },
            new() { Id = Guid.NewGuid(), UserId = userId1, SkillId = skillId2, Level = 3, OrganizationId = orgId }
        };

        var user2Skills = new List<EmployeeSkill>
        {
            new() { Id = Guid.NewGuid(), UserId = userId2, SkillId = skillId1, Level = 2, OrganizationId = orgId }
        };

        _skillRepoMock
            .Setup(r => r.GetAllAsync(orgId))
            .ReturnsAsync(skills);

        _userRepoMock
            .Setup(r => r.GetAllAsync(orgId))
            .ReturnsAsync(users);

        _employeeSkillRepoMock
            .Setup(r => r.GetByUserIdAsync(userId1, orgId))
            .ReturnsAsync(user1Skills);

        _employeeSkillRepoMock
            .Setup(r => r.GetByUserIdAsync(userId2, orgId))
            .ReturnsAsync(user2Skills);

        // Act
        var result = await _reportsService.GetSkillsDistributionAsync(orgId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var csharpSkill = result.First(s => s.SkillName == "C#");
        csharpSkill.TotalEmployees.Should().Be(2);
        csharpSkill.AverageLevel.Should().Be(3); // (4 + 2) / 2 = 3
        csharpSkill.LevelDistribution.Should().ContainKey("2");
        csharpSkill.LevelDistribution.Should().ContainKey("4");

        var sqlSkill = result.First(s => s.SkillName == "SQL");
        sqlSkill.TotalEmployees.Should().Be(1);
        sqlSkill.AverageLevel.Should().Be(3);
    }

    [Fact]
    public async Task GetProjectMetricsAsync_ReturnsMetricsData()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var projectId1 = Guid.NewGuid();
        var projectId2 = Guid.NewGuid();

        var activeProjects = new List<Project>
        {
            new()
            {
                Id = projectId1,
                OrganizationId = orgId,
                Name = "Portal Web",
                Status = ProjectStatus.InProgress
            },
            new()
            {
                Id = projectId2,
                OrganizationId = orgId,
                Name = "App Móvil",
                Status = ProjectStatus.InProgress
            }
        };

        var project1Requirements = new List<ProjectSkillRequirement>
        {
            new() { SkillName = "C#", IsMandatory = true },
            new() { SkillName = "Angular", IsMandatory = true }
        };

        var project2Requirements = new List<ProjectSkillRequirement>
        {
            new() { SkillName = "Kotlin", IsMandatory = false },
            new() { SkillName = "SQL", IsMandatory = true }
        };

        _projectRepoMock
            .Setup(r => r.GetAllAsync(orgId, ProjectStatus.InProgress))
            .ReturnsAsync(activeProjects);

        _projectRepoMock
            .Setup(r => r.GetSkillRequirementsAsync(projectId1, orgId))
            .ReturnsAsync(project1Requirements);

        _projectRepoMock
            .Setup(r => r.GetSkillRequirementsAsync(projectId2, orgId))
            .ReturnsAsync(project2Requirements);

        // Act
        var result = await _reportsService.GetProjectMetricsAsync(orgId);

        // Assert
        result.Should().NotBeNull();
        result.TotalActiveProjects.Should().Be(2);
        result.ProjectsAtRisk.Should().Be(2); // Both have mandatory skills
        result.MostDemandedSkills.Should().HaveCount(4); // 4 unique skills, top 5 picks all

        result.MostDemandedSkills.Should().Contain(s => s.SkillName == "C#" && s.RequiredInProjects == 1);
        result.MostDemandedSkills.Should().Contain(s => s.SkillName == "Angular" && s.RequiredInProjects == 1);
        result.MostDemandedSkills.Should().Contain(s => s.SkillName == "SQL" && s.RequiredInProjects == 1);
    }
}
