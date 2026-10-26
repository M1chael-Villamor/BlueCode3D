using System;
using System.IO;
using SQLite;
using UnityEngine;
using BlueCode3D.Infrastructure.Persistence.Models;

namespace BlueCode3D.Infrastructure.Persistence
{
    public static class DatabaseManager
    {
        private static SQLiteConnection _connection;

        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                    Initialize();

                return _connection;
            }
        }

        public static void Initialize()
        {
            if (_connection != null)
                return;

            string databasePath = Path.Combine(
                Application.persistentDataPath,
                "bluecode3d.db"
            );

            _connection = new SQLiteConnection(databasePath);

            _connection.CreateTable<Role>();
            _connection.CreateTable<User>();

            SeedRole("Estudiante");
            SeedRole("Docente");
            SeedRole("Administrador");

            Debug.Log("Base de datos SQLite inicializada.");
        }

        private static void SeedRole(string roleName)
        {
            Role role = _connection
                .Table<Role>()
                .FirstOrDefault(r => r.Name == roleName);

            if (role == null)
            {
                _connection.Insert(new Role
                {
                    Name = roleName
                });
            }
        }

        public static void Close()
        {
            if (_connection == null)
                return;

            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }
}
