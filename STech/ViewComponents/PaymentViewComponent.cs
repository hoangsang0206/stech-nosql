using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Services;

namespace STech.ViewComponents
{
    public class PaymentViewComponent : ViewComponent
    {
        private readonly IPaymentService _paymentService;

        public PaymentViewComponent(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<PaymentMethod> paymentMethods = await _paymentService.GetPaymentMethods();
            return View(paymentMethods.Where(p => p.IsActive == true).ToList());
        }
    }
}
