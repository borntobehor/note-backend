namespace Backend.Entities
{
    public class User
    {
        public Guid id { set; get; }
        public string username { set; get; } = string.Empty;
        public string passwordHash { set; get; } = string.Empty;
    }
}
