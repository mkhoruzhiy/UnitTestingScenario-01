using Rwt.Abstractions.Facades;
using Rwt.Abstractions.Services;
using Rwt.Core.Services;
using Rwt.Persistence.Abstractions;
using Rwt.Persistence.Repo;
using Unity;

namespace ConsoleApp.Infrastructure
{
    public static class RegisterServices
    {
        public static IUnityContainer AddRwtServices(this IUnityContainer @this)
        {
            @this
                .RegisterType<IDataImportService, DataImportService>()
                .RegisterType<IMessageQueueService, MessageQueueService>()
                .RegisterType<IRegistryHttpFacade, RegistryHttpFacade>()
                .RegisterType<IPersonRepository, PersonRepository>()
                ;

            return @this;
        }
    }
}
