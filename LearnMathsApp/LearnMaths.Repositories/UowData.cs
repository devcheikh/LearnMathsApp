using LearnMaths.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnMaths.Repositories
{
    public class UowData : IUowData
    {
        private readonly DbContext context;
        private readonly Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public UowData()
            : this(new db95e1649d89d04f53bfe6a26f0095fb22Entities())
        {
        }

        public UowData(DbContext context)
        {
            this.context = context;
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(GenericRepository<T>);

                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }

        public IRepository<User> Users
        {
            get { return this.GetRepository<User>(); }
        }

        public IRepository<UserRole> UserRoles
        {
            get { return this.GetRepository<UserRole>(); }
        }

        public IRepository<Level> Levels
        {
            get { return this.GetRepository<Level>(); }
        }

        public IRepository<Category> Categories
        {
            get { return this.GetRepository<Category>(); }
        }

        public IRepository<Question> Questions
        {
            get { return this.GetRepository<Question>(); }
        }

        public IRepository<Answer> Answers
        {
            get { return this.GetRepository<Answer>(); }
        }

        public IRepository<CoverLevel> CoverLevels
        {
            get { return this.GetRepository<CoverLevel>(); }
        }

        public IRepository<Record> Records
        {
            get { return this.GetRepository<Record>(); }
        }
    }
}
