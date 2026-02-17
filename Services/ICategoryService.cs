using Entities;
using DTOs;

namespace Services
{
    public interface ICategoryService
    {
        Task<bool> IsExistsCategoryById(int id);
        Task<List<CategoryDTO>> GetCategories();
        Task<NewCategoryDTO> GetCategoryId(int id);
        Task<NewCategoryDTO> AddCategory(CategoryDTO newCategory);
    }
}