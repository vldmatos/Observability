namespace Library.Models
{
    public class Supply
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string WorkStation { get; set; } = string.Empty;
    }
}