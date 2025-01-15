using BLL.Enums;
using BLL.Interfaces;
using BLL.Services;
using DLLSQLite.Contexts;
using DLLSQLite.Repositories;
using DLLSQLServer.Contexts;
using DLLSQLServer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLProvider {
    public class DataRepositories {

        public ICacheData cacheData { get; private set; }
        public IDataRepository dataRepository { get; private set; }

        public DataRepositories(string connectionString, DatabaseSaveType database) {
            cacheData = new CacheData();
            try {
                switch (database) {
                    case DatabaseSaveType.SqlServer:
                        var SqlServerContext = new SqlServerDataContext(connectionString);
                        dataRepository = new SqlServerRepository(cacheData, SqlServerContext);
                        break;
                    case DatabaseSaveType.Sqlite:
                        var SqlLiteContext = new SqliteDataContext(connectionString);
                        dataRepository = new SqliteRepository(cacheData, SqlLiteContext);
                        break;
                    default:
                        throw new Exception("Invalid database type");
                }
            } catch (Exception ex) {
                throw new Exception("Error creating Database Repositories", ex);
            }
        }

    }
}
