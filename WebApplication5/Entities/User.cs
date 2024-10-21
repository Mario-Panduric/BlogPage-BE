using System.Text.Json.Serialization;

namespace WebApplication5.Entities
{
    public class User : BaseEntity
    {
        public required string UserName { get; set; }
        public required string UserPassword { get; set; }
        public required string Email { get; set; }
        public ICollection<UsersComments> Comments { get; } = new List<UsersComments>();
        public ICollection<Posts> Posts { get; } = new List<Posts>();
    }
}
