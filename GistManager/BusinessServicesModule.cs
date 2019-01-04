using Autofac;
using GistManager.ErrorHandling;
using GistManager.GistService;
using GistManager.GistService.Wpf;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async;
using GistManager.ViewModels;

namespace GistManager
{
    public class BusinessServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<GistManagerWindow>();
            builder.RegisterType<GistManagerWindowControl>();
            builder.RegisterType<GistManagerWindowViewModel>().SingleInstance();

            builder.RegisterType<GistClientService>().As<IGistClientService>();
            builder.RegisterType<WpfAuthenticationHandler>().As<IAuthenticationHandler>();

            builder.RegisterType<WpfErrorHandler>().As<IErrorHandler>().SingleInstance();
            builder.RegisterType<AsyncOperationStatusManager>().As<IAsyncOperationStatusManager>().SingleInstance();
        }
    }
}
