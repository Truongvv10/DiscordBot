using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLProvider {
    public static class DataRepositoryFactory {
        public static DataRepositories GetDataRepositories(string connectionString, DatabaseSaveType database) {
            return new DataRepositories(connectionString, database);
        }
    }
}
