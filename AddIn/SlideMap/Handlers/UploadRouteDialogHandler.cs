using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using SlideMap.ViewModels;
using SlideMap.Helpers;
using SlideMap.Models;
using System;

namespace SlideMap.Handlers
{
    internal class UploadRouteDialogHandler : Button
    {

        protected override async void OnClick()
        {
            if (SlideMapModule.Executor == null)
            {
                MessageBox.Show("No execution context could be found. Please login to UgCS server.");
                return;
            }

            var ugcsHelpers = new UgCSHelpers();

            UploadRouteDialog uploadRouteDialog = new UploadRouteDialog()
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            var viewModel = uploadRouteDialog.DataContext as UploadRouteDialogViewModel;
            viewModel.Vehicles = ugcsHelpers.GetVehicles().AsObservable();
            viewModel.Missions = ugcsHelpers.GetMissions().AsObservable();

            var dialogUpload = uploadRouteDialog.ShowDialog();
            if (dialogUpload == true)
            {
                await QueuedTask.Run(() =>
                {
                    try
                    {
                        UploadRoute uploadRoute = viewModel.GetUploadRoute();
                        bool finishedUploading = uploadRoute.Upload();
                        if (finishedUploading)
                        {
                            SlideMapModule.CurrentVehicle = uploadRoute.GetVehicle();
                            MessageBox.Show("Processed route uploaded successfully!");
                        }
                        else throw new Exception("Error while uploading route.");
                    } catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                });
            }
        }

    }
}
