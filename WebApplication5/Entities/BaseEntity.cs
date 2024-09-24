using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool isActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
        [Timestamp]
        public byte[]? TimeStamp { get; set; }
    }
}
