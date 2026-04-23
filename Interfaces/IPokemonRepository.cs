using Pokemon.Models;

namespace Pokemon.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<PokemonTable> GetPokemons();
        PokemonTable GetPokemon(int id);
        PokemonTable GetPokemon(string name);
        decimal GetPokemonRating(int pokemonId);
        bool PokemonExist(int pokemonId);
        bool CreatePokemon(int ownerId, int categoryId, PokemonTable pokemon);
        bool Save();

    }
}