using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using STech.Services.Constants;
using STech.Services.Services;
using STech.Services.Utils;
using STech.Utils;
using System.Security.Claims;

namespace STech.Areas.Admin.ApiControllers
{
    [AdminAuthorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly AddressService _addressService;
        private readonly IGeocodioService _geocodioService;
        private readonly IWarehouseService _warehouseService;
        private readonly IPaymentService _paymentService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;

        private readonly IWebHostEnvironment _env;

        public OrdersController(IWebHostEnvironment env, IOrderService orderService, 
            IProductService productService, AddressService addressService,
            IGeocodioService geocodioService, IWarehouseService warehouseService,
            IPaymentService paymentService, ICustomerService customerService,
            IEmployeeService employeeService)
        {
            _env = env;
            _orderService = orderService;
            _productService = productService;
            _addressService = addressService;
            _geocodioService = geocodioService;
            _warehouseService = warehouseService;
            _paymentService = paymentService;
            _customerService = customerService;
            _employeeService = employeeService;
        }

        #region Functions

        private AddressVM GetAddress(string? wardCode, string? districtCode, string? cityCode)
        {
            AddressVM address = new AddressVM();
            address._City = _addressService.Address.Cities.FirstOrDefault(c => c.code == cityCode);
            address._District = address._City?.districts.FirstOrDefault(c => c.code == districtCode);
            address._Ward = address._District?.wards.FirstOrDefault(c => c.code == wardCode);

            return address;
        }

        private async Task<decimal> CalculateShippingFee(AddressVM address, string warehouseId)
        {
            if (address._Ward == null || address._District == null || address._City == null)
            {
                return 0;
            }


            var (latitude, longtitude) = await _geocodioService
                .GetLocation(address._City.name_with_type, address._District.name_with_type, address._Ward.name_with_type);

            if (!latitude.HasValue || !longtitude.HasValue)
            {
                return 0;
            }

            Warehouse? warehouse = await _warehouseService.GetWarehouseById(warehouseId);

            double distance = GeocodioUtils.CalculateDistance(latitude.Value, longtitude.Value, Convert.ToDouble(warehouse?.Latitude), Convert.ToDouble(warehouse?.Longtitude));
            double fee = GeocodioUtils.CalculateFee(distance);

            return Convert.ToDecimal(fee);
        }

        private Invoice CreateInvoice(AdminOrderVM order, PaymentMethod paymentMethod, AddressVM address, Customer customer, string? employeeId)
        {
            string address_str = $"{order.Address}, {address._Ward?.name_with_type}, {address._District?.name_with_type}, {address._City?.name_with_type}";
            if(string.IsNullOrEmpty(order.Address) || string.IsNullOrEmpty(order.WardCode) 
                || string.IsNullOrEmpty(order.DistrictCode) || string.IsNullOrEmpty(order.CityCode))
            {
                address_str = $"{customer.Address}, {customer.Ward}, {customer.District}, {customer.Province}";
            }

            DateTime date = DateTime.Now;
            Invoice invoice = new Invoice();
            invoice.InvoiceId = date.ToString("yyyyMMdd") + STech.Utils.RandomUtils.GenerateRandomString(8).ToUpper();
            invoice.OrderDate = date;
            invoice.PaymentMedId = paymentMethod.PaymentMedId;
            invoice.PaymentStatus = PaymentContants.UnPaid;
            invoice.DeliveryMedId = order.DeliveryMethod;
            invoice.CustomerId = customer.CustomerId;
            invoice.EmployeeId = employeeId;
            invoice.Note = order.Note;
            invoice.DeliveryAddress = address_str;
            invoice.RecipientName = order.RecipientName ?? customer.CustomerName;
            invoice.RecipientPhone = order.RecipientPhone ?? customer.Phone;

            return invoice;
        }

        private async Task<PackingSlip> CreatePackingSlip(Invoice invoice, AddressVM address, string warehouseId)
        {
            PackingSlip packingSlip = new PackingSlip();
            packingSlip.InvoiceId = invoice.InvoiceId;
            packingSlip.Psid = DateTime.Now.ToString("yyyyMMdd") + STech.Utils.RandomUtils.GenerateRandomString(8).ToUpper();
            packingSlip.DeliveryFee = invoice.DeliveryMedId == DeliveryContants.COD ? await CalculateShippingFee(address, warehouseId) : 0;
            packingSlip.IsCompleted = false;

            return packingSlip;
        }

        private List<InvoiceStatus> CreateInvoiceStatus(Invoice invoice)
        {
            List<InvoiceStatus> invoiceStatuses = new List<InvoiceStatus>
            {
                new InvoiceStatus
                {
                    Status = "Chờ xác nhận",
                    DateUpdated = invoice.OrderDate
                }
            };

            return invoiceStatuses;
        }

        private async Task<List<InvoiceDetail>> CreateInvoiceDetails(List<AdminOrderVM.Product> products, string invoiceId)
        {
            List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();

            foreach (AdminOrderVM.Product p in products)
            {
                ProductMVM? product = await _productService.GetProduct(p.ProductId);
                if (product != null)
                {
                    invoiceDetails.Add(new InvoiceDetail
                    {
                        ProductId = product.ProductId,
                        Quantity = p.Quantity,
                        Cost = p.SalePrice ?? product.Price
                    });
                }
            }

            return invoiceDetails;
        }

        private async Task<WarehouseExport> CreateWarehouseExport(Invoice invoice, string warehouseId)
        {
            WarehouseExport whE = new WarehouseExport();
            Warehouse? warehouse = await _warehouseService.GetWarehouseById(warehouseId);

            if (warehouse == null)
            {
                return whE;
            }

            whE = new WarehouseExport();
            whE.Weid = DateTime.Now.ToString("yyyyMMdd") + STech.Utils.RandomUtils.GenerateRandomString(8).ToUpper();
            whE.WarehouseId = warehouse.WarehouseId;
            whE.InvoiceId = invoice.InvoiceId;
            whE.DateCreate = DateTime.Now;
            whE.ReasonExport = "Xuất hàng theo hóa đơn";


            foreach (InvoiceDetail detail in invoice.InvoiceDetails)
            {
                ProductMVM? product = await _productService.GetProductWithBasicInfo(detail.ProductId);
                WarehouseProduct? wp = product?.WarehouseProducts?.FirstOrDefault(w => w.WarehouseId == warehouse.WarehouseId);

                if (wp == null)
                {
                    continue;
                }

                whE.WarehouseExportDetails.Add(new WarehouseExportDetail
                {
                    ProductId = wp.ProductId,
                    RequestedQuantity = detail.Quantity,
                    UnitPrice = detail.Cost
                });
            }

            return whE;
        }

        private async Task<bool> CheckInStock(AdminOrderVM order)
        {
            foreach (AdminOrderVM.Product product in order.Products)
            {
                ProductMVM? p = await _productService.GetProductWithBasicInfo(product.ProductId);

                if (p == null)
                {
                    return false;
                }

                WarehouseProduct? wp = p.WarehouseProducts.FirstOrDefault(w => w.WarehouseId == order.WarehouseId);

                if (wp == null || wp.Quantity < product.Quantity)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> GetOrders(int page = 1, string? filter_by = "unaccepted", string? sort_by = null)
        {
            var (invoices, totalPages) = await _orderService.GetInvoices(page, filter_by, sort_by);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    invoices,
                    currentPage = page,
                    totalPages
                }
            });
        }

        [HttpGet("search/{query}")]
        public async Task<IActionResult> SearchOrders(string query)
        {
            IEnumerable<InvoiceMVM> invoices = await _orderService.SearchInvoices(query);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = invoices
            });
        }

        [HttpGet("one/{oId}")]
        public async Task<IActionResult> GetOrder(string oId)
        {
            Invoice? invoice = await _orderService.GetInvoice(oId);

            if (invoice == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            return Ok(new ApiResponse
            {
                Status = true,
                Data = invoice
            });
        }

        [HttpPatch("accept/{oId}")]
        public async Task<IActionResult> AcceptOrder(string oId)
        {
            Invoice? invoice = await _orderService.GetInvoice(oId);

            if (invoice == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            if(invoice.IsAccepted)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Đơn hàng này đã được xác nhận"
                });
            }

            if (invoice.IsCancelled)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Đơn hàng này đã bị hủy"
                });
            }

            bool result = await _orderService.AcceptOrder(invoice.InvoiceId);
            await _orderService.AddInvoiceStatus(invoice.InvoiceId, new InvoiceStatus
            {
                Status = "Đã xác nhận",
                DateUpdated = DateTime.Now
            });

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "" : "Đã xảy ra lỗi"
            });
        }

        [HttpPatch("cancel/{oId}")]
        public async Task<IActionResult> CancelOrder(string oId)
        {
            Invoice? invoice = await _orderService.GetInvoice(oId);

            if (invoice == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            if(invoice.IsCompleted)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Đơn hàng này đã được giao, không thể hủy"
                });
            }

            invoice.IsCancelled = true;
            invoice.CancelledDate = DateTime.Now;

            bool result = await _orderService.CancelOrder(invoice.InvoiceId);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "" : "Đã xảy ra lỗi"
            });
        }

        [HttpGet("print-invoice/{oId}")]
        public async Task<IActionResult> PrintInvoice(string oId)
        {
            InvoiceMVM? invoice = await _orderService.GetInvoiceWithDetails(oId);

            if (invoice == null)
            {
                return NotFound("Không tìm thấy hóa đơn");
            }

            return File(PrintInvoiceUtils.CreatePdf(_env, invoice), "application/pdf", $"HĐ_{oId}.pdf");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] AdminOrderVM order)
        {
            if (ModelState.IsValid)
            {
                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return BadRequest();
                } 

                if(!await CheckInStock(order))
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Có sản phẩm đã hết hàng, vui lòng kiểm tra lại"
                    });
                }

                Employee? employee = await _employeeService.GetEmployeeByUserId(userId);
                Customer? customer = await _customerService.GetCustomerById(order.CustomerId);
                PaymentMethod? paymentMethod = await _paymentService.GetPaymentMethod(PaymentContants.CashPayment);

                if (customer == null || paymentMethod == null)
                {
                    return BadRequest();
                }

                if(order.WardCode == null || order.DistrictCode == null || order.CityCode == null)
                {
                    order.WardCode = customer.WardCode;
                    order.DistrictCode = customer.DistrictCode;
                    order.CityCode = customer.ProvinceCode;
                }

                AddressVM address = GetAddress(order.WardCode, order.DistrictCode, order.CityCode);

                Invoice invoice = CreateInvoice(order, paymentMethod, address, customer, employee?.EmployeeId); 
                invoice.InvoiceDetails = await CreateInvoiceDetails(order.Products, invoice.InvoiceId);
                invoice.InvoiceStatuses = CreateInvoiceStatus(invoice);
                PackingSlip pks = await CreatePackingSlip(invoice, address, order.WarehouseId);
                WarehouseExport whE = await CreateWarehouseExport(invoice, order.WarehouseId);

                invoice.SubTotal = invoice.InvoiceDetails.Sum(t => t.Cost * t.Quantity);
                invoice.Total = invoice.SubTotal + pks.DeliveryFee;

                IEnumerable<WarehouseExport> warehouseExports = new List<WarehouseExport> { whE };

                bool result = await _orderService.CreateInvoice(invoice, pks, warehouseExports);

                if(result)
                {
                    if (await _warehouseService.CreateWarehouseExports(warehouseExports))
                    {
                        await _productService.SubtractProductQuantity(warehouseExports);
                    }
                }

                return Ok(new ApiResponse 
                {
                    Status = result,
                    Message = result ? "" : "Đã xảy ra lỗi"
                });
            }

            return Ok(new ApiResponse
            {
                Status = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }
    }
}
