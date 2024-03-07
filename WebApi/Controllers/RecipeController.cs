using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipeController : ControllerBase
    {
        public IMapper _mapper { get; set; }
        public RecipeController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllRecipes")]
        public List<RecipeDto> GetAllRecipes()
        {
            // get data from entity framework
            // TODO: move this to a repository layer
            AppDbContext db = new AppDbContext();
            var recipes = db.Recipe
                .Include(x => x.Ingredients)
                .Include(x => x.Instructions)
                .AsNoTracking()
                .ToList();

            // map to dto
            var recipeDtos = _mapper.Map<List<RecipeDto>>(recipes);

            return recipeDtos;
        }

        [HttpGet]
        [Route("GetById")]
        public RecipeDto GetById(Guid recipeId)
        {
            // get data from entity framework
            // TODO: move this to a repository layer
            AppDbContext db = new AppDbContext();
            var recipe = db.Recipe
                .Include(x => x.Ingredients)
                .Include(x => x.Instructions)
                .Where(x => x.Id == recipeId)
				.AsNoTracking()
				.FirstOrDefault();

            // map to dto
            var recipeDto = _mapper.Map<RecipeDto>(recipe);

            return recipeDto;
        }

        [HttpPost]
        [Route("Add")]
        public void Add(RecipeDto recipeDto)
        {
            // map to entity
            var recipe = _mapper.Map<Recipe>(recipeDto);

            // check to see if the recipe already exists
            AppDbContext db = new AppDbContext();
            var existingRecipe = db.Recipe
				.Where(x => x.Id == recipeDto.Id)
				.FirstOrDefault();

            if (existingRecipe != null)
			{
				throw new Exception("Recipe already exists");
			}

            // save to database
            db.Recipe.Add(recipe);
            db.SaveChanges();
        }

        [HttpPut]
        [Route("Update")]
        public void Update(RecipeDto recipeDto)
        {

        // need top update the recipe, ingredients, and instructions
        // map to entity
            var recipe = _mapper.Map<Recipe>(recipeDto);
            AppDbContext db = new AppDbContext();
            var existingRecipe = db.Recipe
                .Include(x => x.Ingredients)
                .Include(x => x.Instructions)
                .Where(x => x.Id == recipeDto.Id)
                .FirstOrDefault();

            if (existingRecipe != null) {

            // update the recipe
                db.Entry(existingRecipe).CurrentValues.SetValues(recipe);

                // update the ingredients
                foreach (var existingIngredient in existingRecipe.Ingredients.ToList())
                {
                    if (!recipe.Ingredients.Any(c => c.Id == existingIngredient.Id))
                        db.Ingredient.Remove(existingIngredient);
                }
                foreach (var ingredient in recipe.Ingredients)
                {
                    var existingIngredient = existingRecipe.Ingredients
                        .Where(c => c.Id == ingredient.Id)
                        .SingleOrDefault();
                    if (existingIngredient != null)
                        db.Entry(existingIngredient).CurrentValues.SetValues(ingredient);
                    else
                    {
                        var newIngredient = new Ingredient
                        {
                            Name = ingredient.Name,
                            Quantity = ingredient.Quantity,
                            RecipeId = recipe.Id,
                            Unit = ingredient.Unit
                        };
                        db.Ingredient.Add(newIngredient);
                    }
                }

                // update the instructions
                foreach (var existingInstruction in existingRecipe.Instructions.ToList())
                {
                    if (!recipe.Instructions.Any(c => c.Id == existingInstruction.Id))
                        db.Instruction.Remove(existingInstruction);
                }
                foreach (var instruction in recipe.Instructions)
                {
                    var existingInstruction = existingRecipe.Instructions
                        .Where(c => c.Id == instruction.Id)
                        .SingleOrDefault();
                    if (existingInstruction != null)
                        db.Entry(existingInstruction).CurrentValues.SetValues(instruction);
                    else
                    {
                        var newInstruction = new Instruction
                        {
                            Step = instruction.Step,
                            RecipeId = recipe.Id,
                            Description = instruction.Description
                        };
                        db.Instruction.Add(newInstruction);
                    }
                }

                db.SaveChanges();
            }
        }
    }
}