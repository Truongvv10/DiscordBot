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
        public CosmosDataContext(string? connectionString = null) : base(connectionString) {
            if (connectionString is not null) {
                var values = connectionString.Split(";");
                endpoint = values[0];
                key = values[1];
                database = values[2];
            }
        }
        #endregion

        #region Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseCosmos(endpoint, key, database);
        }
        #endregion
    }
}
