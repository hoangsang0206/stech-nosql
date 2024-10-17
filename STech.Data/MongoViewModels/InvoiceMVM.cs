using STech.Data.Models;

namespace STech.Data.MongoViewModels
{
    public class InvoiceMVM
    {
        public string InvoiceId { get; set; } = null!;

        public DateTime? OrderDate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }

        public string PaymentMedId { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public string DeliveryMedId { get; set; } = null!;

        public string? DeliveryAddress { get; set; }

        public string RecipientPhone { get; set; } = null!;

        public string RecipientName { get; set; } = null!;

        public string? Note { get; set; }

        public bool IsCompleted { get; set; }

        public string? CustomerId { get; set; }

        public string? UserId { get; set; }

        public string? EmployeeId { get; set; }

        public bool IsCancelled { get; set; }

        public DateTime? CancelledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public bool IsAccepted { get; set; }

        public DateTime? AcceptedDate { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        public DeliveryMethod? DeliveryMethod { get; set; }

        public PackingSlip? PackingSlip { get; set; }

        public List<InvoiceDetailMVM> InvoiceDetails { get; set; } = new List<InvoiceDetailMVM>();

        public List<InvoiceStatus> InvoiceStatuses { get; set; } = new List<InvoiceStatus>();
    }
}
