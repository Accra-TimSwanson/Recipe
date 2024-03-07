using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Contracts;

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
            AppDbContext db = new AppDbContext();
            var existingRecipe = db.Recipe
                .Include(x => x.Ingredients)
                .Include(x => x.Instructions)
                .Where(x => x.Id == recipeDto.Id)
                .FirstOrDefault();

            if (existingRecipe == null)
            {
                throw new Exception("Recipe does not exist");
            }

            if (existingRecipe != null) 
            {
                // map dto to recipe entity
                var recipe = _mapper.Map<Recipe>(recipeDto);

                // update recipe
                db.Entry(existingRecipe).CurrentValues.SetValues(recipe);

                // update the ingredients
                if (recipe.Ingredients != null)
                {
                    // remove any ingredients that are not in the new list as they have been deleted
                    if (existingRecipe.Ingredients != null)
                    {
                        foreach (var existingIngredient in existingRecipe.Ingredients.ToList())
                        {
                            if (!recipe.Ingredients.Any(c => c.Id == existingIngredient.Id))
                                db.Ingredient.Remove(existingIngredient);
                        }
                    }

                    foreach (var ingredient in recipe.Ingredients)
                    {
                        // if the recipe does not have any ingredients
                        // create a new list to handle null reference exception
                        existingRecipe.Ingredients ??= new List<Ingredient>();

                        var existingIngredient = existingRecipe.Ingredients
                            .Where(c => c.Id == ingredient.Id)
                            .SingleOrDefault();
                        
                        if (existingIngredient != null)
                        { 
                            // if the ingredient exists, update it incase values changed
                            db.Entry(existingIngredient).CurrentValues.SetValues(ingredient);
                        }
                        else
                        {
                            // if the ingredient does not exist, add it
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
                }

                // update the instructions
                if (recipe.Instructions != null)
                {
                    // remove any instructions that are not in the new list as they have been deleted
                    if (existingRecipe.Instructions != null)
                    {
                        foreach (var existingInstruction in existingRecipe.Instructions.ToList())
                        {
                            if (!recipe.Instructions.Any(c => c.Id == existingInstruction.Id))
                                db.Instruction.Remove(existingInstruction);
                        }
                    }

                    foreach (var instruction in recipe.Instructions)
                    {
                        // if the recipe does not have any instructions
                        // create a new list to handle null reference exception
                        existingRecipe.Instructions ??= new List<Instruction>();
                            
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
                }

                db.SaveChanges();
            }
        }
    }
}