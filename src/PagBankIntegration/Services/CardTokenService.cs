using PagBankIntegration.DTOs;

namespace PagBankIntegration.Services;

public interface ICardTokenService
{
    Task<CardTokenResponse> CreateTokenAsync(CardTokenRequest request, CancellationToken ct = default);
}

public class CardTokenService : ICardTokenService
{
    private readonly IPagBankService          _pagBank;
    private readonly ILogger<CardTokenService> _logger;

    public CardTokenService(IPagBankService pagBank, ILogger<CardTokenService> logger)
    {
        _pagBank = pagBank;
        _logger  = logger;
    }

    public async Task<CardTokenResponse> CreateTokenAsync(CardTokenRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("[CardToken] Tokenizando cartao terminado em {Last4}",
            request.Number[4..]);
        var payload = new
        {
            type = "CREDIT_CARD",
            card = new
            {
                number        = request.Number,
                exp_month     = request.ExpMonth,
                exp_year      = request.ExpYear,
                security_code = request.SecurityCode,
                holder        = new { name = request.HolderName }
            }
        };
        var result = await _pagBank.PostAsync<object, CardTokenResponse>("/cards", payload, ct);
        _logger.LogInformation("[CardToken] Token criado | Id: {TokenId}", result.Id);
        return result;
    }
}
