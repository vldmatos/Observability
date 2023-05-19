namespace Library.Models
{
    public class Product
    {
        private static readonly Random _random = new();
        private static readonly int _max = 10;

        public static readonly List<string> Workstations = new() { "A01", "A02", "A03", "A04", "A05" };

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = Guid.NewGuid()
                                               .ToString()
                                               .Replace("-", string.Empty)[0..8]
                                               .ToUpper();

        public string Status { get; set; }

        public static IEnumerable<Product> CreateList()
        {
            var size = _random.Next(1, _max);

            for (int index = 0; index < size; index++)
            {
                yield return new();
            }
        }

        public static Product CreateOne() => new();
    }
}