using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pokemon.Data;
using Pokemon.Interfaces;
using Pokemon.Models;

namespace Pokemon.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _dataContext;
        public ReviewRepository(DataContext context)
        {
            _dataContext = context;
        }
        public Review GetReview(int reviewId)
        {
            return _dataContext.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _dataContext.Reviews.ToList();
        }

        public ICollection<Review> GetReviewsOfAPokemon(int pokeId)
        {
            return _dataContext.Reviews.Where(r => r.Pokemon.Id == pokeId).ToList();
        }

        public bool ReviewExist(int reviewId)
        {
            return _dataContext.Reviews.Any(r => r.Id == reviewId);
        }
    }
}