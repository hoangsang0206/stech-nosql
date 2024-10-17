using MongoDB.Driver;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Invoice> _collection;
        private readonly IMongoCollection<PackingSlip> _pksCollection;
        private readonly IMongoCollection<WarehouseExport> _wheCollection;

        private readonly IMongoCollection<Product> _pCollection;
        private readonly IMongoCollection<PaymentMethod> _pmCollection;

        private readonly int NumOfInvoicePerPage = 20;

        public OrderService(StechDbContext context)
        {
            _collection = context.GetCollection<Invoice>("Invoices");
            _pksCollection = context.GetCollection<PackingSlip>("PackingSlips");
            _wheCollection = context.GetCollection<WarehouseExport>("WarehouseExports");
            _pCollection = context.GetCollection<Product>("Products");
            _pmCollection = context.GetCollection<PaymentMethod>("PaymentMethods");
        }

        public async Task<bool> CreateInvoice(Invoice invoice, PackingSlip packingSlip, IEnumerable<WarehouseExport> wHEs)
        {
            try
            {

                await _collection.InsertOneAsync(invoice);
                await _pksCollection.InsertOneAsync(packingSlip);
                await _wheCollection.InsertManyAsync(wHEs);

                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<Invoice?> GetInvoice(string invoiceId)
        {
            return await _collection.Find(i => i.InvoiceId == invoiceId).FirstOrDefaultAsync();
        }

        public async Task<InvoiceMVM?> GetUserInvoiceWithDetails(string invoiceId, string userId)
        {
            IEnumerable<Invoice> invoices = await _collection
                .Find(i => i.InvoiceId == invoiceId && i.UserId == userId)
                .ToListAsync();

            return invoices
                .SelectInvoice(_pksCollection, _pmCollection, _pCollection)
                .FirstOrDefault();
          
        }

        public async Task<InvoiceMVM?> GetInvoiceWithDetails(string invoiceId, string phone)
        {
            IEnumerable<Invoice> invoices = await _collection
                .Find(i => i.InvoiceId == invoiceId && i.RecipientPhone == phone)
                .ToListAsync();

            return invoices
                .SelectInvoice(_pksCollection, _pmCollection, _pCollection)
                .FirstOrDefault();
        }

        public async Task<InvoiceMVM?> GetInvoiceWithDetails(string invoiceId)
        {
            IEnumerable<Invoice> invoices = await _collection
                .Find(i => i.InvoiceId == invoiceId)
                .ToListAsync();

            return invoices
                .SelectInvoice(_pksCollection, _pmCollection, _pCollection)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<InvoiceMVM>> GetUserInvoices(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<InvoiceMVM>();
            }

            IEnumerable<Invoice> invoices = await _collection
                .Find(i => i.UserId == userId)
                .ToListAsync();

            return invoices
                .SelectInvoice(_pksCollection, _pmCollection, _pCollection)
                .ToList();
        }

        public async Task<bool> CheckUserIsPurchased(string userId, string productId)
        {
            Invoice? invoice = await _collection
                .Find(i => i.UserId == userId && i.IsCompleted && i.InvoiceDetails.Any(d => d.ProductId == productId))
                .FirstOrDefaultAsync();

            return invoice != null;
        }

        public async Task<bool> CheckIsPurchased(string? phone, string productId)
        {
            Invoice? invoice = await _collection
                .Find(i => i.InvoiceDetails.Any(d => d.ProductId == productId) 
                        && i.RecipientPhone == phone)
                .FirstOrDefaultAsync();

            return invoice != null;
        }

        public async Task<bool> AddInvoiceStatus(string invoiceId, InvoiceStatus invoiceStatus)
        {
            invoiceStatus.IsId = RandomUtils.RandomNumbers(1, 999999999);
            invoiceStatus.DateUpdated = DateTime.Now;
            UpdateDefinition<Invoice> update = Builders<Invoice>.Update.Push(i => i.InvoiceStatuses, invoiceStatus);

            UpdateResult result = await _collection.UpdateOneAsync(i => i.InvoiceId == invoiceId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdatePaymentStatus(string invoiceId, string status)
        {
            UpdateDefinition<Invoice> update = Builders<Invoice>.Update.Set(i => i.PaymentStatus, status);

            UpdateResult result = await _collection.UpdateOneAsync(i => i.InvoiceId == invoiceId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<(IEnumerable<InvoiceMVM>, int)> GetInvoices(int page, string? filterBy, string? sortBy)
        {
            IEnumerable<Invoice> invoices = await _collection
                .Find(_ => true)
                .ToListAsync();

            invoices = invoices.FilterBy(filterBy);

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(invoices.Count()) / Convert.ToDouble(NumOfInvoicePerPage)));

            return (invoices
                .Paginate(page, NumOfInvoicePerPage)
                .SortBy(sortBy)
                .SelectInvoice(_pksCollection, _pmCollection, _pCollection), 
                totalPage);
        }

        public async Task<IEnumerable<InvoiceMVM>> SearchInvoices(string query)
        {
            IEnumerable<Invoice> invoices = await _collection
                .Find(i => i.InvoiceId.Contains(query) || i.RecipientPhone.Contains(query))
                .ToListAsync();

            return invoices.SelectInvoice(_pksCollection, _pmCollection, _pCollection);
        }

        public async Task<bool> AcceptOrder(string invoiceId)
        {
            UpdateDefinition<Invoice> update = Builders<Invoice>.Update
                .Set(i => i.IsAccepted, true)
                .Set(i => i.AcceptedDate, DateTime.Now);

            UpdateResult result = await _collection.UpdateOneAsync(i => i.InvoiceId == invoiceId, update);

            return result.ModifiedCount > 0;
        }
        
        public async Task<bool> CancelOrder(string invoiceId)
        {
            UpdateDefinition<Invoice> update = Builders<Invoice>.Update
                .Set(i => i.IsCancelled, true)
                .Set(i => i.CancelledDate, DateTime.Now);

            UpdateResult result = await _collection.UpdateOneAsync(i => i.InvoiceId == invoiceId, update);

            return result.ModifiedCount > 0;
        }
    }
}
