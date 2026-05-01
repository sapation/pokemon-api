using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Pokemon.Dto;
using Pokemon.Interfaces;
using Pokemon.Models;

namespace Pokemon.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;
        private readonly IPokemonRepository _pokemonRepository;
        public OwnerController(IOwnerRepository ownerRepository, 
            IMapper mapper,
             ICountryRepository countryRepository,
             IPokemonRepository pokemonRepository)
        {
            _mapper = mapper;
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _pokemonRepository = pokemonRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type =typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExist(ownerId))
                return NotFound();

            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpGet("/pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnerOfAPokemon(int pokeId)
        {
            var owners = _mapper.Map<OwnerDto>(_ownerRepository.GetOwnerOfAPokemon(pokeId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owners);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PokemonTable>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExist(ownerId))
                return NotFound();
            var pokemons = _mapper.Map<List<PokemonDto>>(
                _ownerRepository.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int pokeId, [FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {           
            if (ownerCreate == null)
                return BadRequest(ModelState);
            var owners = _ownerRepository.GetOwners()
                .Where(o => o.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (owners != null)
            {
                ModelState.AddModelError("", "Owner already exist");
                return StatusCode(422, ModelState);
            }
            var ownerMap = _mapper.Map<Owner>(ownerCreate);
            ownerMap.Country = _countryRepository.GetCountry(countryId);

            if (!_ownerRepository.CreateOwner(pokeId, ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created.");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto ownerUpdate)
        {
            if (ownerUpdate == null || ownerId == 0)
                return BadRequest(ModelState);

            if (ownerId != ownerUpdate.Id)
                return BadRequest(ModelState);

            if (!_ownerRepository.OwnerExist(ownerId))
                return NotFound();

            var ownerMap = _mapper.Map<Owner>(ownerUpdate);
            if (!_ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully updated owner");
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExist(ownerId))
                return NotFound();

            var ownerToDelete = _ownerRepository.GetOwner(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_ownerRepository.DeleteOwner(ownerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully deleted owner");
        }
    }
}