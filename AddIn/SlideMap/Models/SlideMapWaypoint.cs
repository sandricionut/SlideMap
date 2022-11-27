using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;

namespace SlideMap.Models
{
    class SlideMapWaypoint : SlideMapFeatures
    {

        public SlideMapWaypoint() : base() { }

        public void SetGeometry(MapPoint newMapPoint)
        {
            _attributes["SHAPE"] = newMapPoint;
        }

        public void SetName(string name)
        {
            _attributes["Name"] = name;
        }

        public void SetAMSL(double AMSL)
        {
            _attributes["AMSL"] = AMSL;
        }

        public void SetAGL(double AGL)
        {
            _attributes["AGL"] = AGL;
        }

        public void SetTerrainElevation(double elevation)
        {
            _attributes["Terrain_Elevation"] = elevation;
        }

        public override Dictionary<string, object> GetAttributes()
        {
            if (!_attributes.ContainsKey("SHAPE")
                || !_attributes.ContainsKey("Name")
                || !_attributes.ContainsKey("AMSL")
                || !_attributes.ContainsKey("AGL")
                || !_attributes.ContainsKey("Terrain_Elevation")) throw new Exception("Error returning new SlideMapWaypoint. Not all fields are populated.");
            return _attributes;
        }


    }
}
