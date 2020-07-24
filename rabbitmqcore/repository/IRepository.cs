using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbitmqcore.repository
{
    public interface IRepository<T> where T :class  
    {

        public IEnumerable<T> getAll { get; }

        public void Save(T objet);

        public void Commit();
    }
}
