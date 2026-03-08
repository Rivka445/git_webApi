using DTOs;
using Entities;

namespace Services
{
    public interface IDressService
    {
        Task<bool> IsExistsDressById(int id);
        bool CheckDate(DateOnly date);
        bool CheckPrice(int price);
        Task DeleteDress(int id);
        Task<int> GetPriceById(int id);
        Task<DressDTO> GetDressByModelIdAndSize(int modelId, string size);
        Task<List<DressDTO>> GetDressesByModelId(int modelId);
        Task<DressDTO> GetDressById(int id);
        Task<bool> IsDressAvailable(int id, DateOnly date);
        Task<DressResponseDTO> AddDress(NewDressDTO newDress);
        Task UpdateDress(int id, NewDressDTO updateDress);
    }
}