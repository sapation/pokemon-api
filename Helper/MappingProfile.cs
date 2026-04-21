using AutoMapper;
using Pokemon.Dto;
using Pokemon.Models;

namespace Pokemon.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PokemonTable, PokemonDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Country, CountryDto>();
        }
    }
}