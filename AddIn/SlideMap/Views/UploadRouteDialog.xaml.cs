using SlideMap.ViewModels;
using System.Windows.Controls;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap
{
    public partial class UploadRouteDialog : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        private readonly UploadRouteDialogViewModel _uploadRouteDialogViewModel = new UploadRouteDialogViewModel();
        public UploadRouteDialog()
        {
            InitializeComponent();
            DataContext = _uploadRouteDialogViewModel;
        }

        public void OnMissionChanged(object sender,SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as UploadRouteDialogViewModel;
            Mission mission = viewModel.SelectedMission;
            var routes = mission.Routes;
            if (routes.Count==0)
            {
                selectRoute.IsEnabled = false;
                selectRoute.SelectedItem = null;
                viewModel.SelectedRoute = null;
            } else
            {
                selectRoute.IsEnabled = true;
                selectRoute.ItemsSource = routes;
                selectRoute.SelectedItem = routes[0];
                viewModel.SelectedRoute = routes[0];
            }
        }
    }
}
