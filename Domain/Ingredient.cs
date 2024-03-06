
namespace Domain
{
    public class Ingredient
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public required string Name { get; set; }
        public required int Quantity { get; set; }
        public required string Unit { get; set; }
    }
}