using BLL.Interfaces;
using DLL.Contexts;
using DLL.Repositories;
using DLLCosmos.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCosmos.Repositories {
    public class CosmosRepository : DataRepository {
        #region Constructors
        public CosmosRepository(ICacheData cacheData, CosmosDataContext dataContext) : base(cacheData, dataContext) {
            this.cacheData = cacheData;
            this.dataContext = dataContext;
        }
        #endregion
    }
}
