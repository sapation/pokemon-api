using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pokemon.Dto;
using Pokemon.Interfaces;
using Pokemon.Models;

namespace Pokemon.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var category = _mapper.Map<PokemonDto>(_categoryRepository.GetCategory(categoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PokemonTable>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategory(int categoryId)
        {
            var pokemon = _mapper.Map<List<PokemonDto>>(
                _categoryRepository.GetPokemonByCategory(categoryId));
            
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromQuery] int pokeId, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null || pokeId == 0)
                return BadRequest(ModelState);

            var categoryExist = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryDto.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (categoryExist != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }

            var category = _mapper.Map<Category>(categoryDto);

            if (!_categoryRepository.CreateCategory(pokeId, category))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created category");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            if (updatedCategory == null || categoryId == 0)
                return BadRequest(ModelState);

            if (categoryId != updatedCategory.Id)
                return BadRequest(ModelState);

            if (!_categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var category = _mapper.Map<Category>(updatedCategory);

            if (!_categoryRepository.UpdateCategory(category))
            { 
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated category");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully deleted category");
        }
    }
}