using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class GeocodioUtils
    {
        public static readonly double EarthRadius = 6371.0; //km

        public static double DegreeToRadius(double degree)
        {
            return degree * Math.PI / 180;
        }

        public static double CalculateDistance(double lat1, double long1, double lat2, double long2)
        {
            double _lat = DegreeToRadius(lat2 - lat1);
            double _long = DegreeToRadius(long2 - long1);
            double a =
              Math.Sin(_lat / 2) * Math.Sin(_lat / 2) +
              Math.Cos(DegreeToRadius(lat1)) * Math.Cos(DegreeToRadius(lat2)) *
              Math.Sin(_long / 2) * Math.Sin(_long / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadius * c;
        }

        public static double CalculateFee(double distance)
        {
            double fee = distance switch
            {
                <= 5 => 15000,
                <= 15 => 20000,
                <= 30 => 25000,
                <= 50 => 35000,
                <= 100 => 40000,
                _ => 50000,
            };


            return Math.Floor(fee);
        }

        public static IEnumerable<Warehouse> OrderByDistance(this IEnumerable<Warehouse> warehouses, double latitude, double longtitude)
        {
            return warehouses.OrderBy(wh =>
            {
                double distance = CalculateDistance(latitude, longtitude, Convert.ToDouble(wh.Latitude), Convert.ToDouble(wh.Longtitude));
                return distance;
            }).ToList();
        }
    }
}
