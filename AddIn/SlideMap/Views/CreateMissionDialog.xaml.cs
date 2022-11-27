using SlideMap.Enum;
using SlideMap.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SlideMap
{
    /// <summary>
    /// Interaction logic for CreateMissionDialog.xaml
    /// </summary>
    public partial class CreateMissionDialog : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        private readonly CreateMissionDialogViewModel _createMissionDialogViewModel = new CreateMissionDialogViewModel();
        private readonly UgCSHelpers _helpers = new UgCSHelpers();

        public CreateMissionDialog()
        {
            InitializeComponent();
            DataContext = _createMissionDialogViewModel;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            AltitudeModeEnum.ItemsSource = System.Enum.GetValues(typeof(AltitudeModeEnum));
            TurnTypeEnum.ItemsSource = System.Enum.GetValues(typeof(TurnTypeEnum));
            Missions.ItemsSource = _helpers.GetMissions(); ;
        }

        private void OnVehicleProfileChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as CreateMissionDialogViewModel;
            var vehicleProfile = _helpers.GetVehicleProfileById(viewModel.SelectedVehicleProfile.Id);
            var profileIds = vehicleProfile.PayloadProfiles.Select(x => x.PayloadProfile.Id).ToList();
            var cameraProfiles = _helpers.GetPayloadProfiles(profileIds);
            if (!cameraProfiles.Any())
            {
                CameraProfile.IsEnabled = false;
                CameraProfile.SelectedItem = null;
                viewModel.SelectedCameraProfile = null;
            }
            else
            {
                CameraProfile.IsEnabled = true;
                CameraProfile.ItemsSource = cameraProfiles;
                CameraProfile.SelectedItem = cameraProfiles.First();
                viewModel.SelectedCameraProfile = cameraProfiles.First();
            }
        }

        private void OnMissionTypeChanged(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;

            if (radioButton == ExistingMissionCheckbox)
            {
                NewMissionName.Text = null;
                NewMissionName.IsEnabled = false;
                Missions.IsEnabled = true;
            }
            else if (radioButton == NewMissionCheckbox)
            {
                Missions.IsEnabled = false;
                NewMissionName.IsEnabled = true;
            }
        }
    }
}
