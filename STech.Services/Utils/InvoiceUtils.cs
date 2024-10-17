using MongoDB.Driver;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Services.Constants;

namespace STech.Services.Utils
{
    public static class InvoiceUtils
    {
        public static IEnumerable<InvoiceMVM> SelectInvoice(this IEnumerable<Invoice> invoices, IMongoCollection<PackingSlip> pksCollection, IMongoCollection<PaymentMethod> pmCollection, IMongoCollection<Product> pCollection)
        {
            return invoices.Select(i => new InvoiceMVM
            {
                InvoiceId = i.InvoiceId,
                OrderDate = i.OrderDate,
                PaymentStatus = i.PaymentStatus,
                SubTotal = i.SubTotal,
                Total = i.Total,
                IsAccepted = i.IsAccepted,
                IsCancelled = i.IsCancelled,
                IsCompleted = i.IsCompleted,
                AcceptedDate = i.AcceptedDate,
                CancelledDate = i.CancelledDate,
                CompletedDate = i.CompletedDate,
                CustomerId = i.CustomerId,
                UserId = i.UserId,
                DeliveryAddress = i.DeliveryAddress,
                DeliveryMedId = i.DeliveryMedId,
                EmployeeId = i.EmployeeId,
                Note = i.Note,
                PaymentMedId = i.PaymentMedId,
                RecipientName = i.RecipientName,
                RecipientPhone = i.RecipientPhone,
                InvoiceStatuses = i.InvoiceStatuses,
                InvoiceDetails = i.InvoiceDetails.SelectDetail(pCollection).ToList(),
                PackingSlip = pksCollection.Find(p => p.InvoiceId == i.InvoiceId).FirstOrDefault(),
                PaymentMethod = pmCollection.Find(pm => pm.PaymentMedId == i.PaymentMedId).FirstOrDefaultAsync().Result,
            });
        }

        public static IEnumerable<InvoiceDetailMVM> SelectDetail(this IEnumerable<InvoiceDetail> details, IMongoCollection<Product> pCollection)
        {
            return details.Select(d => new InvoiceDetailMVM
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                Cost = d.Cost,
                Product = pCollection.Find(p => p.ProductId == d.ProductId).FirstOrDefaultAsync().Result,
            });
        }

        public static IEnumerable<Invoice> Paginate(this IEnumerable<Invoice> invoices, int page, int numToTake)
        {
            if(page <= 0)
            {
                page = 1;
            }

            int numToSkip = (page - 1) * numToTake;

            return invoices.Skip(numToSkip).Take(numToTake).ToList(); ;
        }

        public static IEnumerable<Invoice> FilterBy(this IEnumerable<Invoice> invoices, string? filterBy)
        {
            if (filterBy == null)
            {
                return invoices;
            }

            switch (filterBy)
            {
                case "paid":
                    return invoices.Where(i => i.PaymentStatus == PaymentContants.Paid)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "unpaid":
                    return invoices.Where(i => i.PaymentStatus == PaymentContants.UnPaid)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "accepted":
                    return invoices.Where(i => i.IsAccepted && !i.IsCancelled && !i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "unaccepted":
                    return invoices
                        .Where(i => !i.IsAccepted && !i.IsCancelled && !i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "completed":
                    return invoices.Where(i => i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "cancelled":
                    return invoices.Where(i => i.IsCancelled)
                        .OrderByDescending(i => i.OrderDate).ToList();

                default:
                    return invoices;
            }
        }

        public static IEnumerable<Invoice> SortBy(this IEnumerable<Invoice> invoices, string? sortBy)
        {
            if (sortBy == null)
            {
                return invoices;
            }

            switch (sortBy)
            {
                case "date-asc":
                    return invoices.OrderBy(i => i.OrderDate).ToList();
                case "date-desc":
                    return invoices.OrderByDescending(i => i.OrderDate).ToList();
                case "price-asc":
                    return invoices.OrderBy(i => i.Total).ToList();
                case "price-desc":
                    return invoices.OrderByDescending(i => i.Total).ToList();
                default:
                    return invoices;
            }
        }
    }
}
