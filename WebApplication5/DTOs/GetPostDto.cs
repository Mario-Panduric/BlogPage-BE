namespace WebApplication5.DTOs
{
    public class GetPostDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string  Description { get; set; }
        public required int userId { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
