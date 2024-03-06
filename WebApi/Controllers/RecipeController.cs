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
            // check to see if the recipe exists
            AppDbContext db = new AppDbContext();
            var existingRecipe = db.Recipe
            .Where(x => x.Id == recipeDto.Id)
            .FirstOrDefault();

            if (existingRecipe == null)
			{
				throw new Exception("Recipe does not exist");
			}

			// map to entity
			var recipe = _mapper.Map<Recipe>(recipeDto);

			// save to database
			db.Recipe.Update(recipe);
			db.SaveChanges();
		}
    }
}