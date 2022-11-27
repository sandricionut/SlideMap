using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Controls;
//using ArcGIS.Desktop.Mapping;
using SlideMap.Enum;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.ViewModels
{
    public class CreateMissionDialogViewModel : PropertyChangedBase
    {
        public string FlightSpeed { get; set; } = "4";
        public TurnTypeEnum TurnType { get; set; } = TurnTypeEnum.Spline;
        public string GroundResolution { get; set; } = "4";
        public AltitudeModeEnum AltitudeMode { get; set; } = AltitudeModeEnum.AGL;
        public string ForwardOverlap { get; set; } = "70";
        public string SideOverlap { get; set; } = "70";
        public string DirectionAngle { get; set; } = "0";
        public string AGLTolerance { get; set; } = "3";

        private ObservableCollection<PayloadProfile> _cameraProfiles;
        public ObservableCollection<PayloadProfile> CameraProfiles
        {
            get { return _cameraProfiles; }
            set
            {
                _cameraProfiles = value;
                NotifyPropertyChanged("CameraProfiles");
            }
        }

        public PayloadProfile SelectedCameraProfile { get; set; }


        private ObservableCollection<VehicleProfile> _vehicleProfiles;
        public ObservableCollection<VehicleProfile> VehicleProfiles
        {
            get { return _vehicleProfiles; }
            set
            {
                _vehicleProfiles = value;
                NotifyPropertyChanged("VehicleProfiles");
            }
        }
        public VehicleProfile SelectedVehicleProfile { get; set; }

        /*

        private ObservableCollection<FeatureLayer> _featureLayers;

        
        public ObservableCollection<FeatureLayer> FeatureLayers
        {
            get { return _featureLayers; }
            set
            {
                _featureLayers = value;
                NotifyPropertyChanged("FeatureLayers");
            }
        }
        public FeatureLayer SelectedFeatureLayer { get; set; }
        */

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

        public string MissionName { get; set; }
        public string RouteName { get; set; }

        public ICommand CmdOk => new RelayCommand((proWindow) =>
        {
            SlideMapModule.FlightSpeed = FlightSpeed;
            SlideMapModule.TurnType = TurnType;
            SlideMapModule.GroundResolution = GroundResolution;
            SlideMapModule.AltitudeMode = AltitudeMode;
            SlideMapModule.ForwardOverlap = ForwardOverlap;
            SlideMapModule.SideOverlap = SideOverlap;
            SlideMapModule.DirectionAngle = DirectionAngle;
            SlideMapModule.AGLTolerance = AGLTolerance;
            SlideMapModule.SelectedVehicleProfile = SelectedVehicleProfile;

            (proWindow as ProWindow).DialogResult = true;
            (proWindow as ProWindow).Close();
        }, () => true);

        public ICommand CmdCancel => new RelayCommand((proWindow) =>
        {
            SlideMapModule.FlightSpeed = "4";
            SlideMapModule.TurnType = TurnTypeEnum.Spline;
            SlideMapModule.GroundResolution = "4";
            SlideMapModule.AltitudeMode = AltitudeModeEnum.AGL;
            SlideMapModule.ForwardOverlap = "70";
            SlideMapModule.SideOverlap = "70";
            SlideMapModule.DirectionAngle = "0";
            SlideMapModule.AGLTolerance = "3";
            SlideMapModule.SelectedVehicleProfile = null;
            (proWindow as ProWindow).DialogResult = false;
            (proWindow as ProWindow).Close();
        }, () => true);
    }
}

