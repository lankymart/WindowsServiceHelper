using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Xaml;
using System.ServiceProcess;
using ServiceProcess.Helpers;
using System.Reactive.Linq;
using System.Windows;
using ServiceProcess.Helpers.Helpers;

namespace ServiceProcess.Helpers.ViewModels
{
    internal class ServiceViewModel : ReactiveObject
    {
        public ReactiveCommand StartCommand { get; private set; }
        public ReactiveCommand PauseCommand { get; private set; }
        public ReactiveCommand StopCommand { get; private set; }
        public ReactiveCommand ContinueCommand { get; private set; }
        
        public string Name { get; private set; }
        
        private bool _IsBusy = false;
        public bool IsBusy 
        { 
            get { return _IsBusy; } 
            set 
            { 
                this.RaiseAndSetIfChanged(x => x.IsBusy, value); 
            } 
        }

        private ServiceState _CurrentState = ServiceState.Stopped;
        public ServiceState CurrentState
        {
            get { return _CurrentState; }
            set { this.RaiseAndSetIfChanged(x => x.CurrentState, value); }
        }

        public ServiceViewModel(ServiceBase service)
        {
            Name = service.ServiceName;

            //Get an observable for the current state
            var currentStateObs = this.ObservableForProperty(x => x.CurrentState).Value();

            //Map an observable to IsBusy that is True if the current state is *ing

            currentStateObs.Select
            (
                s => s == ServiceState.Pausing ||
                     s == ServiceState.Starting ||
                     s == ServiceState.Stopping
            )
            .Subscribe(s => IsBusy = s);

            StartCommand = new ReactiveCommand(currentStateObs.Select(s => s == ServiceState.Stopped));
            StopCommand = new ReactiveCommand(currentStateObs.Select(s => s == ServiceState.Started || s == ServiceState.Paused));
            PauseCommand = new ReactiveCommand(currentStateObs.Select(s => s == ServiceState.Started && service.CanPauseAndContinue));
            ContinueCommand = new ReactiveCommand(currentStateObs.Select(s => s == ServiceState.Paused && service.CanPauseAndContinue));

            AssignmentSubscription(StartCommand, () => ServiceBaseHelpers.StartService(service));
            AssignmentSubscription(StopCommand, () => ServiceBaseHelpers.StopService(service));
            AssignmentSubscription(PauseCommand, () => ServiceBaseHelpers.PauseService(service));
            AssignmentSubscription(ContinueCommand, () => ServiceBaseHelpers.ContinueService(service));

        }

        private void AssignmentSubscription(ReactiveCommand command, Func<IObservable<ServiceState>> serviceOperation)
        {
            command.SelectMany(_ => serviceOperation().SubscribeOnDispatcher())
                    .Subscribe
                    (
                        s => CurrentState = s,
                        ex => MessageBox.Show(ex.ToString())
                    );
        }

    }
}
