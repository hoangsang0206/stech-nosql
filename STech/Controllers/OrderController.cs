using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Services;
using STech.Services.Constants;
using STech.Services.Services;
using STech.Services.Utils;
using STech.Utils;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace STech.Controllers
{
    public class OrderController : Controller
    {
        private readonly AddressService _addressService;
        private readonly IGeocodioService _geocodioService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IPaymentService _paymentService;
        private readonly IWarehouseService _warehouseService;

        private readonly HttpClient _httpClient;

        public OrderController(AddressService addressService, IGeocodioService geocodioService,
            IUserService userService, IProductService productService, IOrderService orderService,
            ICartService cartService, IPaymentService paymentService, IWarehouseService warehouseService,
            HttpClient httpClient)
        {
            _addressService = addressService;
            _geocodioService = geocodioService;
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _cartService = cartService;
            _paymentService = paymentService;
            _warehouseService = warehouseService;

            _httpClient = httpClient;
        }

        #region Functions

        private string GetDomain()
        {
            HttpRequest request = HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        private AddressVM GetAddress(string? wardCode, string? districtCode, string? cityCode)
        {
            AddressVM address = new AddressVM();
            address._City = _addressService.Address.Cities.FirstOrDefault(c => c.code == cityCode);
            address._District = address._City?.districts.FirstOrDefault(c => c.code == districtCode);
            address._Ward = address._District?.wards.FirstOrDefault(c => c.code == wardCode);

            return address;
        }

        private async Task<decimal> CalculateShippingFee(AddressVM address)
        {
            if(address._Ward == null || address._District == null || address._City == null)
            {
                return 0;
            }


            var (latitude, longtitude) = await _geocodioService
                .GetLocation(address._City.name_with_type, address._District.name_with_type, address._Ward.name_with_type);

            if (!latitude.HasValue || !longtitude.HasValue)
            {
                return 0;
            }

            Warehouse? warehouse = await _warehouseService.GetNearestWarehouse(latitude.Value, longtitude.Value);

            double distance = GeocodioUtils.CalculateDistance(latitude.Value, longtitude.Value, Convert.ToDouble(warehouse?.Latitude), Convert.ToDouble(warehouse?.Longtitude));
            double fee = GeocodioUtils.CalculateFee(distance);

            return Convert.ToDecimal(fee);
        }

        private Data.Models.Invoice CreateInvoice(OrderVM order, Data.Models.PaymentMethod paymentMethod, AddressVM address, string userId)
        {
            DateTime date = DateTime.Now;
            Data.Models.Invoice invoice = new Data.Models.Invoice();
            invoice.InvoiceId = date.ToString("yyyyMMdd") + Utils.RandomUtils.GenerateRandomString(8).ToUpper();
            invoice.OrderDate = date;
            invoice.PaymentMedId = paymentMethod.PaymentMedId;
            invoice.PaymentStatus = PaymentContants.UnPaid;
            invoice.DeliveryMedId = order.DeliveryMethod;
            invoice.UserId = userId;
            invoice.Note = order.Note;
            invoice.DeliveryAddress = $"{order.Address}, {address._Ward?.name_with_type}, {address._District?.name_with_type}, {address._City?.name_with_type}";
            invoice.RecipientName = order.RecipientName;
            invoice.RecipientPhone = order.RecipientPhone;
            invoice.IsCompleted = false;

            return invoice;
        }

        private async Task<PackingSlip> CreatePackingSlip(Data.Models.Invoice invoice, AddressVM address)
        {
            PackingSlip packingSlip = new PackingSlip();
            packingSlip.InvoiceId = invoice.InvoiceId;
            packingSlip.Psid = DateTime.Now.ToString("yyyyMMdd") + Utils.RandomUtils.GenerateRandomString(8).ToUpper();
            packingSlip.DeliveryFee = invoice.DeliveryMedId == DeliveryContants.COD ? await CalculateShippingFee(address) : 0;
            packingSlip.IsCompleted = false;

            return packingSlip;
        }

        private List<InvoiceStatus> CreateInvoiceStatus(Data.Models.Invoice invoice)
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

        private async Task<List<InvoiceDetail>> CreateInvoiceDetails(Data.Models.Invoice invoice, string? pId)
        {
            List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();

            if (pId != null)
            {
                ProductMVM? product = await _productService.GetProduct(pId);
                if (product != null)
                {
                    invoiceDetails.Add(new InvoiceDetail
                    {
                        ProductId = product.ProductId,
                        Quantity = 1,
                        Cost = product.Price
                    });
                }
            }
            else
            {
                if(invoice.UserId != null)
                {
                    IEnumerable<CartMVM> userCart = await _cartService.GetUserCart(invoice.UserId);
                    foreach (CartMVM cart in userCart)
                    {
                        invoiceDetails.Add(new InvoiceDetail
                        {
                            ProductId = cart.Product.ProductId,
                            Quantity = cart.Quantity,
                            Cost = cart.Product.Price
                        });
                    }
                }

            }

            return invoiceDetails;
        }

        private async Task<List<WarehouseExport>> CreateWarehouseExports(Data.Models.Invoice invoice, AddressVM address)
        {
            if (address._Ward == null || address._District == null || address._City == null)
            {
                return new List<WarehouseExport>();
            }

            var (latitude, longtitude) = await _geocodioService
            .GetLocation(address._City.name_with_type, address._District.name_with_type, address._Ward.name_with_type);

            List<WarehouseExport> whEs = new List<WarehouseExport>();
            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehousesOrderByDistanceWithProduct(latitude, longtitude);

            foreach (InvoiceDetail detail in invoice.InvoiceDetails)
            {
                List<WarehouseProduct> whProducts = new List<WarehouseProduct>();

                foreach (Warehouse wh in warehouses)
                {
                    whProducts.AddRange(await _productService.GetAllWarehouseProducts(wh.WarehouseId));
                }
                
                whProducts = whProducts
                    .Where(wp => wp.ProductId == detail.ProductId)
                    .ToList();

                int requestedQty = detail.Quantity;

                foreach (WarehouseProduct wp in whProducts)
                {
                    if (requestedQty <= 0)
                    {
                        break;
                    }

                    int qty = wp.Quantity;
                    if (qty <= 0)
                    {
                        continue;
                    }

                    int exportQty = Math.Min(requestedQty, qty);
                    requestedQty -= exportQty;

                    WarehouseExport? whE = whEs.FirstOrDefault(t => t.WarehouseId == wp.WarehouseId);
                    if(whE == null)
                    {
                        whE = new WarehouseExport();
                        whE.Weid = DateTime.Now.ToString("yyyyMMdd") + Utils.RandomUtils.GenerateRandomString(8).ToUpper();
                        whE.WarehouseId = wp.WarehouseId;
                        whE.InvoiceId = invoice.InvoiceId;
                        whE.DateCreate = DateTime.Now;
                        whE.ReasonExport = "Xuất hàng theo hóa đơn";

                        whEs.Add(whE);
                    }

                    whE.WarehouseExportDetails.Add(new WarehouseExportDetail
                    {
                        ProductId = wp.ProductId,
                        RequestedQuantity = exportQty,
                        UnitPrice = detail.Cost
                    });
                } 
            }

            return whEs;
        }

        private async Task<bool> UpdateWarehouseExports(IEnumerable<WarehouseExport> warehouseExports)
        {
            bool result = await _warehouseService.CreateWarehouseExports(warehouseExports);
            if(result)
            {
                await _productService.SubtractProductQuantity(warehouseExports);
            }

            return result;
        }

        private (Data.Models.Invoice?, List<WarehouseExport>, PackingSlip) GetInvoiceFromSession()
        {
            string invoiceJson = HttpContext.Session.GetString("Invoice") ?? "";
            string whExportsJson = HttpContext.Session.GetString("WarehouseExports") ?? "";
            string pksJson = HttpContext.Session.GetString("PackingSlip") ?? "";

            HttpContext.Session.Remove("Invoice");
            HttpContext.Session.Remove("WarehouseExports");
            HttpContext.Session.Remove("PackingSlip");

            Data.Models.Invoice? invoice = JsonSerializer.Deserialize<Data.Models.Invoice>(invoiceJson);
            PackingSlip pks = JsonSerializer.Deserialize<PackingSlip>(pksJson) ?? new PackingSlip();
            List<WarehouseExport> whEs = JsonSerializer.Deserialize<List<WarehouseExport>>(whExportsJson) ?? new List<WarehouseExport>();

            return (invoice, whEs, pks);
        }

        private async Task<bool> RemoveUserCart()
        {
            bool isOrderFromCart = HttpContext.Session.GetString("OrderFromCart") == "true";
            if (isOrderFromCart)
            {
                string? userId = User.FindFirstValue("Id");
                if (userId != null)
                {
                    bool result = await _cartService.RemoveUserCart(userId);
                    return result;
                }
            }

            return false;
        }

        #endregion


        #region Actions
        [Authorize]
        public async Task<IActionResult> CheckOut(string? pId = null, string? ward = null, string? district = null, string? city = null)
        {
            string? userId = User.FindFirstValue("Id");
            if(userId == null)
            {
                return BadRequest();
            }

            User? user = await _userService.GetUserById(userId);
            if(user == null)
            {
                return BadRequest();
            }

            IEnumerable<UserAddress> userAddresses = await _userService.GetUserAddress(userId);
            user.UserAddresses = userAddresses as List<UserAddress> ?? new List<UserAddress>();
            UserAddress? defaultAddress = userAddresses.FirstOrDefault(t => t.IsDefault == true);

            IEnumerable<CartMVM> cart = new List<CartMVM>();

            AddressVM address = new AddressVM();
            city ??= defaultAddress?.ProvinceCode;
            district ??= defaultAddress?.DistrictCode;
            ward ??= defaultAddress?.WardCode;

            address = GetAddress(ward, district, city);

            ProductMVM? product = null; 

            if (pId == null)
            {
                cart = await _cartService.GetUserCart(userId);

                List<CartMVM> cartToDelete = new List<CartMVM>();
                foreach (CartMVM c in cart)
                {
                    int qty = await _productService.GetTotalQty(c.ProductId);
                    if (qty <= 0)
                    {
                        cartToDelete.Add(c);
                    }
                    else if (c.Quantity > qty)
                    {
                        await _cartService.UpdateQuantity(new UserCart
                        {
                            CId = c.Id,
                            ProductId = c.ProductId,
                            Quantity = qty
                        }, qty);
                    }
                }

                if (cartToDelete.Count > 0)
                {
                    await _cartService.RemoveListCart(cartToDelete);
                }

                cart = await _cartService.GetUserCart(userId);

                if(cart.Count() <= 0)
                {
                    return LocalRedirect("/");
                }
            }
            else
            {
                product = await _productService.GetProductWithBasicInfo(pId);

                if(product == null)
                {
                    return LocalRedirect("/");
                }

                if (await _productService.CheckOutOfStock(pId))
                {
                    return LocalRedirect("/");
                }
            }

            return View(new Tuple<User, IEnumerable<CartMVM>, AddressVM, ProductMVM?>(user, cart, address, product));
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderVM order)
        {
            if(ModelState.IsValid)
            {
                string? userId = User.FindFirstValue("Id");
                if(userId == null)
                {
                    return BadRequest();
                }

                User? user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return BadRequest();
                }

                Data.Models.PaymentMethod? paymentMethod = await _paymentService.GetPaymentMethod(order.PaymentMethod);
                if(paymentMethod == null)
                {
                    return BadRequest();
                }

                AddressVM address = GetAddress(order.WardCode, order.DistrictCode, order.CityCode);

                Data.Models.Invoice invoice = CreateInvoice(order, paymentMethod, address, userId);
                PackingSlip pks = await CreatePackingSlip(invoice, address);
                invoice.InvoiceStatuses = CreateInvoiceStatus(invoice);
                invoice.InvoiceDetails = await CreateInvoiceDetails(invoice, order.pId);

                invoice.SubTotal = invoice.InvoiceDetails.Sum(t => t.Cost * t.Quantity);
                invoice.Total = invoice.SubTotal + pks.DeliveryFee;

                List<WarehouseExport> whEs = await CreateWarehouseExports(invoice, address);

                string str = JsonSerializer.Serialize(invoice);

                HttpContext.Session.SetString("Invoice", JsonSerializer.Serialize(invoice));
                HttpContext.Session.SetString("WarehouseExports", JsonSerializer.Serialize(whEs));
                HttpContext.Session.SetString("PackingSlip", JsonSerializer.Serialize(pks));
                HttpContext.Session.SetString("OrderFromCart", order.pId == null ? "true" : "false");

                switch (order.PaymentMethod)
                {
                    case PaymentContants.CashPayment:
                        return RedirectToAction("PaymentWithCash");

                    case PaymentContants.CardPayment:
                        return RedirectToAction("PaymentWithStripe");
                            
                    case PaymentContants.PaypalPayment:
                        return RedirectToAction("PaymentWithPaypal");

                    default:
                        return BadRequest();
                }
            }

            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> PaymentWithCash()
        {
            var (invoice, whEs, pks) = GetInvoiceFromSession();

            if(invoice == null)
            {
                return NotFound();
            }

            bool result = await _orderService.CreateInvoice(invoice, pks, whEs);
            if (result)
            {
                await RemoveUserCart();
                await UpdateWarehouseExports(whEs);
            }


            HttpContext.Session.SetString("PaymentStatus", JsonSerializer.Serialize(new PaymentStatusVM
            {
                InvoiceId = invoice.InvoiceId,
                IsPaid = result,
                TotalAmount = invoice.Total,
                PaymentDate = DateTime.Now
            }));

            return RedirectToAction("PaymentResult");
        }

        [Authorize]
        public async Task<IActionResult> PaymentWithStripe()
        {
            try
            {
                var (invoice, whEs, pks) = GetInvoiceFromSession();

                if (invoice == null)
                {
                    return NotFound();
                }

                List<SessionLineItemOptions> items = new List<SessionLineItemOptions>();
                foreach (InvoiceDetail detail in invoice.InvoiceDetails)
                {
                    ProductMVM? product = await _productService.GetProductWithBasicInfo(detail.ProductId);

                    items.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)Math.Round(detail.Cost / PaymentContants.USD_EXCHANGE_RATE) * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = product?.ProductName,
                                Images = new List<string> { product?.ProductImages.FirstOrDefault()?.ImageSrc ?? "" }
                            }
                        },
                        Quantity = detail.Quantity
                    });
                }

                if (pks != null)
                {
                    items.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)Math.Round(pks.DeliveryFee / PaymentContants.USD_EXCHANGE_RATE) * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Delivery Fee"
                            }
                        },
                        Quantity = 1
                    });
                }

                SessionCreateOptions options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = items,
                    Metadata = new Dictionary<string, string>
                    {
                        { "invoice_id", invoice.InvoiceId }
                    },
                    Mode = "payment",
                    SuccessUrl = $"{GetDomain()}/order/stripecallback",
                    CancelUrl = $"{GetDomain()}/order/paymentfailed"
                };

                SessionService service = new SessionService();
                Session session = service.Create(options);
                TempData["Invoice"] = session.Id;

                if (await _orderService.CreateInvoice(invoice, pks, whEs))
                {
                    await RemoveUserCart();
                    await UpdateWarehouseExports(whEs);
                    return Redirect(session.Url);
                }

                return NotFound();
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        public async Task<IActionResult> StripeCallback()
        {
            SessionService service = new SessionService();
            Session session = service.Get(TempData["Invoice"]?.ToString());
            string invoiceId = session.Metadata["invoice_id"];
            Data.Models.Invoice? invoice = await _orderService.GetInvoice(invoiceId);

            string? paymentStatus = "";

            if(invoice == null)
            {
                return BadRequest();
            }
            
            if(session.PaymentStatus == "paid")
            {
                paymentStatus = PaymentContants.Paid;
            }
            else
            {
                paymentStatus = PaymentContants.PaymentFailed;
            }

            HttpContext.Session.SetString("PaymentStatus", JsonSerializer.Serialize(new PaymentStatusVM
            {
                InvoiceId = invoice.InvoiceId,
                IsPaid = paymentStatus == PaymentContants.Paid,
                TotalAmount = invoice.Total,
                PaymentDate = DateTime.Now
            }));

            await _orderService.UpdatePaymentStatus(invoice.InvoiceId, paymentStatus);
            return RedirectToAction("PaymentResult");
        }

        [Authorize]
        public async Task<IActionResult> PaymentWithPaypal(Data.Models.Invoice invoice)
        {
            try
            {

                return Ok();

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }   
        }

        [Authorize]
        public IActionResult PaymentResult()
        {
            string? json = HttpContext.Session.GetString("PaymentStatus");
            HttpContext.Session.Remove("PaymentStatus");

            if (string.IsNullOrEmpty(json))
            {
                return NotFound();
            }

            PaymentStatusVM paymentStatus = JsonSerializer.Deserialize<PaymentStatusVM>(json) ?? new PaymentStatusVM();

            return View(paymentStatus);
        }

        public async Task<IActionResult> CheckOrder(string? oId, string? phone)
        {
            IEnumerable<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Tra cứu đơn hàng", "")
            };

            if (string.IsNullOrEmpty(oId) || string.IsNullOrEmpty(phone))
            {
                return View(new Tuple<InvoiceMVM?, IEnumerable<Breadcrumb>>(null, breadcrumbs));
            }

            InvoiceMVM? invoice = await _orderService.GetInvoiceWithDetails(oId, phone);

            ViewBag.SearchOId = oId;
            ViewBag.SearchPhone = phone;

            return View(new Tuple<InvoiceMVM?, IEnumerable<Breadcrumb>>(invoice, breadcrumbs));
        }

        [Route("order/detail/{id}"), Authorize]
        public async Task<IActionResult> OrderDetail(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            string? userId = User.FindFirstValue("Id");
            if(userId == null)
            {
                return Unauthorized();
            }

            InvoiceMVM? invoice = await _orderService.GetUserInvoiceWithDetails(id, userId);
            if (invoice == null)
            {
                return NotFound();
            }

            IEnumerable<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Đơn hàng", "/account#orders"),
                new Breadcrumb(invoice.InvoiceId, "")
            };

            return View(new Tuple<InvoiceMVM, IEnumerable<Breadcrumb>>(invoice, breadcrumbs));
        }

        #endregion
    }
}
