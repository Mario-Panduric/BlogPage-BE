using System.Text.Json.Serialization;
using WebApplication5.Entities;

namespace WebApplication5.DTOs
{
    public class PostsDto
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public int? userID { get; set; }
    }
}
