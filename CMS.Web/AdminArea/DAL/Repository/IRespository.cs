using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Web.AdminArea.DAL.Repository
{
    public interface IRepository<TEntity>
    {
        void SaveOrUpdate(TEntity obj);
        void Delete(TEntity obj);
        TEntity GetById(object objId);
        IQueryable<TEntity> GetAll();
    }
}
