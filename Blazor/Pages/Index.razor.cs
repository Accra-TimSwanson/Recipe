using Blazorise;
using Microsoft.AspNetCore.Components;
using Recipe.Services;
using RecipeApp.Contracts;

namespace Recipe.Pages
{
    public partial class Index
	{
		private Modal _recipeClickedModal = new();
		private Modal _addRecipeModal = new();
		private string _searchText = "";

		private List<RecipeDto> _recipes = new();
		private List<RecipeDto> _filteredRecipes = new();
		private RecipeDto _selectedRecipe = new();

		// creating new recipe vars
		private RecipeDto _newRecipe = new();
		private IngredientDto _newRecipeIngredient = new();
		private InstructionDto _newRecipeInstruction = new();

		[Inject]
        public IRecipeService _recipeService { get; set; }

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_recipes = await _recipeService.GetAllRecipes();
				_filteredRecipes = _recipes;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private void RecipeClicked(RecipeDto recipe)
		{
			_selectedRecipe = recipe;
			_recipeClickedModal.Show();
		}

		private void Search()
		{
			// set to lower so we can ignore case
			_searchText = _searchText.ToLower();

			_filteredRecipes = _recipes.Where(r => r.Title.ToLower().Contains(_searchText) ||
												r.Ingredients.Any(i => i.Name.ToLower().Contains(_searchText))).ToList();
		}

		private void CreateRecipe()
		{
			_newRecipe = new();
			_newRecipe.Id = Guid.NewGuid();
			_addRecipeModal.Show();
		}

		private void AddIngredient()
		{
			// Add the ingredient to the list
			if (_newRecipe.Ingredients is null)
				_newRecipe.Ingredients = new();

			_newRecipe.Ingredients.Add(
				new IngredientDto
				{
					Id = Guid.NewGuid(),
					RecipeId = _newRecipe.Id,
					Name = _newRecipeIngredient.Name,
					Quantity = _newRecipeIngredient.Quantity,
					Unit = _newRecipeIngredient.Unit
				}
			);

			// Clear the ingredient so we can add another
			_newRecipeIngredient = new();
		}

		private void AddInstruction()
		{
			// Add the instruction to the list
			if (_newRecipe.Instructions is null)
				_newRecipe.Instructions = new();

			_newRecipe.Instructions.Add(
				new InstructionDto
				{
					Id = Guid.NewGuid(),
					RecipeId = _newRecipe.Id,
					Step = _newRecipe.Instructions.Count() + 1,
					Description = _newRecipeInstruction.Description
				}
			);

			// Clear the instruction so we can add another
			_newRecipeInstruction = new();
		}

		private async Task AddRecipe()
		{
			// check to see if it was an edit or a new recipe
			if (_newRecipe == _selectedRecipe)
			{
				//await RecipeService.UpdateRecipe(_newRecipe);
				// update the recipe in the list
				_filteredRecipes[_filteredRecipes.FindIndex(x => x.Id == _newRecipe.Id)] = _newRecipe;
				await _addRecipeModal.Hide();
				return;
			}

			// add img
			_newRecipe.Image = "https://via.placeholder.com/150";

			// Add the recipe to the database
			await RecipeService.AddRecipe(_newRecipe);

			// Add the recipe to the list locally
			_filteredRecipes.Add(_newRecipe);

			// Clear the new recipe and close the modal
			_newRecipe = new();
			_newRecipeIngredient = new();
			_newRecipeInstruction = new();

			await _addRecipeModal.Hide();
		}
	}
}
