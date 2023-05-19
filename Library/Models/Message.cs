namespace Library.Models
{
    public class Message
    {
        public const string EntryCode = "entry";
        public const string ApproveCode = "approve";
        public const string FinalizedCode = "finalized";

        public string Code { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new();
        public DateTime Date { get; set; }
    }
}