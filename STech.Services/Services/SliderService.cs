using MongoDB.Driver;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class SliderService : ISliderService
    {
        private readonly IMongoCollection<Slider> _collection;
        public SliderService(StechDbContext context) => _collection = context.GetCollection<Slider>("Sliders");

        public async Task<IEnumerable<Slider>> GetAll()
        {
            return await _collection.Find(_ => true).ToListAsync();
        } 
    }
}
