using WebApplication5.Entities;

namespace WebApplication5.DTOs
{
    public record GetPostWithCommentsDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required int userId { get; set; }
        public ICollection<UsersCommentsDto>? Comments { get; } = new List<UsersCommentsDto>();
        public DateTime CreatedAt { get; set; }
    }
}
