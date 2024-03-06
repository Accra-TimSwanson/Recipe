using AutoMapper;
using Domain;
using RecipeApp.Contracts;

namespace WebApi
{
    public class AutomapperConfig:Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Recipe, RecipeDto>().ReverseMap();
            CreateMap<Recipe, RecipeDto>().ReverseMap();
            CreateMap<Ingredient, IngredientDto>().ReverseMap();
            CreateMap<Instruction, InstructionDto>().ReverseMap();
        }
    }
}
