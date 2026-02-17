using AutoMapper;
using Entities;
using DTOs;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;       
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> IsExistsCategoryById(int id)
        {
            return await _categoryRepository.IsExistsCategoryById(id);  
        }
        public async Task<List<CategoryDTO>> GetCategories()
        {
            List<Category> categories = await _categoryRepository.GetCategories();
            List<CategoryDTO> categoriesDTO = _mapper.Map<List<Category>, List<CategoryDTO>>(categories);
            return categoriesDTO;    
        }
        public async Task<CategoryDTO> GetCategoryId(int id)
        {
            Category? category = await _categoryRepository.GetCategoryById(id);
            if (category == null)
                return null;
            CategoryDTO categoryDTO = _mapper.Map<Category, CategoryDTO>(category);
            return categoryDTO;
        }
        public async Task<CategoryDTO> AddCategory(NewCategoryDTO newCategory)
        {
  
            Category category = _mapper.Map<NewCategoryDTO ,Category>(newCategory);
            Category addedCategory = await _categoryRepository.AddCategory(category);
            CategoryDTO categoryDTO = _mapper.Map<Category, CategoryDTO>(addedCategory);
            return categoryDTO;
        }
       
    }
}
