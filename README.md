# 💳 PagBank Integration — C# / ASP.NET Core

Integração completa com a API do **PagBank (PagSeguro)** em C#, com suporte a **tokenização de cartão de crédito**, cobrança segura e logging estruturado.

---

## 🚀 Funcionalidades

- ✅ Tokenização de cartão de crédito via API PagBank
- ✅ Cobrança usando card token (dados brutos nunca são persistidos)
- ✅ Logging estruturado com Serilog (console + arquivo rotacionado)
- ✅ Middleware de logging de requisições com Request ID
- ✅ Tratamento de erros com exceções personalizadas
- ✅ Suporte a ambiente Sandbox e Produção
- ✅ Documentação interativa via Swagger/OpenAPI
- ✅ Injeção de dependência nativa do ASP.NET Core

---

## 🏗️ Estrutura do Projeto

```
PagBankIntegration/
├── src/
│   ├── Controllers/
│   │   ├── CardTokenController.cs   # Endpoint de tokenização
│   │   └── PaymentController.cs     # Endpoint de cobrança
│   ├── Services/
│   │   ├── PagBankService.cs        # HTTP client base (auth, logs, erros)
│   │   ├── CardTokenService.cs      # Lógica de tokenização
│   │   └── PaymentService.cs        # Lógica de cobrança
│   ├── DTOs/
│   │   └── PaymentDtos.cs           # Modelos de request/response
│   ├── Configuration/
│   │   └── PagBankOptions.cs        # Configurações tipadas
│   ├── Middleware/
│   │   └── RequestLoggingMiddleware.cs
│   ├── appsettings.json
│   └── Program.cs
└── README.md
```

---

## ⚙️ Configuração

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/pagbank-integration-csharp.git
cd pagbank-integration-csharp
```

### 2. Configure suas credenciais

Edite o `appsettings.json`:

```json
{
  "PagBank": {
    "BaseUrl": "https://sandbox.api.pagseguro.com",
    "Token": "SEU_TOKEN_AQUI",
    "IsSandbox": true
  }
}
```

> ⚠️ **Nunca commite seu token no repositório.** Use `dotnet user-secrets` ou variáveis de ambiente em produção:
> ```bash
> dotnet user-secrets set "PagBank:Token" "seu_token_aqui"
> ```

### 3. Execute

```bash
cd src
dotnet run
```

Acesse o Swagger em: `https://localhost:5001/swagger`

---

## 📡 Endpoints

### `POST /api/cardtoken` — Tokenizar Cartão

```json
{
  "number": "4111111111111111",
  "expMonth": "12",
  "expYear": "2030",
  "securityCode": "123",
  "holderName": "JOAO S"
}
```

**Resposta:**
```json
{
  "id": "token_abc123xyz",
  "brand": "VISA",
  "lastDigits": "1111",
  "expMonth": "12",
  "expYear": "2030"
}
```

---

### `POST /api/payment/charge` — Realizar Cobrança

```json
{
  "cardTokenId": "token_abc123xyz",
  "amount": 150.00,
  "description": "Pedido #1001",
  "installments": 1,
  "capture": true
}
```

**Resposta:**
```json
{
  "id": "CHARGE_ABC123",
  "referenceId": "REF-xxxxxxxxxxxx",
  "status": "PAID",
  "amount": {
    "value": 15000,
    "currency": "BRL"
  }
}
```

---

## 🔒 Segurança (PCI)

- Os dados brutos do cartão **nunca são armazenados** — apenas o token é trafegado após a tokenização
- Toda comunicação com a API PagBank é via **HTTPS**
- O token de autenticação é lido via configuração segura (não hardcoded)
- Recomenda-se usar **HTTPS obrigatório** em produção e restringir CORS

---

## 📋 Logs

Os logs são gerados automaticamente em dois destinos:

| Destino | Localização |
|---------|-------------|
| Console | Saída padrão (desenvolvimento) |
| Arquivo | `logs/pagbank-YYYYMMDD.log` |

Exemplo de log:
```
[REQ-A1B2C3D4] ▶ POST /api/cardtoken
[PagBank] POST /cards | Payload: {...}
[CardToken] Token criado com sucesso | Id: token_abc123xyz
[REQ-A1B2C3D4] ◀ 200 (312ms)
```

---

## 🧪 Testando em Sandbox

Use os cartões de teste oficiais do PagBank:

| Bandeira | Número | CVV | Validade |
|----------|--------|-----|----------|
| Visa | `4111111111111111` | `123` | qualquer futura |
| Mastercard | `5500000000000004` | `123` | qualquer futura |

---

## 🛠️ Tecnologias

- [.NET 8](https://dotnet.microsoft.com/)
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core)
- [Serilog](https://serilog.net/)
- [Swashbuckle / Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [PagBank API v4](https://dev.pagbank.uol.com.br/reference)

---

## 📄 Licença

MIT © seu-usuario
