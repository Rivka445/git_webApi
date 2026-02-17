using AutoMapper;
using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace Services
{
    public class DressService : IDressService
    {
        private readonly IDressRepository _dressRepository;
        private readonly IMapper _mapper;
        public DressService(IDressRepository dressRepository, IMapper mapper)
        {
            _mapper = mapper;
            _dressRepository = dressRepository;
        }
        public async Task<bool> IsExistsDressById(int id)
        {
            return await _dressRepository.IsExistsDressById(id);
        }
        public bool checkPrice(int price)
        {
            return price > 0;
        }
        public bool checkDate(DateOnly date)
        {
            return date > DateOnly.FromDateTime(DateTime.Now);
        }
        public bool checkDressByDate(int id,DateOnly date)
        {
            return date > DateOnly.FromDateTime(DateTime.Now);
        }
        public async Task<int> GetPriceById(int id)
        {
            return await _dressRepository.GetPriceById(id);
        }
        public async Task<DressDTO> GetDressById(int id)
        {
            Dress? dress = await _dressRepository.GetDressById(id);
            if (dress == null)
                return null;
            DressDTO dressDTO = _mapper.Map<Dress, DressDTO>(dress);
            return dressDTO;
        }
        public async Task<List<string>> GetSizesByModelId(int modelId)
        {
            return await _dressRepository.GetSizesByModelId(modelId);
        }
        public async Task<bool> IsDressAvailable(int id, DateOnly date)
        {
            return await _dressRepository.IsDressAvailable(id, date);
        }

        public async Task<int> GetCountByModelIdAndSizeForDate(int modelId, string size, DateOnly date)
        {
            return await _dressRepository.GetCountByModelIdAndSizeForDate(modelId, size, date);
        }
        public async Task<DressDTO> AddDress(NewDressDTO newDress)
        {
            Dress addedDress = _mapper.Map<NewDressDTO, Dress>(newDress);
            addedDress.IsActive = true;
            Dress dress = await _dressRepository.AddDress(addedDress);
            DressDTO dressDTO = _mapper.Map<Dress, DressDTO>(dress);
            return dressDTO;
        }
        public async Task UpdateDress(int id, DressDTO updateDress)
        {
            Dress update = _mapper.Map<DressDTO, Dress>(updateDress);
            await _dressRepository.UpdateDress(update);
        }
        public async Task DeleteDress(int id, DressDTO deleteDress)
        {
            Dress dress = _mapper.Map<DressDTO, Dress>(deleteDress);
            dress.IsActive = false;
            await _dressRepository.DeleteDress(dress);
        }

    }
}
