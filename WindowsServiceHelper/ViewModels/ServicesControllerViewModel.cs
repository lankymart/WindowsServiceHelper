using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using System.ServiceProcess;
using ReactiveUI.Xaml;
using System.Reactive.Linq;

namespace ServiceProcess.Helpers.ViewModels
{
    internal class ServicesControllerViewModel : ReactiveObject
    {
        public IEnumerable<ServiceViewModel> Services { get; private set; }

        private ServiceViewModel _SelectedService = null;
        public ServiceViewModel SelectedService
        {
            get { return _SelectedService; }
            set { this.RaiseAndSetIfChanged(x => x.SelectedService, value); }
        }

        public ReactiveCommand StartCommand { get; private set; }
        public ReactiveCommand PauseCommand { get; private set; }
        public ReactiveCommand StopCommand { get; private set; }

        public ServicesControllerViewModel(IEnumerable<ServiceViewModel> serviceViewModels)
        {
            Services = serviceViewModels;
            
            var selectedServiceObs = this.ObservableForProperty(x => x.SelectedService).Value();

            //Combine the latest value from either the start or continue executes
            //to determine if you can press the "play" button
            StartCommand = new ReactiveCommand
            (
                selectedServiceObs.SelectMany
                (
                    s => s.StartCommand.CanExecuteObservable
                        .CombineLatest
                        (
                            s.ContinueCommand.CanExecuteObservable, 
                            (l,r) => l || r
                        )
                )
            );

            StopCommand = new ReactiveCommand(selectedServiceObs.SelectMany(s => s.StopCommand.CanExecuteObservable));
            PauseCommand = new ReactiveCommand(selectedServiceObs.SelectMany(s => s.PauseCommand.CanExecuteObservable));

            //TODO: create an observable that returns the play or continue command (or null) depending on the situation
            //That ternary is pretty ugly
            StartCommand.Subscribe
            (
                _ =>
                {
                    var command = (SelectedService.StartCommand.CanExecute(null) ? 
                                    SelectedService.StartCommand : 
                                    SelectedService.ContinueCommand.CanExecute(null) ? 
                                        SelectedService.ContinueCommand : 
                                        null);
                    if (command != null)
                    {
                        command.Execute(null);
                    }
                }
            );

            StopCommand.Subscribe(_ => SelectedService.StopCommand.Execute(null));
            PauseCommand.Subscribe(_ => SelectedService.PauseCommand.Execute(null));

            SelectedService = Services.FirstOrDefault();
        }
        
    }
}
