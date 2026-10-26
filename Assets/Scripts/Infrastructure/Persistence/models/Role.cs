using SQLite;

namespace BlueCode3D.Infrastructure.Persistence.Models
{
    [Table("Roles")]
    public class Role
    {
        [PrimaryKey, AutoIncrement]
        public int RoleId { get; set; }

        [Unique, NotNull]
        public string Name { get; set; }
    }
}
