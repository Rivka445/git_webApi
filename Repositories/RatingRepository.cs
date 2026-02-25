using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
namespace Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly EventDressRentalContext _eventDressRentalContext;
        public RatingRepository(EventDressRentalContext eventDressRentalContext)
        {
            _eventDressRentalContext = eventDressRentalContext;
        }
        public async Task<Rating> Add(Rating rating)
        {
            await _eventDressRentalContext.Ratings.AddAsync(rating);
            await _eventDressRentalContext.SaveChangesAsync();
            return rating;
        }
    }
}
