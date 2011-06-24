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
        public ReactiveCommand ContinueCommand { get; private set; }

        public ServicesControllerViewModel(IEnumerable<ServiceBase> services)
        {
            Services = services.Select(s => new ServiceViewModel(s)).ToList();

            var selectedServiceObs = this.ObservableForProperty(x => x.SelectedService).Value();

            StartCommand = new ReactiveCommand(selectedServiceObs.SelectMany(s => s.StartCommand.CanExecuteObservable));
            StopCommand = new ReactiveCommand(selectedServiceObs.SelectMany(s => s.StopCommand.CanExecuteObservable));
            PauseCommand = new ReactiveCommand(selectedServiceObs.SelectMany(s => s.PauseCommand.CanExecuteObservable));
            ContinueCommand = new ReactiveCommand(selectedServiceObs.SelectMany(s => s.ContinueCommand.CanExecuteObservable));

            StartCommand.Subscribe(_ => SelectedService.StartCommand.Execute(null));
            StopCommand.Subscribe(_ => SelectedService.StopCommand.Execute(null));
            PauseCommand.Subscribe(_ => SelectedService.PauseCommand.Execute(null));
            ContinueCommand.Subscribe(_ => SelectedService.ContinueCommand.Execute(null));
        }
        
    }
}
