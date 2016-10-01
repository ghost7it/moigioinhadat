using Common;
using DataAccess.Context;
using Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataAccess.Extensions;
using DataAccess.Helpers;
using Entities.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Reflection;
namespace DataAccess.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> DbSet;
        private readonly DataContext _dbContext;
        public GenericRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = _dbContext.Set<TEntity>();
        }
        public GenericRepository()
        {
        }
        //#################
        //GetAll
        public IQueryable<TEntity> GetAll(ref Paging paging, Expression<Func<TEntity, string>> orderKey, Expression<Func<TEntity, bool>> predicate)
        {
            paging.TotalRecord = this.Count(predicate);
            var rusult = DbSet.Where(predicate).OrderBy(orderKey).Skip(paging.Skip);
            if (paging.Take != -1)
                return rusult.Take(paging.Take);
            else
                return rusult;
        }
        public IQueryable<TEntity> GetAll(ref Paging paging, string orderKey, Expression<Func<TEntity, bool>> predicate)
        {
            var source = DbSet.Where(predicate);
            var type = typeof(TEntity);
            var property = type.GetProperty(orderKey);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), paging.OrderDirection == "asc" ? "OrderBy" : "OrderByDescending", new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            paging.TotalRecord = source.Count();
            if (paging.Take != -1)
                return source.Provider.CreateQuery<TEntity>(resultExp).Skip(paging.Skip).Take(paging.Take);
            else
                return source.Provider.CreateQuery<TEntity>(resultExp).Skip(paging.Skip);
        }
        public IQueryable<TEntity> GetAll<TKey>(ref Paging paging, Expression<Func<TEntity, TKey>> orderKey, Expression<Func<TEntity, bool>> predicate)
        {
            var source = DbSet.Where(predicate);
            paging.TotalRecord = source.Count();
            IQueryable<TEntity> result;
            if (paging.OrderDirection == "asc" || paging.OrderDirection == "OrderBy")
            {
                result = source.OrderBy(orderKey);
            }
            else
            {
                result = source.OrderByDescending(orderKey);
            }
            if (paging.Take != -1)
                return result.Skip(paging.Skip).Take(paging.Take);
            else
                return result.Skip(paging.Skip);
        }
        public IQueryable<TEntity> GetAll<TKey>(ref Paging paging, Expression<Func<TEntity, TEntity>> keySelectors, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderKey)
        {
            var source = DbSet.Where(predicate);
            paging.TotalRecord = source.Count();
            if (paging.Take != -1)
                return source.Select(keySelectors).OrderBy(orderKey).Skip(paging.Skip).Take(paging.Take);
            else
                return source.Select(keySelectors).OrderBy(orderKey).Skip(paging.Skip);
        }
        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }
        public IQueryable<TEntity> GetAll(ref PagingInformation pagingInfo)
        {
            pagingInfo.NumberOfRecords = this.Count(o => true);
            return DbSet.Skip(pagingInfo.NumberOfSkipItem).Take(pagingInfo.RecordsPerPage);
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, ref PagingInformation pagingInfo)
        {
            pagingInfo.NumberOfRecords = this.Count(predicate);
            return this.GetAll(predicate).Skip(pagingInfo.NumberOfSkipItem).Take(pagingInfo.RecordsPerPage);
        }
        public IEnumerable<TEntity> GetAllUsingStored(string nameOfStore, object obj)
        {
            return _dbContext.Database.SqlQuerySmart<TEntity>(nameOfStore, obj);
        }
        public IQueryable<TEntity> GetAll(string keywords)
        {
            keywords = keywords.Trim();
            var result = DbSet.FullTextSearch<TEntity>(keywords);
            return result;
        }
        //End GetAll
        //#################
        //GetAllAsync
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync<TEntity>();
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync<TEntity>();
        }
        //End GetAllAsync
        //Read #################
        public TEntity Read(long id)
        {
            return DbSet.Find(id);
        }
        public TEntity Read(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }
        public async Task<TEntity> ReadAsync(long id)
        {
            return await DbSet.FindAsync(id);
        }
        public async Task<TEntity> ReadAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.FirstOrDefaultAsync(predicate);
        }
        //Update #################
        public int Update(TEntity entity, long accountId)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return this.SaveChanges(accountId);
        }
        public async Task<int> UpdateAsync(TEntity entity, long accountId)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return await this.SaveChangesAsync(accountId);
        }
        //Create #################
        public int Create(TEntity entity, long accountId)
        {
            DbSet.Add(entity);
            return this.SaveChanges(accountId);
        }
        public async Task<int> CreateAsync(TEntity entity, long accountId)
        {
            DbSet.Add(entity);
            return await this.SaveChangesAsync(accountId);
        }
        public int Create(IEnumerable<TEntity> entities, long accountId)
        {
            DbSet.AddRange(entities);
            return this.SaveChanges(accountId);
        }
        public async Task<int> CreateAsync(IEnumerable<TEntity> entities, long accountId)
        {
            DbSet.AddRange(entities);
            return await this.SaveChangesAsync(accountId);
        }
        //Delete #################
        public int Delete(TEntity entity, long accountId)
        {
            DbSet.Remove(entity);
            return this.SaveChanges(accountId);
        }
        public int Delete(long id, long accountId)
        {
            TEntity entity = Read(id);
            return this.Delete(entity, accountId);
        }
        //public void DeleteItem(TEntity entity)
        //{
        //    DbSet.Remove(entity);
        //}
        public int Delete(Expression<Func<TEntity, bool>> predicate, long accountId)
        {
            var entities = GetAll(predicate);
            //foreach (TEntity entity in entities.ToList<TEntity>())
            //{
            //    DeleteItem(entity);
            //}
            DbSet.RemoveRange(entities);
            return this.SaveChanges(accountId);
        }
        public async Task<int> DeleteAsync(TEntity entity, long accountId)
        {
            DbSet.Remove(entity);
            return await this.SaveChangesAsync(accountId);
        }
        public async Task<int> DeleteAsync(long id, long accountId)
        {
            TEntity entity = Read(id);
            return await this.DeleteAsync(entity, accountId);
        }
        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, long accountId)
        {
            var entities = GetAll(predicate);
            //foreach (TEntity entity in entities.ToList<TEntity>())
            //{
            //    DeleteItem(entity);
            //}
            DbSet.RemoveRange(entities);
            return await this.SaveChangesAsync(accountId);
        }
        public int Delete(IEnumerable<TEntity> entities, long accountId)
        {
            DbSet.RemoveRange(entities);
            return this.SaveChanges(accountId);
        }
        public async Task<int> DeleteAsync(IEnumerable<TEntity> entities, long accountId)
        {
            DbSet.RemoveRange(entities);
            return await this.SaveChangesAsync(accountId);
        }
        //Any #################
        public bool Any()
        {
            return DbSet.Any();
        }
        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }
        public async Task<bool> AnyAsync()
        {
            return await DbSet.AnyAsync();
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }
        //Count #################
        public int Count()
        {
            return DbSet.Count();
        }
        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Count(predicate);
        }
        public async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }
        //Max #################
        public long Max(Expression<Func<TEntity, long>> selector)
        {
            try
            {
                return DbSet.Max(selector);
            }
            catch
            {
                return long.MinValue;
            }
        }
        public async Task<long> MaxAsync(Expression<Func<TEntity, long>> selector)
        {
            try
            {
                return await DbSet.MaxAsync(selector);
            }
            catch
            {
                return long.MinValue;
            }
        }
        public string Max(Expression<Func<TEntity, string>> selector)
        {
            return DbSet.Max(selector);
        }
        //SqlQuery #################
        public IEnumerable<TEntity> SqlQuery(string queryString)
        {
            return _dbContext.Database.SqlQuery<TEntity>(queryString);
        }
        public async Task<IEnumerable<TEntity>> SqlQueryAsync(string queryString)
        {
            return await _dbContext.Database.SqlQuery<TEntity>(queryString).ToListAsync();
        }
        private int SaveChanges(long accountId)
        {
            int result = 0;
            foreach (var ent in _dbContext.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                DbSet DbSetLog = _dbContext.Set<AuditLog>();
                if (ent.State == EntityState.Added)
                {
                    result = _dbContext.SaveChanges();
                    DbSetLog.AddRange(GetAuditRecordsForChange(ent, accountId, true));
                }
                else
                {
                    DbSetLog.AddRange(GetAuditRecordsForChange(ent, accountId));
                }
            }
            int result2 = _dbContext.SaveChanges();
            if (result == 0) result = result2;
            return result;
        }
        private async Task<int> SaveChangesAsync(long accountId)
        {
            int result = 0;
            foreach (var ent in _dbContext.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                DbSet DbSetLog = _dbContext.Set<AuditLog>();
                if (ent.State == EntityState.Added)
                {
                    result = await _dbContext.SaveChangesAsync();
                    DbSetLog.AddRange(GetAuditRecordsForChange(ent, accountId, true));
                }
                else
                {
                    DbSetLog.AddRange(GetAuditRecordsForChange(ent, accountId));
                }
            }
            int result2 = await _dbContext.SaveChangesAsync();
            if (result == 0) result = result2;
            return result;
        }
        private List<AuditLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, long accountId, bool create = false)
        {
            List<AuditLog> result = new List<AuditLog>();
            DateTime changeTime = DateTime.Now;
            // Lấy Table() attribute nếu có
            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            // Lấy Table Name thông qua Table Attr, nếu không có thì lấy theo tên lớp class
            string tmp = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
            string tableName = tmp.Split('_')[0];
            //Lấy tên của khóa chính
            string keyName = dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;
            if (dbEntry.State == EntityState.Added || create)
            {
                result.Add(new AuditLog()
                {
                    AuditLogId = Guid.NewGuid(),
                    AccountId = accountId,
                    EventDate = changeTime,
                    EventType = "C", //C = Create = Tạo mới
                    TableName = tableName,
                    RecordId = (long)dbEntry.CurrentValues.GetValue<object>(keyName),
                    ColumnName = "*ALL",
                    NewValue = (dbEntry.CurrentValues.ToObject() is Entity) ? (dbEntry.CurrentValues.ToObject() as Entity).Describe() : dbEntry.CurrentValues.ToObject().ToString()
                });
            }
            else if (dbEntry.State == EntityState.Deleted)
            {
                result.Add(new AuditLog()
                {
                    AuditLogId = Guid.NewGuid(),
                    AccountId = accountId,
                    EventDate = changeTime,
                    EventType = "D", //D = Deleted = Xóa
                    TableName = tableName,
                    RecordId = (long)dbEntry.OriginalValues.GetValue<object>(keyName),
                    ColumnName = "*ALL",
                    NewValue = (dbEntry.OriginalValues.ToObject() is Entity) ? (dbEntry.OriginalValues.ToObject() as Entity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
                });
            }
            else if (dbEntry.State == EntityState.Modified)
            {
                //Lặp tất cả các cột, nếu có thay đổi thì ghi nhật ký
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    //string x = dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
                    //string y = dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();                    
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        result.Add(new AuditLog()
                        {
                            AuditLogId = Guid.NewGuid(),
                            AccountId = accountId,
                            EventDate = changeTime,
                            EventType = "U", // U = Update = Cập nhật
                            TableName = tableName,
                            RecordId = (long)dbEntry.OriginalValues.GetValue<object>(keyName),
                            ColumnName = propertyName,
                            OriginalValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                            NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                        });
                    }
                }
            }
            return result;
        }
        #region Gọi stored
        ////Những thứ này chưa được test
        ////Gọi stored với output parameter
        //public int GetRowCount(TEntity entity, long id)
        //{
        //    _dbContext.Database.Connection.Open();
        //    DbCommand cmd = _dbContext.Database.Connection.CreateCommand();
        //    cmd.CommandText = "MySproc";
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Add(new SqlParameter("UserID", id));
        //    var totalCountParam = new SqlParameter("TotalCount", 0) { Direction = ParameterDirection.Output };
        //    cmd.Parameters.Add(totalCountParam);

        //    using (var reader = cmd.ExecuteReader())
        //    {
        //        //tasks = reader.MapToList<T>(); //in DataReaderExtensions class
        //    }
        //    //Access output variable after reader is closed
        //    var totalCount = (totalCountParam.Value == null) ? 0 : Convert.ToInt32(totalCountParam.Value);
        //    return totalCount;
        //}
        ////GỌi stored thông thường
        //private void SimpleCallingStored()
        //{
        //    //cách 1
        //    var t = _dbContext.Database.SqlQuery<TEntity>("Name of store", "parameter");
        //    //cách 2
        //    IEnumerable<TEntity> ts = ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<TEntity>("Name of store", "parameter");
        //}
        ////Gọi stored có sql parameter 1
        //private IList<TEntity> GetProductsByCategoryId(int param)
        //{
        //    IList<TEntity> ts;
        //    SqlParameter parameter = new SqlParameter("@parameter", param);
        //    ts = _dbContext.Database.SqlQuery<TEntity>("Name of store @parameter", parameter).ToList();
        //    return ts;
        //}
        ////Gọi stored có sql parameter 2
        //private TEntity GetTById(int id)
        //{
        //    TEntity t = null;
        //    SqlParameter idParameter = new SqlParameter("@id", id);
        //    t = _dbContext.Database.SqlQuery<TEntity>("Name of store @id", idParameter).FirstOrDefault();
        //    return t;
        //}
        #endregion

    }
}
