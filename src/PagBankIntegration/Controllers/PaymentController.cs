using Microsoft.AspNetCore.Mvc;
using PagBankIntegration.DTOs;
using PagBankIntegration.Services;

namespace PagBankIntegration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService          _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger         = logger;
    }

    [HttpPost("charge")]
    [ProducesResponseType(typeof(ChargeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Charge([FromBody] ChargeRequest request, CancellationToken ct)
    {
        if (ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _paymentService.ChargeAsync(request, ct);
            return Ok(result);
        }
        catch (PagBankException ex)
        {
            _logger.LogError(ex, "[Payment] Falha ao processar cobranca");
            return StatusCode(ex.StatusCode, new { ex.Message, ex.RawResponse });
        }
    }
}
