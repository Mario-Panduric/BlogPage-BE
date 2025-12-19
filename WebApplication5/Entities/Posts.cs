using System.Text.Json.Serialization;
using WebApplication5.Entities;

namespace WebApplication5.Entities
{
    public class Posts : BaseEntity
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public required byte[] Image { get; set; }
        public ICollection<UsersComments>? Comments { get; } = new List<UsersComments>();

    }
}
