namespace ConcurExpense.Models;

public record ItemizationDto : ExpenseBaseDto
{
    // Required
    public required string EntryID { get; init; }
    public required string ExpenseTypeCode { get; init; }
    public required string ExpenseTypeName { get; init; }
    public required string ReportID { get; init; }
    public required string ReportOwnerID { get; init; }
    public required string SpendCategoryCode { get; init; }
    public required string SpendCategoryName { get; init; }
    public required DateTime TransactionDate { get; init; }

    // Optional fields
    public string? AllocationType { get; init; }
    public decimal ApprovedAmount { get; init; }
    public string? Description { get; init; }
    public bool HasComments { get; init; }
    public bool HasExceptions { get; init; }
    public bool IsBillable { get; init; }
    public bool IsImageRequired { get; init; }
    public bool IsPersonal { get; init; }
    public DateTime? LastModified { get; init; }
    public string? LocationCountry { get; init; }
    public string? LocationID { get; init; }
    public string? LocationName { get; init; }
    public string? LocationSubdivision { get; init; }
    public decimal PostedAmount { get; init; }
    public decimal TransactionAmount { get; init; }

    // Org unit fields
    public CustomFieldDto? OrgUnit1 { get; init; }
    public CustomFieldDto? OrgUnit2 { get; init; }
    public CustomFieldDto? OrgUnit3 { get; init; }
    public CustomFieldDto? OrgUnit4 { get; init; }
    public CustomFieldDto? OrgUnit5 { get; init; }
    public CustomFieldDto? OrgUnit6 { get; init; }

    // Custom fields
    public CustomFieldDto? Custom1 { get; init; }
    public CustomFieldDto? Custom2 { get; init; }
    public CustomFieldDto? Custom3 { get; init; }
    public CustomFieldDto? Custom4 { get; init; }
    public CustomFieldDto? Custom5 { get; init; }
    public CustomFieldDto? Custom6 { get; init; }
    public CustomFieldDto? Custom7 { get; init; }
    public CustomFieldDto? Custom8 { get; init; }
    public CustomFieldDto? Custom9 { get; init; }
    public CustomFieldDto? Custom10 { get; init; }
    public CustomFieldDto? Custom11 { get; init; }
    public CustomFieldDto? Custom12 { get; init; }
    public CustomFieldDto? Custom13 { get; init; }
    public CustomFieldDto? Custom14 { get; init; }
    public CustomFieldDto? Custom15 { get; init; }
    public CustomFieldDto? Custom16 { get; init; }
    public CustomFieldDto? Custom17 { get; init; }
    public CustomFieldDto? Custom18 { get; init; }
    public CustomFieldDto? Custom19 { get; init; }
    public CustomFieldDto? Custom20 { get; init; }
    public CustomFieldDto? Custom21 { get; init; }
    public CustomFieldDto? Custom22 { get; init; }
    public CustomFieldDto? Custom23 { get; init; }
    public CustomFieldDto? Custom24 { get; init; }
    public CustomFieldDto? Custom25 { get; init; }
    public CustomFieldDto? Custom26 { get; init; }
    public CustomFieldDto? Custom27 { get; init; }
    public CustomFieldDto? Custom28 { get; init; }
    public CustomFieldDto? Custom29 { get; init; }
    public CustomFieldDto? Custom30 { get; init; }
    public CustomFieldDto? Custom31 { get; init; }
    public CustomFieldDto? Custom32 { get; init; }
    public CustomFieldDto? Custom33 { get; init; }
    public CustomFieldDto? Custom34 { get; init; }
    public CustomFieldDto? Custom35 { get; init; }
    public CustomFieldDto? Custom36 { get; init; }
    public CustomFieldDto? Custom37 { get; init; }
    public CustomFieldDto? Custom38 { get; init; }
    public CustomFieldDto? Custom39 { get; init; }
    public CustomFieldDto? Custom40 { get; init; }
}
