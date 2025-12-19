namespace WebApplication5.DTOs
{
    public record UsersCommentsDto
    {
        public required string Content { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public int PostId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
