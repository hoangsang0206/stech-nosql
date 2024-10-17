using System.Globalization;

namespace STech.Utils
{
    public static class CurrencyFormatter
    {
        private static readonly string CURRENCY_UNIT = "đ";
        private static readonly string CULTURE_INFO = "vi-VN";

        public static string Format(decimal? amount)
        {
            if (amount == null)
            {
                return "";
            }

            if(amount.Value == 0)
            {
                return "0" + CURRENCY_UNIT;
            }

            CultureInfo cultureInfo = new CultureInfo(CULTURE_INFO);
            return amount.Value.ToString("##,###", cultureInfo) + CURRENCY_UNIT;
        }
    }
}
