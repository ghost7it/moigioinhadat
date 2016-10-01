using DataAccess.Context;
using Interface;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
namespace DataAccess.Repositories
{
    public class Repository : IRepository
    {
        private DataContext _context;
        public Repository(DataContext context)
        {
            _context = context;
        }
        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }
        public int ExecuteSqlCommand(string queryString)
        {
            return _context.Database.ExecuteSqlCommand(queryString);
        }
        public bool ExecSqlCommand(string queryString)
        {
            return _context.Database.SqlQuery<bool>(queryString).FirstOrDefault();
        }
        public int ExecuteSqlCommandWithoutTrans(string queryString)
        {
            return _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, queryString);
        }
        public async Task<int> ExecuteSqlCommandAsync(string queryString)
        {
            return await _context.Database.ExecuteSqlCommandAsync(queryString);
        }
        public async Task<bool> ExecSqlCommandAsync(string queryString)
        {
            return await _context.Database.SqlQuery<bool>(queryString).FirstOrDefaultAsync();
        }
        public async Task<int> ExecuteSqlCommandWithoutTransAsync(string queryString)
        {
            return await _context.Database.ExecuteSqlCommandAsync(TransactionalBehavior.DoNotEnsureTransaction, queryString);
        }
    }
}
