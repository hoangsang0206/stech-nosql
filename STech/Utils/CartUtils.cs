using Newtonsoft.Json;
using STech.Data.ViewModels;
using System.Text;

namespace STech.Utils
{
    public static class CartUtils
    {
        private static readonly string CART_KEY = "CART";
        private static readonly int COOKIE_EXPIRE = 60;

        public static void SaveCartToCookie(HttpResponse response, IEnumerable<CartVM> cart)
        {
            if (cart.Count() <= 0)
            {
                DeleteCookieCart(response);
                return;
            }

            string cartJson = JsonConvert.SerializeObject(cart);
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(cartJson);
            string base64String = Convert.ToBase64String(bytesToEncode);

            response.Cookies.Append(CART_KEY, base64String, new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddDays(COOKIE_EXPIRE)
            });
        }

        public static List<CartVM> GetCartFromCookie(HttpRequest request)
        {
            if (request.Cookies.TryGetValue(CART_KEY, out var base64String))
            {
                if (!string.IsNullOrEmpty(base64String))
                {
                    byte[] bytesToDecode = Convert.FromBase64String(base64String);
                    string cartJson = Encoding.UTF8.GetString(bytesToDecode);
                    return JsonConvert.DeserializeObject<List<CartVM>>(cartJson) ?? new List<CartVM>();
                }
            }

            return new List<CartVM>();
        }

        public static void DeleteCookieCart(HttpResponse response)
        {
            response.Cookies.Delete(CART_KEY);
        }

        public static int UpdateCartItemQuantity(string? type, int oldQty, int newQty)
        {
            switch (type)
            {
                case "plus":
                    oldQty += 1;
                    break;
                case "minus":
                    oldQty -= 1;
                    break;
                default:
                    oldQty = newQty;
                    break;
            }

            if (oldQty <= 0)
            {
                oldQty = 1;
            }

            return oldQty;
        }
    }
}
