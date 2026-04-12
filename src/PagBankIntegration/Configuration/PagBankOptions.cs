namespace PagBankIntegration.Configuration;

public class PagBankOptions
{
    public string BaseUrl  { get; set; } = "https://sandbox.api.pagseguro.com";
    public string Token    { get; set; } = string.Empty;
    public bool   IsSandbox{ get; set; } = true;
}
