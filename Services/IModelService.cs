using DTOs;

namespace Services
{
    public interface IModelService
    {
        Task<bool> IsExistsModelById(int id);
        bool CheckDate(DateOnly date);
        Task<ModelResponseDTO> AddModel(NewModelDTO newModel);
        Task<bool> CheckCategories(List<int> categories);
        bool CheckPrice(int price);
        Task<int> GetCountByModelIdAndSizeForDate(int modelId, string size, DateOnly date);
        Task<List<string>> GetSizesByModelId(int modelId);
        Task DeleteModel(int id);
        Task<ModelDTO> GetModelById(int id);
        Task<FinalModels> GetModelds(string? description, int? minPrice, int? maxPrice, int[] categoriesId, string[] color, int position = 1, int skip = 8);
        Task UpdateModel(int id, NewModelDTO updateModel);
        bool ValidateQueryParameters(int position, int skip, int? minPrice, int? maxPrice);
    }
}