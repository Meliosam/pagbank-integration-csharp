namespace PagBankIntegration.DTOs;

public record CardTokenRequest(
    string Number,
    string ExpMonth,
    string ExpYear,
    string SecurityCode,
    string HolderName
);

public record CardTokenResponse(
    string   Id,
    string   Brand,
    string   FirstDigits,
    string   LastDigits,
    string   ExpMonth,
    string   ExpYear,
    DateTime CreatedAt
);

public record ChargeRequest(
    string  CardTokenId,
    decimal Amount,
    string  Description,
    int     Installments = 1,
    bool    Capture      = true,
    string  Currency     = "BRL"
);

public record ChargeResponse(
    string         Id,
    string         ReferenceId,
    string         Status,
    ChargeAmountDto Amount,
    string?        ErrorMessages
);

public record ChargeAmountDto(int Value, string Currency);

public record PagBankErrorResponse(
    string?                          Error_messages,
    string?                          Description,
    IEnumerable<PagBankErrorDetail>? Errors
);

public record PagBankErrorDetail(string Code, string Description);
