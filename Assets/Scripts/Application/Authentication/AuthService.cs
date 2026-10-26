
using System;
using System.Linq;
using SQLite;
using BlueCode3D.Infrastructure.Persistence;
using BlueCode3D.Infrastructure.Persistence.Models;

namespace BlueCode3D.Application.Authentication
{
    public class AuthenticatedUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string RoleName { get; set; }
    }

    public class AuthService
    {
        public bool Register(
            string username,
            string displayName,
            string password,
            out string message)
        {
            message = string.Empty;

            username = NormalizeUsername(username);
            displayName = displayName == null
                ? string.Empty
                : displayName.Trim();

            if (username.Length < 3 || username.Length > 32)
            {
                message = "El usuario debe tener entre 3 y 32 caracteres.";
                return false;
            }

            if (username.Any(c =>
                !char.IsLetterOrDigit(c) &&
                c != '_' && c != '-' && c != '.'))
            {
                message = "El usuario contiene caracteres no permitidos.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(displayName) ||
                displayName.Length > 80)
            {
                message = "Ingresa un nombre válido.";
                return false;
            }

            if (string.IsNullOrEmpty(password) ||
                password.Length < 8 ||
                password.Length > 256)
            {
                message = "La contraseña debe tener entre 8 y 256 caracteres.";
                return false;
            }

            var db = DatabaseManager.Connection;

            bool exists = db.Table<User>()
                .Any(u => u.Username == username);

            if (exists)
            {
                message = "Ese nombre de usuario ya está registrado.";
                return false;
            }

            Role studentRole = db.Table<Role>()
                .FirstOrDefault(r => r.Name == "Estudiante");

            if (studentRole == null)
            {
                message = "No se encontró el rol de estudiante.";
                return false;
            }

            PasswordHasher.CreateHash(
                password,
                out string passwordHash,
                out string passwordSalt);

            var user = new User
            {
                Username = username,
                DisplayName = displayName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = studentRole.RoleId,
                CreatedAtUtc = DateTime.UtcNow.ToString("o"),
                IsActive = true
            };

            try
            {
                db.Insert(user);
                message = "Registro completado correctamente.";
                return true;
            }
            catch (SQLiteException)
            {
                message = "No se pudo completar el registro.";
                return false;
            }
        }

        public bool Login(
            string username,
            string password,
            out AuthenticatedUser authenticatedUser,
            out string message)
        {
            authenticatedUser = null;
            message = "Credenciales incorrectas o cuenta inactiva.";

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrEmpty(password) ||
                password.Length > 256)
            {
                return false;
            }

            username = NormalizeUsername(username);

            var db = DatabaseManager.Connection;

            User user = db.Table<User>()
                .FirstOrDefault(u => u.Username == username);

            if (user == null || !user.IsActive)
                return false;

            if (!PasswordHasher.Verify(
                password,
                user.PasswordHash,
                user.PasswordSalt))
            {
                return false;
            }

            Role role = db.Table<Role>()
                .FirstOrDefault(r => r.RoleId == user.RoleId);

            if (role == null)
                return false;

            authenticatedUser = new AuthenticatedUser
            {
                UserId = user.UserId,
                Username = user.Username,
                DisplayName = user.DisplayName,
                RoleName = role.Name
            };

            message = "Inicio de sesión correcto.";
            return true;
        }

        private static string NormalizeUsername(string username)
        {
            return (username ?? string.Empty)
                .Trim()
                .ToLowerInvariant();
        }
    }
}
