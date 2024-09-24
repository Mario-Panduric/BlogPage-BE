﻿using Microsoft.EntityFrameworkCore;
using WebApplication5.Entities;

namespace WebApplication5.Entities
{
    public class UsersComments : BaseEntity
    {
        public required string Content { get; set; }
        public int UserId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public User User { get; set; } = null!;
        public int PostId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Posts Post { get; set; } = null!;
    }
}
