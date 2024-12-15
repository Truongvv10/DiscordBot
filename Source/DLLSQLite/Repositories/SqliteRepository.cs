using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using BLL.Services;
using DLL.Contexts;
using DLL.Repositories;
using DLLSQLite.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLSQLite.Repositories {
    public class SqliteRepository : DataRepository {

        #region Constructors
        public SqliteRepository(ICacheData cacheData, DataContextAbstract dataContext) : base(cacheData, dataContext) {
            this.cacheData = cacheData;
            this.dataContext = (SqliteDataContext)dataContext;
        }
        #endregion

    }
}
