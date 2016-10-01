using System.Threading.Tasks;
namespace Interface
{
    public interface IRepository
    {
        IGenericRepository<T> GetRepository<T>() where T : class;

        int ExecuteSqlCommand(string queryString);
        bool ExecSqlCommand(string queryString);
        int ExecuteSqlCommandWithoutTrans(string queryString);

        Task<int> ExecuteSqlCommandAsync(string queryString);
        Task<bool> ExecSqlCommandAsync(string queryString);
        Task<int> ExecuteSqlCommandWithoutTransAsync(string queryString);
    }
}
