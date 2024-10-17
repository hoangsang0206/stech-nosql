using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface IDeliveryService
    {
        Task<DeliveryMethod?> GetDeliveryMethodById(string id);
    }
}
