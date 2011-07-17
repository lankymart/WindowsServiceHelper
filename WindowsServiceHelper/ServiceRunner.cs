using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using ServiceProcess.Helpers.ViewModels;

namespace ServiceProcess.Helpers
{
    public static class ServiceRunner
    {
        public static void LoadServices(this IEnumerable<ServiceBase> services)
        {
            if (Debugger.IsAttached)
            {
                var t = Task.Factory.StartNew
                (
                    () =>
                    {
                        var app = new App();
                        app.InitializeComponent();
                        app.Startup += (o, e) =>
                        {
                            var window = new Window
                            {
                                Width = 350,
                                Height = 200,
                                Title = "Windows Service Runner",
                                Content = new ServicesControllerViewModel(services.Select(s => new ServiceViewModel(s)).ToList())
                            };

                            window.Show();
                        };
                        app.Run();
                    }, 
                    CancellationToken.None, 
                    TaskCreationOptions.PreferFairness, 
                    new StaTaskScheduler(25)
                );
                t.Wait();
            }
            else
            {
                ServiceBase.Run(services.ToArray());
            }
        }
    }
}