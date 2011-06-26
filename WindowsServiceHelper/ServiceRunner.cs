using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Windows;
using ServiceProcess.Helpers.ViewModels;
using System.Diagnostics;
using System.Threading;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace ServiceProcess.Helpers
{
    public static class ServiceRunner
    {
        

        public static void LoadServices(this IEnumerable<ServiceBase> services)
        {
            if (Debugger.IsAttached)
            {
                var t = Task.Factory.StartNew(() =>
                    {
                        App app = new App();
                        app.InitializeComponent();
                        app.Startup += (o, e) =>
                            {
                                Window window = new Window();
                                window.Title = "Windows Service Runner";
                                window.Content = new ServicesControllerViewModel(services.Select(s => new ServiceViewModel(s)).ToList());
                                window.Show();
                            };
                        app.Run();
                    }, CancellationToken.None, TaskCreationOptions.PreferFairness, new StaTaskScheduler(25));
                t.Wait();
            }
            else
            {
                ServiceBase.Run(services.ToArray());
            }
        }
    }
}
