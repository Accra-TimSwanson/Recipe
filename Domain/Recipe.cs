namespace Domain
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? Image { get; set; }
        public List<Ingredient>? Ingredients { get; set; }
        public List<Instruction>? Instructions { get; set; }
    }
}