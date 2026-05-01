using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Pokemon.Dto;
using Pokemon.Interfaces;
using Pokemon.Models;

namespace Pokemon.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;
        public ReviewController(IReviewRepository reviewRepository, 
            IMapper mapper,
            IPokemonRepository pokemonRepository,
            IReviewerRepository reviewerRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExist(reviewId))
                return NotFound();

            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(review);
        }

        [HttpGet("pokeId")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            var review = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return Ok(review);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int pokeId, [FromQuery] int reviewerId, [FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null)
                return BadRequest(ModelState);
            
            var pokemon = _pokemonRepository.GetPokemon(pokeId);
            var reviewer = _reviewerRepository.GetReviewer(reviewerId);

            var reviews = _reviewRepository.GetReviews()
                .Where(r => r.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (reviews != null)
            {
                ModelState.AddModelError("", "Review already exists.");
                return StatusCode(422, ModelState);
            }
            
            var reviewMap = _mapper.Map<Review>(reviewCreate);
            reviewMap.Pokemon = _mapper.Map<PokemonTable>(pokemon);
            reviewMap.Reviewer = _mapper.Map<Reviewer>(reviewer);

            if (!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created.");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto reviewUpdate)
        {
            if (reviewUpdate == null || reviewId == 0)
                return BadRequest(ModelState);

            if (reviewId != reviewUpdate.Id)
                return BadRequest(ModelState);

            if (!_reviewRepository.ReviewExist(reviewId))
                return NotFound();

            var reviewMap = _mapper.Map<Review>(reviewUpdate);
            if (!_reviewRepository.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully updated review");
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExist(reviewId))
                return NotFound();
            var reviewToDelete = _reviewRepository.GetReview(reviewId);
            if (!ModelState.IsValid)
                 return BadRequest(ModelState);
            if (!_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully deleted review");
        }
    }
}