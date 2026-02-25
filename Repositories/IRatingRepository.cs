using Entities;

namespace Repositories
{
    public interface IRatingRepository
    {
        Task<Rating> Add(Rating rating);
    }
}