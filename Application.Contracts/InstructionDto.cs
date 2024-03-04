namespace RecipeApp.Shared
{
	public class InstructionDto
	{
		public Guid Id { get; set; }
		public Guid RecipeId { get; set; }
		public int Step { get; set; }
		public string Description { get; set; }
	}
}