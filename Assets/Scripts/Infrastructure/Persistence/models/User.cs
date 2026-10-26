
using SQLite;

namespace BlueCode3D.Infrastructure.Persistence.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        [NotNull, Unique]
        public string Username { get; set; }

        [NotNull]
        public string DisplayName { get; set; }

        [NotNull]
        public string PasswordHash { get; set; }

        [NotNull]
        public string PasswordSalt { get; set; }

        [NotNull]
        public int RoleId { get; set; }

        [NotNull]
        public string CreatedAtUtc { get; set; }

        [NotNull]
        public bool IsActive { get; set; }
    }
}
