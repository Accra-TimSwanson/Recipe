
namespace Domain
{
    public class Ingredient
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
    }
}