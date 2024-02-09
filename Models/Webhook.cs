using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Prueba2Webhook.Models
{
    public class StatusDetails
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

    }
    public class WebhookListModel
    {
        [JsonIgnore]
        public int WebhookModelId { get; set; }
        public int RequestId { get; set; }
        public StatusDetails Status { get; set; }
        public Request Request { get; set; }
        [JsonIgnore]
        public PayerDetails Payer { get; set; }
        public List<PaymentDetails> Payment { get; set; }
        [JsonIgnore]
        public AmountDetails Amount { get; set; }
    }


    public class PayerDetails
    {
        public string Document { get; set; }
        public string DocumentType { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }

    public class PaymentDetails
    {
        public string IssuerName { get; set; }
        public string PaymentMethodName { get; set; }
        public AmountDetails Amount { get; set; }
    }

    public class AmountDetails
    {
        public ToDetails To { get; set; }
    }

    public class ToDetails
    {
        public int Total { get; set; }
    }


    public partial class Payer
    {
        public Payer()
        {
            Requests = new HashSet<Request>();
        }

        [JsonIgnore]
        public int Id { get; set; }
        public string Document { get; set; }
        public string DocumentType { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        [JsonIgnore]
        public virtual ICollection<Request> Requests { get; set; }
    }

    public partial class Payment
    {
        public Payment()
        {
            Amounts = new HashSet<Amount>();
            Requests = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string IssuerName { get; set; }
        public string PaymentMethodName { get; set; }

        public virtual ICollection<Amount> Amounts { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }
    public partial class Amount
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int? Total { get; set; }
        [JsonIgnore]
        public int? PaymentId { get; set; }
        [JsonIgnore]
        public virtual Payment Payment { get; set; }
    }


    public partial class Request
    {
        public Request()
        { 
            WebhookModels = new HashSet<WebhookModel>();
            Payments = new HashSet<Payment>();
        }

        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int? StatusId { get; set; }
        [JsonIgnore]
        public int? PayerId { get; set; }

        [JsonIgnore]
        public virtual Status Status { get; set; }
        public virtual Payer Payer { get; set; }
        [JsonIgnore]
        public virtual ICollection<WebhookModel> WebhookModels { get; set; }
        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; }
    }

    public partial class Status
    {
        public Status()
        {
            Requests = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string StatusValue { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }

    public partial class WebhookModel
    {
        public int Id { get; set; }
        public int? RequestId { get; set; }
        public virtual Request Request { get; set; }
    }
}
