using Microsoft.AspNetCore.Mvc;
using PagBankIntegration.DTOs;
using PagBankIntegration.Services;

namespace PagBankIntegration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardTokenController : ControllerBase
{
    private readonly ICardTokenService          _tokenService;
    private readonly ILogger<CardTokenController> _logger;

    public CardTokenController(ICardTokenService tokenService, ILogger<CardTokenController> logger)
    {
        _tokenService = tokenService;
        _logger       = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CardTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> CreateToken([FromBody] CardTokenRequest request, CancellationToken ct)
    {
        if (ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _tokenService.CreateTokenAsync(request, ct);
            return Ok(result);
        }
        catch (PagBankException ex)
        {
            _logger.LogError(ex, "[CardToken] Falha ao criar token");
            return StatusCode(ex.StatusCode, new { ex.Message, ex.RawResponse });
        }
    }
}
