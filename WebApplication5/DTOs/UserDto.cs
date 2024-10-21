namespace WebApplication5.DTOs
{
    public record UserDto
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }
}
