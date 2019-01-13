using Autofac;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GistManager
{
    public class AutofacEnabledAsyncPackage : AsyncPackage
    {
        private IContainer container;
        private readonly ContainerBuilder containerBuilder;
        public AutofacEnabledAsyncPackage()
        {
            containerBuilder = new ContainerBuilder();
        }

        public void RegisterModule<TModule>() where TModule : Autofac.Core.IModule, new()
        {
            containerBuilder.RegisterModule<TModule>();
        }

        protected override System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            container = containerBuilder.Build();
            return base.InitializeAsync(cancellationToken, progress);
        }

        protected override object GetService(Type serviceType)
        {
            if (container?.IsRegistered(serviceType) ?? false)
            {
                return container.Resolve(serviceType);
            }
            return base.GetService(serviceType);
        }

        protected override WindowPane InstantiateToolWindow(Type toolWindowType) => (WindowPane)GetService(toolWindowType);

        protected override void Dispose(bool disposing)
        {
            try
            {
                container?.Dispose();
            }
            catch { }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
