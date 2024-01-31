using System;

public class WebhookModel
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public DateTime Date { get; set; }
    public string Document { get; set; }
    public string DocumentType { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public int Total { get; set; }
    public string Message { get; set; }
    public string IssuerName { get; set; }
    public string PaymentMethodName { get; set; }
}