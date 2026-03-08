using Entities;

namespace Repositories
{
    public interface IDressRepository
    {
        Task<int> GetPriceById(int id);
        Task<bool> IsExistsDressById(int id);
        Task<Dress?> GetDressById(int id);
        Task<List<Dress>> GetDressesByModelId(int modelId);
        Task<Dress?> GetDressByModelIdAndSize(int modelId, string size);
        Task<bool> IsDressAvailable(int id, DateOnly date);
        Task<Dress> AddDress(Dress dress);
        Task UpdateDress(Dress dress);
        Task DeleteDress(int id);
    }
}