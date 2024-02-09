using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Status
{
    [JsonProperty("status")]
    public string StatusValue { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }
}

public class Payment
{
    [JsonProperty("issuerName")]
    public string IssuerName { get; set; }

    [JsonProperty("paymentMethodName")]
    public string PaymentMethodName { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }
}

public class Payer
{
    [JsonProperty("document")]
    public string Document { get; set; }

    [JsonProperty("documentType")]
    public string DocumentType { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("surname")]
    public string Surname { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("mobile")]
    public string Mobile { get; set; }
}

public class Request
{
    [JsonProperty("status")]
    public Status Status { get; set; }

    [JsonProperty("payer")]
    public Payer Payer { get; set; }

    [JsonProperty("payment")]
    public List<Payment> Payments { get; set; }
}

public class WebhookModel
{
    public int Id { get; set; }

    [JsonProperty("requestId")]
    public int RequestId { get; set; }

    [JsonProperty("request")]
    public Request Request { get; set; }
}
