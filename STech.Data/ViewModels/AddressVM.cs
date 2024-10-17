using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class AddressVM
    {
        public class City
        {
            public string name { get; set; } = null!;
            public string type { get; set; } = null!;
            public string slug { get; set; } = null!;
            public string name_with_type { get; set; } = null!;
            public string code { get; set; } = null!;
            public List<District> districts { get; set; } = new List<District>();
        }

        public class District
        {
            public string name { get; set; } = null!;
            public string type { get; set; } = null!;
            public string slug { get; set; } = null!;
            public string name_with_type { get; set; } = null!;
            public string path { get; set; } = null!;
            public string path_with_type { get; set; } = null!;
            public string code { get; set; } = null!;
            public string parent_code { get; set; } = null!;
            public List<Ward> wards { get; set; } = new List<Ward>();
        }

        public class Ward
        {
            public string name { get; set; } = null!;
            public string type { get; set; } = null!;
            public string slug { get; set; } = null!;
            public string name_with_type { get; set; } = null!;
            public string path { get; set; } = null!;
            public string path_with_type { get; set; } = null!;
            public string code { get; set; } = null!;
            public string parent_code { get; set; } = null!;
        }

        public List<City> Cities { get; set; } = new List<City>();
        public List<District> Districts { get; set; } = new List<District>();
        public List<Ward> Wards { get; set; } = new List<Ward>();

        public City? _City { get; set; }
        public District? _District { get; set; }
        public Ward? _Ward { get; set; }
    }
}
