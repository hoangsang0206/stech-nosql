using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface IGeocodioService
    {
        Task<(double? Latitude, double? Longtitude)> GetLocation(string city, string district, string ward);
        Task<(double? Latitude, double? Longtitude)> GetLocation(string address);
    }
}
