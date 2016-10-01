using Entities.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
namespace DataAccess.Context
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("Name=ConnectionString")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Không khởi tạo data, tạo, tạo lại bảng
            //Database.SetInitializer<DataContext>(null);
            //Có khởi tạo data, tạo, tạo lại bảng khi không tồn tại
            //Database.SetInitializer<DataContext>(new DropCreateDatabaseIfModelChanges<DataContext>());
            //Database.SetInitializer<DataContext>(new DropCreateDatabaseAlways<DataContext>());
            //Chỉ sửa đổi cấu trúc bảng, không làm mất dữ liệu
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
            //Bỏ tùy chọn chuyển tên bảng thành dạng số nhiều khi đọc trong CSDL
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //Tạo DbSet
            ConfigureModel(modelBuilder);
            this.Configuration.LazyLoadingEnabled = true;
            base.OnModelCreating(modelBuilder);
        }
        private void ConfigureModel(DbModelBuilder modelBuilder)
        {
            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");
            var entityTypes = Assembly.GetAssembly(typeof(Entity)).GetTypes().Where(x => x.Namespace == "Entities.Models" && !x.IsAbstract).ToList();
            foreach (var type in entityTypes)
            {
                entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, new object[] { });
            }
            //modelBuilder.Entity<Profile>()
            //    .HasOptional(f => f.Account)
            //    .WithRequired(s => s.Profile);
            modelBuilder.Entity<Profile>()
               .HasOptional(f => f.Account)
               .WithOptionalPrincipal()
            .Map(x => x.MapKey("ProfileId"));
            modelBuilder.Entity<Account>()
               .HasOptional(f => f.Profile)
               .WithOptionalPrincipal()
            .Map(x => x.MapKey("AccountId"));
        }
    }
}
