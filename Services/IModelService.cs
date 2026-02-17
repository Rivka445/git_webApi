using DTOs;

namespace Services
{
    public interface IModelService
    {
        Task<bool> IsExistsModelById(int id);
        Task<ModelDTO> AddModel(NewModelDTO newModel);
        Task<bool> checkCategories(List<NewCategoryDTO> categories);
        bool checkPrice(int price);
        Task DeleteModel(int id, ModelDTO deleteModel);
        Task<ModelDTO> GetModelById(int id);
        Task<FinalModels> GetModelds(string? description, int? minPrice, int? maxPrice, int[] categoriesId, string? color, int position = 1, int skip = 8);
        Task UpdateModel(int id, ModelDTO updateModel);
        bool ValidateQueryParameters(int position, int skip, int? minPrice, int? maxPrice);
    }
}