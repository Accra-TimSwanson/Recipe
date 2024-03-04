using RecipeApp.Shared;
using System.Net.Http.Json;

namespace Recipe.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly HttpClient httpClient;

        public RecipeService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<List<RecipeDto>> GetAllRecipes()
        {   
            return await httpClient.GetFromJsonAsync<List<RecipeDto>>("https://localhost:7092/Recipe/GetAllRecipes");
        }

        public async Task<RecipeDto> GetRecipeById(Guid Id)
        {
            return await httpClient.GetFromJsonAsync<RecipeDto>("https://localhost:7092/Recipe/GetById");
        }

        public async Task<RecipeDto> AddRecipe(RecipeDto recipe)
        {
            return await httpClient.GetFromJsonAsync<RecipeDto>("https://localhost:7092/Recipe/get");
        }

        public async Task<RecipeDto> UpdateRecipe(RecipeDto recipe)
        {
            return await httpClient.GetFromJsonAsync<RecipeDto>("https://localhost:7092/Recipe/get");
        }
    }
}
