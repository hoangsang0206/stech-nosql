using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Services;
using STech.Services.Constants;
using STech.Utils;
using System.Security.Claims;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IWebHostEnvironment _env;

        public OrdersController(IOrderService orderService, IWebHostEnvironment env)
        {
            _orderService = orderService;
            _env = env;
        }

        [HttpGet("userorders"), Authorize]
        public async Task<IActionResult> GetOrders([FromQuery] string? type)
        {
            string? userId = User.FindFirstValue("Id");

            if (userId == null)
            {
                return Unauthorized();
            }

            IEnumerable<InvoiceMVM> invoices = await _orderService.GetUserInvoices(userId);

            switch(type)
            {
                case "completed":
                    invoices = invoices.Where(i => i.IsCompleted == true);
                    break;
                case "uncompleted":
                    invoices = invoices.Where(i => i.IsCompleted == false && i.IsCancelled == false);
                    break;
                case "cancelled":
                    invoices = invoices.Where(i => i.IsCancelled == true);
                    break;
                case "paid":
                    invoices = invoices.Where(i => i.PaymentStatus == PaymentContants.Paid);
                    break;
                case "unpaid":
                    invoices = invoices.Where(i => i.PaymentStatus == PaymentContants.UnPaid);
                    break;
                default:
                    break;
            }

            return Ok(new ApiResponse
            {
                Status = true,
                Data = invoices
            });
        }

        [HttpGet("one")]
        public async Task<IActionResult> CheckOrder([FromQuery] string oId, string phone)
        {
            InvoiceMVM? invoice = await _orderService.GetInvoiceWithDetails(oId, phone);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(new ApiResponse
            {
                Status = true,
                Data = invoice
            });
        }

        [HttpGet("download-invoice")]
        public async Task<IActionResult> DownloadInvoice([FromQuery] string oId, string? phone)
        {
            InvoiceMVM? invoice;
            byte[] file;

            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue("Id");

                if (userId == null)
                {
                    return Unauthorized();
                }

                invoice = await _orderService.GetUserInvoiceWithDetails(oId, userId);
            }
            else
            {
                if(string.IsNullOrEmpty(phone))
                {
                    return BadRequest();
                }

                invoice = await _orderService.GetInvoiceWithDetails(oId, phone);
            }

            if (invoice == null)
            {
                return NotFound();
            }

            file = PrintInvoiceUtils.CreatePdf(_env, invoice);

            return File(file, "application/pdf", $"HĐ_{invoice.InvoiceId}.pdf");
        }
    }
}
