using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Constants
{
    public class PaymentContants
    {
        public const decimal USD_EXCHANGE_RATE = 25000;

        public const string UnPaid = "unpaid";
        public const string Paid = "paid";
        public const string PaymentFailed = "payment-failed";

        public const string CashPayment = "cash";
        public const string CardPayment = "card";
        public const string PaypalPayment = "paypal";
    }
}
