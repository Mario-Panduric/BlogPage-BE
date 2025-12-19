namespace WebApplication5.DTOs
{
    public record GetPostDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required byte[] Image { get; set; }
        public required int userId { get; set; }
        DateTime CreatedAt { get; set; }

    }

}
