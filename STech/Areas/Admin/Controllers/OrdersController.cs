using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin"), AdminAuthorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(int page = 1, string? filter_by = "unaccepted", string? sort_by = null)
        {
            var (invoices, totalPage ) = await _orderService.GetInvoices(page, filter_by, sort_by);

            ViewBag.ActivePageNav = filter_by;
            ViewBag.ActiveSidebar = "orders";

            ViewBag.TotalPages = totalPage;
            ViewBag.CurrentPage = page;

            return View(invoices);
        }

        [Route("admin/orders/search/{query}")]
        public async Task<IActionResult> SearchOrders(string query)
        {
            IEnumerable<InvoiceMVM> invoices = await _orderService.SearchInvoices(query);

            ViewBag.ActiveSidebar = "orders";
            ViewBag.SearchValue = query;
            ViewBag.TotalPages = 1;
            ViewBag.CurrentPage = 1;

            return View("Index", invoices);
        }

        public IActionResult Create()
        {
            ViewBag.ActiveSidebar = "orders";
            return View();
        }
    }
}
