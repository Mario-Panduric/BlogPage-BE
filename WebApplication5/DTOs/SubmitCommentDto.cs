namespace WebApplication5.DTOs
{
    public record SubmitCommentDto
    {
        public required string Content { get; set; }
        public int? UserId { get; set; }
        public int? PostId { get; set; }
    }
}
