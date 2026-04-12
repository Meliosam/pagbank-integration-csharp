using PagBankIntegration.DTOs;

namespace PagBankIntegration.Services;

public interface IPaymentService
{
    Task<ChargeResponse> ChargeAsync(ChargeRequest request, CancellationToken ct = default);
}

public class PaymentService : IPaymentService
{
    private readonly IPagBankService        _pagBank;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPagBankService pagBank, ILogger<PaymentService> logger)
    {
        _pagBank = pagBank;
        _logger  = logger;
    }

    public async Task<ChargeResponse> ChargeAsync(ChargeRequest request, CancellationToken ct = default)
    {
        var referenceId = $"REF-{Guid.NewGuid():N}";
        _logger.LogInformation("[Payment] Cobranca | Ref: {Ref} | Valor: {Amount}",
            referenceId, request.Amount);
        var payload = new
        {
            reference_id = referenceId,
            charges      = new[]
            {
                new
                {
                    reference_id   = referenceId,
                    description    = request.Description,
                    amount         = new
                    {
                        value    = (int)(request.Amount * 100),
                        currency = request.Currency
                    },
                    payment_method = new
                    {
                        type         = "CREDIT_CARD",
                        installments = request.Installments,
                        capture      = request.Capture,
                        card         = new { id = request.CardTokenId }
                    }
                }
            }
        };
        var result = await _pagBank.PostAsync<object, ChargeResponse>("/charges", payload, ct);
        _logger.LogInformation("[Payment] Concluido | Id: {Id} | Status: {Status}",
            result.Id, result.Status);
        return result;
    }
}
