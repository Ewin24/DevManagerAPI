using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevManager.AgentClient;

/// <summary>
/// Cliente de ejemplo para integrar el Agente de DevManager en aplicaciones .NET
/// </summary>
public class DevManagerAgentClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string? _token;

    public DevManagerAgentClient(string baseUrl = "http://localhost:5073")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    #region Authentication

    /// <summary>
    /// Login para obtener JWT token
    /// </summary>
    public async Task<bool> LoginAsync(string email, string password)
    {
        var loginRequest = new { email, password };
        
        var response = await _httpClient.PostAsJsonAsync("/auth/login", loginRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error en login: {response.StatusCode}");
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        _token = result?.Data?.Token;

        if (!string.IsNullOrEmpty(_token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _token);
            Console.WriteLine("Login exitoso. Token configurado.");
            return true;
        }

        return false;
    }

    #endregion

    #region Agent Queries

    /// <summary>
    /// Consulta general al agente en lenguaje natural
    /// </summary>
    public async Task<AgentQueryResponse?> QueryAgentAsync(
        string query, 
        string? context = null, 
        bool requireApproval = false)
    {
        EnsureAuthenticated();

        var request = new
        {
            query,
            context,
            requireApproval
        };

        var response = await _httpClient.PostAsJsonAsync("/agent/query", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AgentQueryResponse>>();
        return result?.Data;
    }

    /// <summary>
    /// Validación semántica de skills
    /// </summary>
    public async Task<SkillValidationResponse?> ValidateSkillAsync(
        Guid userId,
        Guid skillId,
        int level,
        string? evidenceUrl = null,
        List<Guid>? certificationIds = null)
    {
        EnsureAuthenticated();

        var request = new
        {
            userId,
            skillId,
            level,
            evidenceUrl,
            relatedCertificationIds = certificationIds
        };

        var response = await _httpClient.PostAsJsonAsync("/agent/validate-skill", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<SkillValidationResponse>>();
        return result?.Data;
    }

    /// <summary>
    /// Matching inteligente de candidatos para un proyecto
    /// </summary>
    public async Task<SkillMatchResponse?> MatchCandidatesAsync(
        Guid projectId,
        int maxCandidates = 10)
    {
        EnsureAuthenticated();

        var request = new
        {
            projectId,
            maxCandidates,
            includeReasoningDetails = true
        };

        var response = await _httpClient.PostAsJsonAsync("/agent/match-candidates", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<SkillMatchResponse>>();
        return result?.Data;
    }

    /// <summary>
    /// Aprobar una acción del agente (HITL)
    /// </summary>
    public async Task<bool> ApproveAgentActionAsync(Guid actionId)
    {
        EnsureAuthenticated();

        var response = await _httpClient.PostAsync($"/agent/approve/{actionId}", null);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Rechazar una acción del agente
    /// </summary>
    public async Task<bool> RejectAgentActionAsync(Guid actionId, string reason)
    {
        EnsureAuthenticated();

        var request = new { reason };
        var response = await _httpClient.PostAsJsonAsync($"/agent/reject/{actionId}", request);
        return response.IsSuccessStatusCode;
    }

    #endregion

    #region Helpers

    private void EnsureAuthenticated()
    {
        if (string.IsNullOrEmpty(_token))
        {
            throw new InvalidOperationException("No autenticado. Llame a LoginAsync primero.");
        }
    }

    #endregion

    #region DTOs

    private record LoginResponse(bool Success, string Message, LoginData? Data);
    private record LoginData(string Token, UserInfo User);
    private record UserInfo(Guid Id, string FirstName, string LastName, string Email);

    private record ApiResponse<T>(bool Success, string Message, T? Data);

    public record AgentQueryResponse(
        string Response,
        string ReasoningSteps,
        List<ToolExecutionResult> ToolsExecuted,
        bool RequiresHumanApproval,
        Guid? ActionId);

    public record ToolExecutionResult(
        string ToolName,
        object Input,
        object Output,
        bool Success,
        string? ErrorMessage);

    public record SkillValidationResponse(
        bool IsValid,
        double ConfidenceScore,
        string ValidationReasoning,
        List<string> Recommendations,
        List<string> Warnings,
        bool RequiresCertification);

    public record SkillMatchResponse(
        Guid ProjectId,
        string ProjectName,
        List<CandidateMatch> Candidates,
        string AnalysisNarrative);

    public record CandidateMatch(
        Guid UserId,
        string FullName,
        string Email,
        double MatchScore,
        List<SkillAlignment> SkillAlignments,
        string RecommendationReason);

    public record SkillAlignment(
        string SkillName,
        int RequiredLevel,
        int CurrentLevel,
        bool IsMandatory,
        bool Meets);

    #endregion
}

/// <summary>
/// Programa de ejemplo
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var client = new DevManagerAgentClient("http://localhost:5073");

        // 1. Login
        Console.WriteLine("=== DevManager AI Agent - Demo ===\n");
        Console.Write("Email: ");
        var email = Console.ReadLine();
        Console.Write("Password: ");
        var password = ReadPassword();

        if (!await client.LoginAsync(email!, password!))
        {
            Console.WriteLine("\nError en login. Saliendo...");
            return;
        }

        while (true)
        {
            Console.WriteLine("\n=== Menú ===");
            Console.WriteLine("1. Consulta general al agente");
            Console.WriteLine("2. Validar skill");
            Console.WriteLine("3. Matching de candidatos");
            Console.WriteLine("4. Salir");
            Console.Write("\nOpción: ");

            var option = Console.ReadLine();

            try
            {
                switch (option)
                {
                    case "1":
                        await DemoGeneralQuery(client);
                        break;
                    case "2":
                        await DemoSkillValidation(client);
                        break;
                    case "3":
                        await DemoCandidateMatching(client);
                        break;
                    case "4":
                        Console.WriteLine("\n¡Hasta luego!");
                        return;
                    default:
                        Console.WriteLine("\nOpción inválida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
    }

    static async Task DemoGeneralQuery(DevManagerAgentClient client)
    {
        Console.Write("\nEscribe tu consulta al agente: ");
        var query = Console.ReadLine();

        Console.WriteLine("\n⏳ Consultando al agente...\n");

        var response = await client.QueryAgentAsync(query!, requireApproval: false);

        if (response != null)
        {
            Console.WriteLine("📝 Respuesta:");
            Console.WriteLine(response.Response);
            Console.WriteLine("\n🧠 Razonamiento:");
            Console.WriteLine(response.ReasoningSteps);
            
            if (response.RequiresHumanApproval)
            {
                Console.WriteLine($"\n⚠️ Esta acción requiere aprobación humana (ID: {response.ActionId})");
            }
        }
    }

    static async Task DemoSkillValidation(DevManagerAgentClient client)
    {
        Console.Write("\nUser ID (GUID): ");
        var userIdStr = Console.ReadLine();
        Console.Write("Skill ID (GUID): ");
        var skillIdStr = Console.ReadLine();
        Console.Write("Nivel (1-5): ");
        var levelStr = Console.ReadLine();

        if (!Guid.TryParse(userIdStr, out var userId) ||
            !Guid.TryParse(skillIdStr, out var skillId) ||
            !int.TryParse(levelStr, out var level))
        {
            Console.WriteLine("\n❌ Datos inválidos.");
            return;
        }

        Console.WriteLine("\n⏳ Validando skill...\n");

        var response = await client.ValidateSkillAsync(userId, skillId, level);

        if (response != null)
        {
            Console.WriteLine($"✅ Válido: {(response.IsValid ? "SÍ" : "NO")}");
            Console.WriteLine($"📊 Confianza: {response.ConfidenceScore:F2}%");
            Console.WriteLine($"\n💡 Razonamiento:");
            Console.WriteLine(response.ValidationReasoning);

            if (response.Recommendations.Any())
            {
                Console.WriteLine("\n📌 Recomendaciones:");
                foreach (var rec in response.Recommendations)
                {
                    Console.WriteLine($"  • {rec}");
                }
            }

            if (response.Warnings.Any())
            {
                Console.WriteLine("\n⚠️ Advertencias:");
                foreach (var warning in response.Warnings)
                {
                    Console.WriteLine($"  • {warning}");
                }
            }
        }
    }

    static async Task DemoCandidateMatching(DevManagerAgentClient client)
    {
        Console.Write("\nProject ID (GUID): ");
        var projectIdStr = Console.ReadLine();
        Console.Write("Max candidatos (default 10): ");
        var maxStr = Console.ReadLine();

        if (!Guid.TryParse(projectIdStr, out var projectId))
        {
            Console.WriteLine("\n❌ Project ID inválido.");
            return;
        }

        int max = string.IsNullOrWhiteSpace(maxStr) ? 10 : int.Parse(maxStr);

        Console.WriteLine("\n⏳ Buscando candidatos...\n");

        var response = await client.MatchCandidatesAsync(projectId, max);

        if (response != null)
        {
            Console.WriteLine($"📊 Proyecto: {response.ProjectName}");
            Console.WriteLine($"👥 Candidatos encontrados: {response.Candidates.Count}\n");

            foreach (var candidate in response.Candidates.Take(5))
            {
                Console.WriteLine($"✨ {candidate.FullName} ({candidate.Email})");
                Console.WriteLine($"   Score: {candidate.MatchScore:F2}/100");
                Console.WriteLine($"   Razón: {candidate.RecommendationReason}");
                Console.WriteLine();
            }

            Console.WriteLine($"📝 Análisis:\n{response.AnalysisNarrative}");
        }
    }

    static string ReadPassword()
    {
        var password = string.Empty;
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                password += keyInfo.KeyChar;
                Console.Write("*");
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }
}
