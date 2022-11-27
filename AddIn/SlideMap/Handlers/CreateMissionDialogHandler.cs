using System;
using System.Collections.Generic;
using System.Linq;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using SlideMap.Helpers;
using SlideMap.Services;
using SlideMap.ViewModels;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Handlers
{
    internal class CreateMissionDialogHandler : Button
    {
        protected override async void OnClick()
        {
            var missionService = new MissionService();
            var routeService = new RouteService();

            try
            {
                if (SlideMapModule.Executor == null)
                {
                    MessageBox.Show("No execution context could be found. Please login to UgCS server.");
                    return;
                }

                var ugcsHelpers = new UgCSHelpers();
                var arcgisHelper = new ArcGISHelpers();

                CreateMissionDialog createMissionDialog = new CreateMissionDialog()
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };

                var viewModel = createMissionDialog.DataContext as CreateMissionDialogViewModel;
                viewModel.VehicleProfiles = ugcsHelpers.GetVehicleProfiles().AsObservable();
                //viewModel.FeatureLayers = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList().AsObservable();

                var dialogResult = createMissionDialog.ShowDialog();

                if (dialogResult == true)
                {
                    await QueuedTask.Run(async () =>
                    {
                        var selectedVehicleProfile = viewModel.SelectedVehicleProfile;
                        //ArcGIS.Core.Data.Selection selectionfromMap = viewModel.SelectedFeatureLayer.GetSelection();
                        QueryFilter filter = new QueryFilter
                        {
                            //ObjectIDs = selectionfromMap.GetObjectIDs()
                        };

                        Mission mission;

                        if (!string.IsNullOrEmpty(viewModel.MissionName))
                            mission = missionService.CreateNewMission(viewModel.MissionName);
                        else
                            mission = missionService.GetMissionByName(viewModel.SelectedMission.Name);

                        var route = routeService.CreateNewRoute(mission, selectedVehicleProfile, viewModel.RouteName);
                        var scanArea = MapView.Active.Map.GetLayersAsFlattenedList().First((l) => l.Name == "ScanArea") as FeatureLayer;
                        //var scanAreaFC = scanArea.GetFeatureClass() as FeatureClass;

                        using (RowCursor rowCursor = scanArea.Search(filter))
                        {
                            while (rowCursor.MoveNext())
                            {
                                long oid = rowCursor.Current.GetObjectID();
                                Feature feature = rowCursor.Current as Feature;
                                Polygon polygon = feature.GetShape() as Polygon;

                                // route = routeService.SetTakeoffPoint(route, polygon, index++);

                                try
                                {
                                    route = routeService.AddWaypoint(route, polygon, viewModel);
                                    ProcessedRoute processedRoute = routeService.CalculateRouteById(route.Id);
                                    List<Waypoint> waypoints = ugcsHelpers.GetRouteWaypoints(processedRoute);
                                    var pointFeatureLayer = MapView.Active.Map.GetLayersAsFlattenedList().First((l) => l.Name == "Waypoints") as FeatureLayer;
                                    var routeFeatureLayer = MapView.Active.Map.GetLayersAsFlattenedList().First((l) => l.Name == "Route") as FeatureLayer;
                                    var spatialReference = arcgisHelper.GetSpatialReferenceFromFeatureLayer(pointFeatureLayer);
                                    bool finishedEditing = await arcgisHelper.AddWaypointsRouteToArcGISProject(pointFeatureLayer, routeFeatureLayer, waypoints, mission, route, spatialReference);
                                    if (finishedEditing)
                                    {
                                        MessageBox.Show("Mission with processed route created successfully!");
                                    }
                                    else throw new Exception("Error while creating waypoints and route.");
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message);
                                    return;
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($@"Error: {ex.Message}");
            }
        }
    }
}
