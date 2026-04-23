using Pokemon.Models;

namespace Pokemon.Dto
{
    public class ReviewerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<Review> Reviews { get; set; }
    }
}