namespace Domain
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        /*public List<Ingredient> Ingredients { get; set; }
        public string Instructions { get; set; }*/

    }
}