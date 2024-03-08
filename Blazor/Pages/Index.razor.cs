using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Recipe.Services;
using RecipeApp.Contracts;

namespace Recipe.Pages
{
	public partial class Index
	{
		[Inject] public IRecipeService _recipeService { get; set; }
		[Inject] public IJSRuntime _jsRuntime { get; set; }

		public bool _isLoaded;

		private Modal? _recipeClickedModal = new();
		private Modal? _addRecipeModal = new();
        private string _searchText = "";

		private List<RecipeDto> _recipes = new();
		private List<RecipeDto> _filteredRecipes = new();
		private RecipeDto _selectedRecipe = new();

		// creating new recipe vars
		private RecipeDto _newRecipe = new();
		private IngredientDto _newRecipeIngredient = new();
		private InstructionDto _newRecipeInstruction = new();

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_recipes = await _recipeService.GetAllRecipes();
				_filteredRecipes = _recipes;

				await _jsRuntime.InvokeVoidAsync("PerformAjaxCall");

				// just to simulate load and loading screen
				await Task.Delay(2000);

				_isLoaded = true;
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

		public async Task OnFileUpload(FileUploadEventArgs e)
		{
			try
			{
				using (MemoryStream result = new MemoryStream())
				{
					await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
					_newRecipe.Image = $"data:{e.File.Name.Split('.')[1]};base64,{Convert.ToBase64String(result.ToArray())}";
				}
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.Message);
			}
			finally
			{
				this.StateHasChanged();
			}
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

		private async Task AddUpdateRecipe()
		{
			// check to see if it was an edit or a new recipe
			if (_newRecipe == _selectedRecipe)
			{
				await _recipeService.UpdateRecipe(_newRecipe);
				// update the recipe in the list
				_filteredRecipes[_filteredRecipes.FindIndex(x => x.Id == _newRecipe.Id)] = _newRecipe;
				await _addRecipeModal.Hide();
				return;
			}

			// Add the recipe to the database
			await _recipeService.AddRecipe(_newRecipe);

			// Add the recipe to the list locally
			_filteredRecipes.Add(_newRecipe);

			// Clear the new recipe and close the modal
			_newRecipe = new();
			_newRecipeIngredient = new();
			_newRecipeInstruction = new();

			await _addRecipeModal.Hide();
		}

		// put in util to share out when used across pages
		public static Task OnModalClosing(ModalClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.UserClosing)
				e.Cancel = true;

			return Task.CompletedTask;
		}
	}
}
