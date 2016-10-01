using DataAccess.Repositories;
using Interface;
using Ninject.Modules;
namespace CompositionRoot
{
    public class DependencyMapper : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IRepository>().To<Repository>();
        }
    }
}
