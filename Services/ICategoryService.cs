using Entities;
using DTOs;

namespace Services
{
    public interface ICategoryService
    {
        Task<bool> IsExistsCategoryById(int id);
        Task<List<CategoryDTO>> GetCategories();
        Task<CategoryDTO> GetCategoryId(int id);
        Task<CategoryDTO> AddCategory(NewCategoryDTO newCategory);
    }
}