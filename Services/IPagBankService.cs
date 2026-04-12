namespace PagBankIntegration.Services;

public interface IPagBankService
{
    Task<T> PostAsync<TRequest, T>(string endpoint, TRequest body, CancellationToken ct = default);
    Task<T> GetAsync<T>(string endpoint, CancellationToken ct = default);
}
