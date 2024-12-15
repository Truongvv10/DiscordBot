using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Model;
using Newtonsoft.Json;
using DLL.Contexts;

namespace DLLSQLServer.Contexts {
    public class SqlServerDataContext : DataContextAbstract {

        #region Fields
        private readonly string connectionString;
        #endregion

        #region Constructors
        public SqlServerDataContext(string? connectionString = null) : base(connectionString) {
            if (connectionString == null) {
                this.connectionString = "Data Source=LAPTOP-HT0MKK7V\\SQLEXPRESS;Initial Catalog=DiscordBot;Integrated Security=True;Trust Server Certificate=True";
            } else {
                this.connectionString = connectionString;
            }
        }
        #endregion

        #region Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(connectionString);
        }
        #endregion
    }
}
