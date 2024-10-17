using STech.Data.Models;
using STech.Data.MongoViewModels;

namespace STech.Services
{
    public interface IOrderService
    {
        Task<bool> CreateInvoice(Invoice invoice, PackingSlip packingSlip, IEnumerable<WarehouseExport> wHEs);
        Task<Invoice?> GetInvoice(string invoiceId);
        Task<InvoiceMVM?> GetInvoiceWithDetails(string invoiceId);
        Task<InvoiceMVM?> GetInvoiceWithDetails(string invoiceId, string phone);
        Task<InvoiceMVM?> GetUserInvoiceWithDetails(string invoiceId, string userId);
        Task<IEnumerable<InvoiceMVM>> GetUserInvoices(string userId);
        Task<bool> CheckUserIsPurchased(string userId, string productId);
        Task<bool> CheckIsPurchased(string? phone, string productId);
        Task<bool> AddInvoiceStatus(string invoiceId, InvoiceStatus invoiceStatus);
        Task<bool> UpdatePaymentStatus(string invoiceId, string status);
        Task<(IEnumerable<InvoiceMVM>, int)> GetInvoices(int page, string? filterBy, string? sortBy);
        Task<IEnumerable<InvoiceMVM>> SearchInvoices(string query);

        Task<bool> AcceptOrder(string invoiceId);
        Task<bool> CancelOrder(string invoiceId);
    }
}
