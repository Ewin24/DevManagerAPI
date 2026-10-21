namespace Domain.Enums;

/// <summary>
/// Tipo de entidad de economía solidaria de una organización
/// </summary>
public enum OrganizationEntityType : byte
{
    WorkerCooperative = 1,
    SavingsCredit = 2,
    Multiactive = 3,
    Mutual = 4,
    EmployeeFund = 5,
    Other = 6
}
