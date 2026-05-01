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
            CreateMap<Review, ReviewDto>();
            CreateMap<Reviewer, ReviewerDto>();

            CreateMap<PokemonDto, PokemonTable>();
            CreateMap<ReviewDto, Review>();
            CreateMap<ReviewerDto, Reviewer>();
            CreateMap<OwnerDto, Owner>();
            CreateMap<CountryDto, Country>();
            CreateMap<CategoryDto, Category>();
        }
    }
}