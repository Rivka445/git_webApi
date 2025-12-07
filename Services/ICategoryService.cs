using Entities;

namespace Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategories();
    }
}