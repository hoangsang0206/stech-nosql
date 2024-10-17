namespace STech.Data.ViewModels
{
    public class PaymentStatusVM
    {
        public string? InvoiceId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
