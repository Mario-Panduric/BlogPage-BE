using System.Text.Json.Serialization;
using WebApplication5.Entities;

namespace WebApplication5.DTOs
{
    public record PostsDto
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required byte[] Image { get; set; }
        public int? userID { get; set; }
        [JsonIgnore]
        public ICollection<UsersComments>? Comments { get; } = new List<UsersComments>();
    }
}
