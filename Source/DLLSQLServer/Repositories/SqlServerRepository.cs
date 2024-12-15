using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using BLL.Services;
using DLL.Contexts;
using DLL.Repositories;
using DLLSQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLSQLServer.Repositories {
    public class SqlServerRepository : DataRepository {

        #region Constructors
        public SqlServerRepository(ICacheData cacheData, DataContextAbstract dataContext) : base(cacheData, dataContext) {
            this.cacheData = cacheData;
            this.dataContext = (SqlServerDataContext)dataContext;
        }
        #endregion

    }
}
