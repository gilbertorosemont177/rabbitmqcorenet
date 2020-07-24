using Microsoft.EntityFrameworkCore;
using rabbitmqcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbitmqcore.repository
{
    public abstract class Repository<T> : IRepository<T>, IDisposable where T: class 
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dataSet;
        private bool disposed = false;
        public Repository(LibraryContext db)
        {
            _dbContext = db;
            _dataSet = this._dbContext.Set<T>();
            
        }
       
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }


        public void Save(T c)
        {
            _dataSet.Add(c);
            this.Commit();
          
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public IEnumerable<T> getAll
        {
            
            get
            {
                return _dataSet.ToList();
            }
        }

        public void Commit()
        {
            this._dbContext.SaveChanges();
        }
       
    
    }
}
