using STech.Data.ViewModels;
using System.Text.Json;

namespace STech.Services.Services
{
    public class AddressService
    {
        private readonly string ROOT_PATH;
        private readonly string CITIES_FILE_PATH = "cities.json";
        private readonly string DISTRICTS_FILE_PATH = "districts.json";
        private readonly string WARDS_FILE_PATH = "wards.json";

        public AddressVM Address = new AddressVM();

        public AddressService(string rootPath)
        {
            ROOT_PATH = rootPath;
            LoadCities().Wait();
            LoadDistricts().Wait();
            LoadWards().Wait();

            Address.Cities.ForEach(city =>
            {
                city.districts = Address.Districts.Where(d => d.parent_code == city.code).OrderBy(d => d.slug).ToList();
            });

            Address.Districts.ForEach(district =>
            {
                district.wards = Address.Wards.Where(w => w.parent_code == district.code).OrderBy(w => w.slug).ToList();
            });
        }

        private async Task<List<T>?> ReadJson<T>(string relativePath)
        {
            string filePath = Path.Combine(ROOT_PATH, relativePath);
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = await File.ReadAllTextAsync(filePath);
                    return JsonSerializer.Deserialize<List<T>>(jsonContent);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        private async Task LoadCities()
        {
            Address.Cities = await ReadJson<AddressVM.City>(CITIES_FILE_PATH) ?? new List<AddressVM.City>();
            Address.Cities = Address.Cities.OrderBy(c => c.slug).ToList();
        }

        private async Task LoadDistricts()
        {
            Address.Districts = await ReadJson<AddressVM.District>(DISTRICTS_FILE_PATH) ?? new List<AddressVM.District>();
        }

        private async Task LoadWards()
        {
            Address.Wards = await ReadJson<AddressVM.Ward>(WARDS_FILE_PATH) ?? new List<AddressVM.Ward>();
        }
    }
}
