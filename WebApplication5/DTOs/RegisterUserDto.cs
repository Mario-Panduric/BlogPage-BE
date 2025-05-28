namespace WebApplication5.DTOs
{
    public record RegisterUserDto
    {
        public required string UserName { get; set; }
        public required string UserPassword { get; set; }
        public required string Email { get; set; }
    }
}
