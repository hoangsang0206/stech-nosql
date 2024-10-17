using Microsoft.AspNetCore.Mvc;
using STech.Services.Services;

namespace STech.ViewComponents
{
    public class SelectOptionsViewComponent : ViewComponent
    {
        private readonly AddressService _addressService;

        public SelectOptionsViewComponent(AddressService addressService)
        {
            _addressService = addressService;
        }

        public IViewComponentResult Invoke()
        {
            return View("Cities", _addressService.Address.Cities);
        }
    }
}
