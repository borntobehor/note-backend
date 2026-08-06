namespace Backend
{
    public class Note
    {
        public int id {  get; set; }
        public Guid userId { get; set; }
        public required string title { get; set; }
        public string content { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;
    }
}
