using DLL.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCosmos.Contexts {
    public class CosmosDataContext : DataContextAbstract {
        #region Fields
        private readonly string endpoint;
        private readonly string key;
        private readonly string database;
        #endregion

        #region Constructors
        public CosmosDataContext(string? endpoint, string? key, string? connectionString = null) : base(connectionString) {
            this.endpoint = endpoint ?? "https://localhost:8081";
            this.key = key ?? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            database = connectionString ?? "Database";
        }
        #endregion

        #region Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseCosmos(endpoint, key, database);
        }
        #endregion
    }
}
