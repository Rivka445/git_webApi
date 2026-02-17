
using Entities;

namespace Repositories
{
    public interface IModelRepository
    {
        Task<bool> IsExistsModelById(int id);
        Task<Model?> GetModelById(int id);
        Task<(List<Model> Items, int TotalCount)> GetModels(
            string? description, int? minPrice, 
            int? maxPrice, int[] categoriesId,
            string? color, int position = 1, int skip = 8);
        Task<Model> AddModel(Model model);
        Task UpdateModel(Model model);
        Task DeleteModel(Model model);
    }
}