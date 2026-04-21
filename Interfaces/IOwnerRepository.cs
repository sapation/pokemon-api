using Pokemon.Models;

namespace Pokemon.Interfaces
{
    public interface IOwnerRepository
    {
        ICollection<Owner> GetOwners();
        Owner GetOwner(int ownerId);
        ICollection<Owner> GetOwnerOfAPokemon(int pokeId);
        ICollection<PokemonTable> GetPokemonByOwner(int ownerId);
        bool OwnerExist(int ownerId);
    }
}