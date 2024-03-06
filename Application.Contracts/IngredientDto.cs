namespace RecipeApp.Contracts
{
    public class IngredientDto
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }

    }
}