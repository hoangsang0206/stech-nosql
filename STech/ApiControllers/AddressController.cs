using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using STech.Data.ViewModels;
using STech.Services.Services;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("cities")]
        public IActionResult GetCities()
        {
            IEnumerable<AddressVM.City> cities = _addressService.Address.Cities;
            return Ok(cities);
        }

        [HttpGet("districts/{cityCode}")]
        public IActionResult GetDistricts(string cityCode)
        {
            AddressVM.City? city = _addressService.Address.Cities.FirstOrDefault(c => c.code == cityCode);
            if(city == null)
            {
                return BadRequest();
            }

            return Ok(city.districts);
        }

        [HttpGet("wards/{districtCode}")]
        public IActionResult GetWards(string districtCode)
        {
            AddressVM.District? district = _addressService.Address.Districts.FirstOrDefault(d => d.code == districtCode);
            if(district == null)
            {
                return BadRequest();
            }   

            return Ok(district.wards);
        }

        [HttpGet("address/{wardCode}/{districtCode}/{cityCode}")]
        public IActionResult GetAddress(string wardCode, string districtCode, string cityCode)
        {
            if(cityCode == null || districtCode == null || wardCode == null)
            {
                return BadRequest();
            }

            AddressVM.City? city = _addressService.Address.Cities.FirstOrDefault(c => c.code == cityCode);
            AddressVM.District? district = city?.districts.FirstOrDefault(d => d.code == districtCode);
            AddressVM.Ward? ward = district?.wards.FirstOrDefault(w => w.code == wardCode);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    City = city,
                    District = district,
                    Ward = ward
                }
            });
        }
    }
}
