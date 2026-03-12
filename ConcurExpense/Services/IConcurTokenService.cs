namespace ConcurExpense.Services;

internal interface IConcurTokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
