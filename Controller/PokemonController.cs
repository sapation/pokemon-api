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
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        public PokemonController(IPokemonRepository pokemonRepository, 
            IMapper mapper,
            IReviewRepository reviewRepository)
        {
            _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PokemonTable>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(PokemonTable))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExist(pokeId))
                return NotFound();

            var pokemon = _mapper.Map <PokemonDto>(_pokemonRepository.GetPokemon(pokeId));

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepository.PokemonExist(pokeId))
                return NotFound();

            var rating = _pokemonRepository.GetPokemonRating(pokeId);

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(204)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
                return BadRequest(ModelState);
            
            var pokemons = _pokemonRepository.GetPokemons()
                .Where(p => p.Name.Trim().ToUpper() == pokemonCreate.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (pokemons != null)
            {
                ModelState.AddModelError("", "Pokemon Already Exist");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var pokemonMap = _mapper.Map<PokemonTable>(pokemonCreate);
            if (!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId, 
            [FromQuery] int ownerId, 
            [FromQuery] int catId, 
            [FromBody] PokemonDto pokemonUpdate)
        {
            if (pokemonUpdate == null || pokeId == 0)
                return BadRequest(ModelState);

            if (pokeId != pokemonUpdate.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExist(pokeId))
                return NotFound();

            var pokemonMap = _mapper.Map<PokemonTable>(pokemonUpdate);
            if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully updated pokemon");
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExist(pokeId))
                return NotFound();

            var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);
            var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if(!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong while deleting reviews");
                return StatusCode(500, ModelState);
            }

            if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully deleted pokemon");
        }
    }
}