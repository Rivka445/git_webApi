using Entities;

namespace Services
{
    public interface IRatingService
    {
        Task<Rating> Add(Rating rating);
    }
}