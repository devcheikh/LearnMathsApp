using LearnMaths.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnMaths.Repositories
{
    public interface IUowData
    {
        IRepository<User> Users { get; }

        IRepository<UserRole> UserRoles { get; }

        IRepository<Level> Levels { get; }

        IRepository<Category> Categories { get; }

        IRepository<Question> Questions { get; }

        IRepository<Answer> Answers { get; }

        IRepository<CoverLevel> CoverLevels { get; }

        IRepository<Record> Records { get; }

        int SaveChanges();
    }
}
