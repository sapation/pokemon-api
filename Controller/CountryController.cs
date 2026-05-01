using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Pokemon.Dto;
using Pokemon.Interfaces;
using Pokemon.Models;

namespace Pokemon.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;
        private readonly IOwnerRepository _ownerRepository;
        public CountryController(ICountryRepository repository, 
            IMapper mapper,
            IOwnerRepository ownerRepository)
        {
            _mapper = mapper;
            _countryRepository = repository;
            _ownerRepository = ownerRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var Countries = _mapper.Map<List<CountryDto>>(
                _countryRepository.GetCountries());

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return Ok(Countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId))
                return NotFound();
            
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryofAnOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDto>(
                _countryRepository.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpGet("/owners/country/{countryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnerFromAcountry(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId)) 
                return NotFound();

            var owners = _countryRepository.GetOwnersFromCountry(countryId);

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return Ok(owners);
        }

        [HttpPost]
        [ProducesResponseType(204, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromQuery] int ownerId, [FromBody] CountryDto countryCreate)
        {   if (countryCreate == null || ownerId == 0)
                return BadRequest(ModelState);

            var countries = _countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (countries != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }

            var countryMap = _mapper.Map<Country>(countryCreate);
            countryMap.Owner = new List<Owner>
            {
                _ownerRepository.GetOwner(ownerId)
            };

            if (!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto countryUpdate)
        {
            if (countryUpdate == null || countryId == 0)
                return BadRequest(ModelState);
            
            if (countryId != countryUpdate.Id)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExist(countryId))
                return NotFound();

            var countryMap = _mapper.Map<Country>(countryUpdate);
            if (!_countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully updated country");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId))
                return NotFound();
            var countryToDelete = _countryRepository.GetCountry(countryId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState); 
            }
            return Ok("Successfully deleted country");
        }
    }
}