using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Controls;
using SlideMap.Models;
using SlideMap.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.ViewModels
{
    public class UploadRouteDialogViewModel : PropertyChangedBase

    {
        private readonly UploadRoute _uploadRoute;
        private readonly RouteService _routeService;

        public UploadRouteDialogViewModel()
        {
            _uploadRoute = new UploadRoute();
            _routeService = new RouteService();
        }
        private ObservableCollection<Vehicle> _vehicles;
        public ObservableCollection<Vehicle> Vehicles
        {
            get { return _vehicles; }
            set
            {
                _vehicles = value;
                NotifyPropertyChanged("Vehicles");
            }
        }
        public Vehicle SelectedVehicle { get; set; }

        private ObservableCollection<Mission> _missions;
        public ObservableCollection<Mission> Missions
        {
            get { return _missions; }
            set
            {
                _missions = value;
                NotifyPropertyChanged("Missions");
            }
        }

        public Mission SelectedMission { get; set; }

        private ObservableCollection<Route> _routes;
        public ObservableCollection<Route> Routes
        {
            get { return _routes; }
            set
            {
                _routes = value;
                NotifyPropertyChanged("Routes");
            }
        }

        public Route SelectedRoute { get; set; }

        public UploadRoute GetUploadRoute()
        {
            return _uploadRoute;
        }

        public ICommand CmdOk => new RelayCommand((proWindow) =>
        {
            
            _uploadRoute.SetVehicle(SelectedVehicle);
            var processedRoute = _routeService.CalculateRouteById(SelectedRoute.Id);
            _uploadRoute.SetProcessedRoute(processedRoute);
            (proWindow as ProWindow).DialogResult = true;
            (proWindow as ProWindow).Close();

        }, () => true);

        public ICommand CmdCancel => new RelayCommand((proWindow) =>
        {
            (proWindow as ProWindow).DialogResult = false;
            (proWindow as ProWindow).Close();

        }, () => true);
    }
}
