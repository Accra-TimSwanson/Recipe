namespace Domain
{
    public class Instruction
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public int Step { get; set; }
        public required string Description { get; set; }
    }
}