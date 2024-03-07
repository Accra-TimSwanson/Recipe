using RecipeApp.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

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
            var result = await httpClient.GetFromJsonAsync<List<RecipeDto>>("https://localhost:7092/Recipe/GetAllRecipes");
            return result != null ? result : new();
        }

        public async Task<RecipeDto> GetRecipeById(Guid Id)
        {
            var result = await httpClient.GetFromJsonAsync<RecipeDto>("https://localhost:7092/Recipe/GetById");
			return result != null ? result : new();
		}

        public async Task<RecipeDto> AddRecipe(RecipeDto recipe)
        {
			var result = await httpClient.PostAsJsonAsync("https://localhost:7092/Recipe/Add", recipe);

            if (result.IsSuccessStatusCode) return recipe;
            
			return new();
		}

        public async Task<RecipeDto> UpdateRecipe(RecipeDto recipe)
        {
            var result = await httpClient.PutAsJsonAsync("https://localhost:7092/Recipe/Update", recipe);
			if (result.IsSuccessStatusCode) return recipe;

			return new();
		}
    }
}
