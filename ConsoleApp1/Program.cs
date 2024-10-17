using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ConsoleApp1
{
    public partial class Brand
    {
        public ObjectId _id { get; set; }

        public string BrandId { get; set; }

        public string BrandName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string LogoSrc { get; set; }

    }

    public class Program
    {
        static async Task Main(string[] args)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017/");
            IMongoDatabase _database = client.GetDatabase("STech");
            IMongoCollection<Brand> _brands = _database.GetCollection<Brand>("Brands");

            IEnumerable<Brand> brands = await _brands.Find(_ => true).ToListAsync();

            foreach(Brand b in brands)
            {
                Console.WriteLine(b.BrandName);
            }

            Console.ReadKey();
        }
    }
}
