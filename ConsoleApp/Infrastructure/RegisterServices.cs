using Microsoft.Extensions.Logging;
using NLog;
using Rwt.Abstractions.Facades;
using Rwt.Abstractions.Services;
using Rwt.Core.Services;
using Rwt.Persistence.Abstractions;
using Rwt.Persistence.Repo;
using Unity;
using Unity.Injection;
using Unity.NLog;

namespace ConsoleApp.Infrastructure
{
    public static class RegisterServices
    {
        public static IUnityContainer AddRwtServices(this IUnityContainer @this)
        {
            @this
                .AddNewExtension<NLogExtension>()
                .RegisterType<IDataImportService, DataImportService>()
                .RegisterType<IMessageQueueService, MessageQueueService>()
                .RegisterType<IRegistryHttpFacade, RegistryHttpFacade>()
                .RegisterType<IPersonRepository, PersonRepository>()
                .RegisterType<Microsoft.Extensions.Logging.ILogger>(new InjectionFactory(l => LogManager.GetCurrentClassLogger()));
                ;

            return @this;
        }
    }
}
