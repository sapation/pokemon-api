using Pokemon.Models;

namespace Pokemon.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        ICollection<PokemonTable> GetPokemonByCategory(int categoryId);
        bool CategoryExist(int categoryId);
    }
}