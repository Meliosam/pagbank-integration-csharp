using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PagBankIntegration.Configuration;

namespace PagBankIntegration.Services;

public class PagBankService : IPagBankService
{
    private readonly HttpClient              _http;
    private readonly ILogger<PagBankService> _logger;
    private readonly PagBankOptions          _options;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented        = false
    };

    public PagBankService(HttpClient http, IOptions<PagBankOptions> options, ILogger<PagBankService> logger)
    {
        _options          = options.Value;
        _logger           = logger;
        _http             = http;
        _http.BaseAddress = new Uri(_options.BaseUrl);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.Token}");
        _http.DefaultRequestHeaders.Add("x-api-version", "4.0");
    }

    public async Task<T> PostAsync<TRequest, T>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(body, JsonOpts);
        _logger.LogInformation("[PagBank] POST {Endpoint} | Payload: {Payload}", endpoint, json);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response      = await _http.PostAsync(endpoint, content, ct);
        return await HandleResponse<T>(response, endpoint);
    }

    public async Task<T> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        _logger.LogInformation("[PagBank] GET {Endpoint}", endpoint);
        var response = await _http.GetAsync(endpoint, ct);
        return await HandleResponse<T>(response, endpoint);
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response, string endpoint)
    {
        var raw = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("[PagBank] {Status} {Endpoint} | Body: {Body}",
            (int)response.StatusCode, endpoint, raw);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogError("[PagBank] Erro {Status} em {Endpoint}: {Body}",
                (int)response.StatusCode, endpoint, raw);
            throw new PagBankException(
                $"PagBank retornou {^(int^)response.StatusCode}", raw, (int)response.StatusCode);
        }
        return JsonSerializer.Deserialize<T>(raw, JsonOpts)
            ?? throw new InvalidOperationException("Resposta vazia da API PagBank.");
    }
}

public class PagBankException : Exception
{
    public string RawResponse { get; }
    public int    StatusCode  { get; }
    public PagBankException(string message, string rawResponse, int statusCode) : base(message)
    {
        RawResponse = rawResponse;
        StatusCode  = statusCode;
    }
}
