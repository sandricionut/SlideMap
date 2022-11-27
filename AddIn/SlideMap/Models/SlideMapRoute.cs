using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;

namespace SlideMap.Models
{
    class SlideMapRoute:SlideMapFeatures
    {
  

        public SlideMapRoute():base(){}

        public void SetGeometry(Polyline newPolyline)
        {
            _attributes["SHAPE"] = newPolyline;
        }

        public void SetMissionName(string missionName)
        {
            _attributes["MissionName"] = missionName;
        }

        public void SetMissionId(int missionId)
        {
            _attributes["MissionId"] = missionId;
        }

        public void SetRouteName(string routeName)
        {
            _attributes["RouteName"] = routeName;
        }

        public void SetRouteId(int routeId)
        {
            _attributes["RouteId"] = routeId;
        }

        public override Dictionary<string, object> GetAttributes()
        {
            if (!_attributes.ContainsKey("SHAPE")
                || !_attributes.ContainsKey("MissionName")
                || !_attributes.ContainsKey("MissionId")
                || !_attributes.ContainsKey("RouteName")
                || !_attributes.ContainsKey("RouteId")) throw new Exception("Error returning new SlideMapRoute. Not all fields are populated.");
            return _attributes;
        }
    }
}
