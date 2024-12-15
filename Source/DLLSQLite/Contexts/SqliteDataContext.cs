using BLL.Model;
using DLL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLSQLite.Contexts {
    public class SqliteDataContext : DataContextAbstract {

        #region Fields
        private readonly string file = Path.Combine(Environment.CurrentDirectory, "Saves", "Data.db");
        #endregion

        #region Constructors
        public SqliteDataContext(string? connectionString = null) : base(connectionString) {
            if (connectionString != null) {
                file = Path.Combine(Environment.CurrentDirectory, "Saves", connectionString.Replace("Data Source=", ""));
            } 
        }
        #endregion

        #region Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source={file}");
        }
        #endregion
    }
}
