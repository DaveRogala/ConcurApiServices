namespace ConcurExpense.Tests.Helpers;

internal static class JsonPayloads
{
    public static string TokenResponse(string accessToken = "test-token", int expiresIn = 3600) => $$"""
        {
          "access_token": "{{accessToken}}",
          "token_type": "Bearer",
          "expires_in": {{expiresIn}}
        }
        """;

    public static string ReportPage(string? nextPage = null, params object[] ids)
    {
        var items = string.Join(",\n", ids.Select(id => $$"""
            {
              "ID": "{{id}}",
              "Name": "Report {{id}}",
              "Total": 100.00,
              "CurrencyCode": "USD",
              "ApprovalStatusCode": "A_APPR",
              "ApprovalStatusName": "Approved",
              "OwnerLoginID": "user@example.com",
              "OwnerName": "Test User",
              "LastModifiedDate": "2024-01-15T10:00:00",
              "OrgUnit1": { "Code": "OU1", "Value": "Org Unit 1", "Type": "List", "ListItemID": "abc" }
            }
            """));

        var nextPageJson = nextPage is null ? "null" : $"\"{nextPage}\"";
        return $$"""{ "Items": [{{items}}], "NextPage": {{nextPageJson}}, "TotalCount": {{ids.Length}} }""";
    }

    public static string EntryPage(string reportId, string? nextPage = null, params object[] ids)
    {
        var items = string.Join(",\n", ids.Select(id => $$"""
            {
              "ID": "{{id}}",
              "ReportID": "{{reportId}}",
              "ExpenseTypeCode": "MEALS",
              "ExpenseTypeName": "Meals",
              "TransactionAmount": 50.00,
              "TransactionCurrencyCode": "USD",
              "TransactionDate": "2024-01-10T00:00:00",
              "PaymentTypeID": "CASH",
              "Custom1": { "Code": "C1", "Value": "Custom Value 1", "Type": "Text", "ListItemID": null }
            }
            """));

        var nextPageJson = nextPage is null ? "null" : $"\"{nextPage}\"";
        return $$"""{ "Items": [{{items}}], "NextPage": {{nextPageJson}}, "TotalCount": {{ids.Length}} }""";
    }

    public static string ItemizationPage(string entryId, string reportId, string? nextPage = null, params object[] ids)
    {
        var items = string.Join(",\n", ids.Select(id => $$"""
            {
              "ID": "{{id}}",
              "EntryID": "{{entryId}}",
              "ReportID": "{{reportId}}",
              "ExpenseTypeCode": "BRKFT",
              "ExpenseTypeName": "Breakfast",
              "ReportOwnerID": "owner@example.com",
              "SpendCategoryCode": "MEALS",
              "SpendCategoryName": "Meals & Entertainment",
              "TransactionDate": "2024-01-10T00:00:00",
              "Custom1": { "Code": "C1", "Value": "Item Value", "Type": "Text", "ListItemID": null }
            }
            """));

        var nextPageJson = nextPage is null ? "null" : $"\"{nextPage}\"";
        return $$"""{ "Items": [{{items}}], "NextPage": {{nextPageJson}}, "TotalCount": {{ids.Length}} }""";
    }

    public static string AllocationPage(string? nextPage = null, params object[] ids)
    {
        var items = string.Join(",\n", ids.Select(id => $$"""
            {
              "ID": "{{id}}",
              "EntryID": "entry-1",
              "Percentage": "100.00",
              "IsHidden": false,
              "IsPercentEdited": false,
              "Custom1": { "Code": "GL1", "Value": "GL Account", "Type": "List", "ListItemID": "gl1" }
            }
            """));

        var nextPageJson = nextPage is null ? "null" : $"\"{nextPage}\"";
        return $$"""{ "Items": [{{items}}], "NextPage": {{nextPageJson}}, "TotalCount": {{ids.Length}} }""";
    }

    public static string EmptyPage() =>
        """{ "Items": [], "NextPage": null, "TotalCount": 0 }""";
}
