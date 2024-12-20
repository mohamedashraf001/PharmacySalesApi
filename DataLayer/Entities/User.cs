using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Address { get; set; }
        public string? NationalId { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // New properties for refresh tokens
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
