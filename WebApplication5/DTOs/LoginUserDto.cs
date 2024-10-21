namespace WebApplication5.DTOs
{
    public record LoginUserDto
    {
        public required string UserName { get; set; }
        public required string UserPassword { get; set; }
    }
}
