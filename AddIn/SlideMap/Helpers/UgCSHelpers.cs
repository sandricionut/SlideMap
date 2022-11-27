using System;
using System.Collections.Generic;
using System.Linq;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap
{
    public class UgCSHelpers
    {
        public VehicleProfile GetVehicleProfileById(int id)
        {
            MessageFuture<GetObjectResponse> listFuture =
            SlideMapModule.Executor.Submit<GetObjectResponse>(
            new GetObjectRequest
            {
                ClientId = SlideMapModule.ClientId,
                ObjectType = "VehicleProfile",
                ObjectId = id
            });

            return listFuture.Value.Object.VehicleProfile;
        }



        public PayloadProfile GetPayloadProfileById(int id)
        {
            MessageFuture<GetObjectResponse> listFuture =
            SlideMapModule.Executor.Submit<GetObjectResponse>(
            new GetObjectRequest
            {
                ClientId = SlideMapModule.ClientId,
                ObjectType = "PayloadProfile",
                ObjectId = id
            });

            return listFuture.Value.Object.PayloadProfile;
        }

        public List<PayloadProfile> GetPayloadProfiles(List<int> ids)
        {
            List<PayloadProfile> profiles = new List<PayloadProfile>();
            foreach (var id in ids)
            {
                MessageFuture<GetObjectResponse> listFuture =
                SlideMapModule.Executor.Submit<GetObjectResponse>(
                new GetObjectRequest
                {
                    ClientId = SlideMapModule.ClientId,
                    ObjectType = "PayloadProfile",
                    ObjectId = id
                });
                profiles.Add(listFuture.Value.Object.PayloadProfile);

            }
            return profiles;
        }

        public List<VehicleProfile> GetVehicleProfiles()
        {
            GetObjectListRequest request = new GetObjectListRequest()
            {
                ClientId = SlideMapModule.ClientId,
                ObjectType = "VehicleProfile",
                RefreshDependencies = true
            };
            request.RefreshDependencies = true;
            var task = SlideMapModule.Executor.Submit<GetObjectListResponse>(request);
            task.Wait();

            var list = task.Value.Objects;
            List<VehicleProfile> vehiclesWithCameras = new List<VehicleProfile>();

            foreach (var o in list)
            {
                var payloadProfiles = o.VehicleProfile.PayloadProfiles;
                foreach (VehicleProfilePayloadProfile vehProfPaylProf in payloadProfiles)
                {
                    var payloadProfile = vehProfPaylProf.PayloadProfile;
                    if (payloadProfile != null && payloadProfile.PayloadType == PayloadType.PT_CAMERA)
                    {
                        if (!vehiclesWithCameras.Contains(o.VehicleProfile))
                        {
                            vehiclesWithCameras.Add(o.VehicleProfile);
                        }
                    }

                }
            }
            if (vehiclesWithCameras.Count == 0) throw new Exception("No vehicle profile with payload profile of type camera found.");
            return vehiclesWithCameras;
        }

        public List<Mission> GetMissions()
        {
            MessageFuture<GetObjectListResponse> listFuture = SlideMapModule.Executor.Submit<GetObjectListResponse>(
                 new GetObjectListRequest
                 {
                     ClientId = SlideMapModule.ClientId,
                     ObjectType = "Mission"
                 });
            GetObjectListResponse listResp = listFuture.Value;
            return listResp.Objects.Select(x => x.Mission).ToList();
        }

        public Vehicle GetVehicleByName(string vehicleName)
        {
            GetObjectListRequest request = new GetObjectListRequest()
            {
                ClientId = SlideMapModule.ClientId,
                ObjectType = "Vehicle",
                RefreshDependencies = true
            };
            request.RefreshExcludes.Add("PayloadProfile");
            request.RefreshExcludes.Add("Route");
            var task = SlideMapModule.Executor.Submit<GetObjectListResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                //logger.LogException(task.Exception);
            }
            if (task.Value == null)
            {
                throw new Exception("Could not retrieve list of vehicles");
            }
            var list = task.Value;
            var vehicle = list.Objects.FirstOrDefault(x => string.Compare(vehicleName, x.Vehicle.Name, StringComparison.OrdinalIgnoreCase) == 0).Vehicle;
            return vehicle;
        }

        public string GetPayloadProfileId(string cameraName)
        {
            MessageFuture<GetObjectListResponse> listFuture = SlideMapModule.Executor.Submit<GetObjectListResponse>(
                new GetObjectListRequest
                {
                    ClientId = SlideMapModule.ClientId,
                    ObjectType = "PayloadProfile"
                });
            List<DomainObjectWrapper> listResp = listFuture.Value.Objects;
            foreach (DomainObjectWrapper obj in listResp)
            {
                if (obj.PayloadProfile.Name == cameraName)
                {
                    return obj.PayloadProfile.Id.ToString();
                }
            }
            return null;
        }

        public List<Waypoint> GetRouteWaypoints(ProcessedRoute processedRoute)
        {
            List<Waypoint> waypoints = new List<Waypoint>();
            ProcessedSegment processedSegment = processedRoute.Segments.FirstOrDefault();
            List<UGCS.Sdk.Protocol.Encoding.Action> actions = processedSegment.SegmentActions;
            foreach (UGCS.Sdk.Protocol.Encoding.Action action in actions)
            {
                if (action.Waypoint != null)
                {
                    waypoints.Add(action.Waypoint);
                }
            }
            if (waypoints.Count == 0) throw new Exception("Error. List of waypoints empty."); 
            return waypoints;
        }

        public List<Vehicle> GetVehicles()
        {
            GetObjectListRequest getObjectListRequestVehicle = new GetObjectListRequest()
            {
                ClientId = SlideMapModule.ClientId,
                ObjectType = "Vehicle",
                RefreshDependencies = true
            };
            getObjectListRequestVehicle.RefreshExcludes.Add("PayloadProfile");
            getObjectListRequestVehicle.RefreshExcludes.Add("Route");
            var taskVehicle = SlideMapModule.Executor.Submit<GetObjectListResponse>(getObjectListRequestVehicle);
            taskVehicle.Wait();
            List<Vehicle> listVehicles = new List<Vehicle>();
            foreach (DomainObjectWrapper obj in taskVehicle.Value.Objects)
            {
                var payloadProfiles = obj.Vehicle.Profile.PayloadProfiles;
                foreach (VehicleProfilePayloadProfile vehProfPaylProf in payloadProfiles)
                {
                    var payloadProfile = vehProfPaylProf.PayloadProfile;
                    if (payloadProfile != null && payloadProfile.PayloadType == PayloadType.PT_CAMERA)
                    {
                        if (!listVehicles.Contains(obj.Vehicle))
                        {
                            listVehicles.Add(obj.Vehicle);
                        }
                    }
                }
            }
            if (listVehicles.Count == 0) throw new Exception("No vehicle with payload profile of type camera found. Please add a vehicle.");
            return listVehicles;
        }
    }
}
