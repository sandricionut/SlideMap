using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Mapping;
using SlideMap.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Helpers
{
    class ArcGISHelpers
    {

        public SpatialReference GetSpatialReferenceFromFeatureLayer(FeatureLayer featureLayer)
        {
            var featureClass = featureLayer.GetTable() as FeatureClass;
            var featureClassDefinition = featureClass.GetDefinition();
            return featureClassDefinition.GetSpatialReference();
        }

        public async Task<bool> AddWaypointsRouteToArcGISProject(FeatureLayer waypointFeatureLayer,FeatureLayer routeFeatureLayer, List<Waypoint> waypoints, Mission mission, Route route, SpatialReference spatialReference)
        {

            var op = new EditOperation();
            op.Name = "Adding waypoints and route";
            op.SelectNewFeatures = false;

            int count = 1;
            MapPoint newMapPoint = null;
            var lineMapPoints = new List<MapPoint>();
            foreach (Waypoint waypoint in waypoints)
            {
                newMapPoint = MapPointBuilderEx.CreateMapPoint(waypoint.Longitude.FromRadians(), waypoint.Latitude.FromRadians(), spatialReference);
                SlideMapWaypoint slideMapWayPoint = new SlideMapWaypoint();
                slideMapWayPoint.SetGeometry(newMapPoint);
                slideMapWayPoint.SetName(string.Format("Waypoint #{0}", count++));
                slideMapWayPoint.SetAMSL(waypoint.Wgs84Altitude);
                slideMapWayPoint.SetAGL(waypoint.Wgs84Altitude - waypoint.Elevation);
                slideMapWayPoint.SetTerrainElevation(waypoint.Elevation);
                op.Create(waypointFeatureLayer, slideMapWayPoint.GetAttributes());
                lineMapPoints.Add(newMapPoint);
            }
            var newPolyline = PolylineBuilderEx.CreatePolyline(lineMapPoints, spatialReference);
            SlideMapRoute slideMapRoute = new SlideMapRoute();
            slideMapRoute.SetGeometry(newPolyline);
            slideMapRoute.SetMissionName(mission.Name);
            slideMapRoute.SetMissionId(mission.Id);
            slideMapRoute.SetRouteName(route.Name);
            slideMapRoute.SetRouteId(route.Id);
            op.Create(routeFeatureLayer,slideMapRoute.GetAttributes());
            await op.ExecuteAsync();
            if (op.IsSucceeded) await Project.Current.SaveEditsAsync();
            return op.IsSucceeded;
        }
    }
}
