using ArcGIS.Core.Geometry;
using SlideMap.Enum;
using SlideMap.Helpers;
using SlideMap.ViewModels;
using System;
using System.Linq;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Services
{
    public class RouteService
    {
        private UgCSHelpers _ugcsHelpers;
        private MappingRequestService _mappingRequestService;

        public RouteService()
        {
            _mappingRequestService = new MappingRequestService();
            _ugcsHelpers = new UgCSHelpers();
        }

        public Route SetTakeoffPoint(Route route, Polygon polygon, int index)
        {
            TraverseAlgorithm wpAlgorithm = _mappingRequestService.GetAlgoritmByClassName("com.ugcs.ucs.service.routing.impl.TakeOffAlgorithm");
            SegmentDefinition newSegment = new SegmentDefinition
            {
                Uuid = Guid.NewGuid().ToString(),
                AlgorithmClassName = wpAlgorithm.ImplementationClass
            };
            newSegment.Figure = new Figure { Type = wpAlgorithm.FigureType };

            newSegment.ParameterValues.Add(new ParameterValue()
            {
                Name = "speed",
                NameSpecified = true,
                Value = "7.0",
                ValueSpecified = true,
            });
            newSegment.ParameterValues.Add(new ParameterValue()
            {
                Name = "avoidObstacles",
                NameSpecified = true,
                Value = "True",
                ValueSpecified = true
            });
            newSegment.ParameterValues.Add(new ParameterValue()
            {
                Name = "avoidTerrain",
                NameSpecified = true,
                Value = "True",
                ValueSpecified = true
            });


            var point = polygon.Points.First();
            newSegment.Figure.Points.Add(new FigurePoint()
            {
                AglAltitude = 0.0,
                AglAltitudeSpecified = true,
                AltitudeType = AltitudeType.AT_AGL,
                AltitudeTypeSpecified = true,
                Latitude = point.Y.ToRadians(), //0.99443566874164979, -90 --- +90
                LatitudeSpecified = true,
                Longitude = point.X.ToRadians(), //0.42015588448045021, -180 --- +180
                LongitudeSpecified = true
            });


            route.Segments.Add(newSegment);
            return SaveUpdatedRoute(route);

        }

        public Route AddWaypoint(Route route, Polygon polygon, CreateMissionDialogViewModel viewModel)
        {
            TraverseAlgorithm wpAlgorithm = _mappingRequestService.GetAlgoritmByClassName("com.ugcs.ucs.service.routing.impl.PhotogrammetryAlgorithm");
            SegmentDefinition segment = new SegmentDefinition
            {
                Uuid = Guid.NewGuid().ToString(),
                AlgorithmClassName = wpAlgorithm.ImplementationClass
            };
            segment.Figure = new Figure { Type = wpAlgorithm.FigureType };

            var cameraName = viewModel.SelectedCameraProfile?.Name.Trim();

            segment = segment.AddParameter("speed", viewModel.FlightSpeed)
                   .AddParameter("wpTurnType", viewModel.TurnType == TurnTypeEnum.StopAndTurn ? "STOP_AND_TURN" : "SPLINE")
                   .AddParameter("camera", _ugcsHelpers.GetPayloadProfileId(cameraName))
                   .AddParameter("groundSampleDistance", viewModel.GroundResolution)
                   .AddParameter("overlapForward", viewModel.ForwardOverlap)
                   .AddParameter("overlapSide", viewModel.SideOverlap)
                   .AddParameter("camTopFacingForward", "True")
                   .AddParameter("directionAngle", viewModel.DirectionAngle)
                   .AddParameter("avoidObstacles", "True")
                   .AddParameter("actionExecution", "ACTIONS_ON_FORWARD_PASSES")
                   .AddParameter("generateAdditionalWaypoints", "False")
                   .AddParameter("overshoot", string.Empty)
                   .AddParameter("overshootSpeed", string.Empty)
                   .AddParameter("altitudeType", viewModel.AltitudeMode == AltitudeModeEnum.AGL ? "AGL" : "AMSL")
                   .AddParameter("areaScanAllowPartialCalculation", "False")
                   .AddParameter("tolerance", viewModel.AGLTolerance)
                   .AddParameter("noActionsAtLastPoint", "True")
                   .AddParameter("doubleGrid", "False");

            if (polygon != null)
            {
                foreach (var point in polygon.Points)
                {
                    segment.Figure.Points.Add(new FigurePoint()
                    {
                        AglAltitude = 0.0,
                        AglAltitudeSpecified = true,
                        AltitudeType = AltitudeType.AT_AGL,
                        AltitudeTypeSpecified = true,
                        Latitude = point.Y.ToRadians(),
                        LatitudeSpecified = true,
                        Longitude = point.X.ToRadians(),
                        LongitudeSpecified = true
                    });
                }

                route.Segments.Add(segment);
                return SaveUpdatedRoute(route);

            } else throw new Exception("Geometry type of feature layer must be polygon. Please try again.");
            
        }

        public Route CreateNewRoute(Mission mission, VehicleProfile vehicleProfile, string routeName)
        {
            Route route = new Route
            {
                CreationTime = ServiceHelpers.CreationTime(),
                Name = routeName,
                Mission = mission
            };

            ChangeRouteVehicleProfileRequest request = new ChangeRouteVehicleProfileRequest
            {
                ClientId = SlideMapModule.ClientId,
                Route = route,
                NewProfile = new VehicleProfile { Id = vehicleProfile.Id }
            };
            MessageFuture<ChangeRouteVehicleProfileResponse> future =
                SlideMapModule.Executor.Submit<ChangeRouteVehicleProfileResponse>(request);
            future.Wait();
            route = future.Value.Route;
            route.Mission = mission;
            route.TrajectoryTypeSpecified = false;

            route.MaxAltitude = 120.0;
            route.MaxAltitudeSpecified = true;

            route.SafeAltitude = 50.0;
            route.SafeAltitudeSpecified = true;

            route.CheckAerodromeNfz = false;
            route.CheckAerodromeNfzSpecified = true;

            route.InitialSpeed = 0.0;
            route.InitialSpeedSpecified = true;

            route.MaxSpeed = 0.0;
            route.MaxSpeedSpecified = true;

            route.TakeoffHeight = 0.0;
            route.TakeoffHeightSpecified = true;

            route.CheckCustomNfz = false;
            route.CheckCustomNfzSpecified = true;

            route.Failsafes.Add(new Failsafe()
            {
                Action = FailsafeAction.FA_GO_HOME,
                ActionSpecified = true,
                Reason = FailsafeReason.FR_RC_LOST,
                ReasonSpecified = true,
            });
            route.Failsafes.Add(new Failsafe()
            {
                Action = FailsafeAction.FA_LAND,
                ActionSpecified = true,
                Reason = FailsafeReason.FR_LOW_BATTERY,
                ReasonSpecified = true
            });
            route.Failsafes.Add(new Failsafe()
            {
                Action = FailsafeAction.FA_WAIT,
                ActionSpecified = true,
                Reason = FailsafeReason.FR_GPS_LOST,
                ReasonSpecified = true
            });
            route.Failsafes.Add(new Failsafe()
            {
                Action = FailsafeAction.FA_GO_HOME,
                ActionSpecified = true,
                Reason = FailsafeReason.FR_DATALINK_LOST,
                ReasonSpecified = true
            });

            return SaveUpdatedRoute(route);
        }

        /// <summary>
        /// Examp,e how create or update route on server
        /// </summary>
        /// <param name="route">modified route</param>
        /// <returns>Updated route from server</returns>
        public Route SaveUpdatedRoute(Route route)
        {
            CreateOrUpdateObjectRequest request = new CreateOrUpdateObjectRequest()
            {
                ClientId = SlideMapModule.ClientId,
                Object = new DomainObjectWrapper().Put(route, "Route"),
                WithComposites = true,
                ObjectType = "Route",
                AcquireLock = false
            };
            var task = SlideMapModule.Executor.Submit<CreateOrUpdateObjectResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                //logger.LogException(task.Exception);
                throw new Exception("Save error: " + task.Exception.Message);
            }

            if (task.Value == null)
            {
                //logger.LogWarningMessage("Could not save route info: " + route.Name);
                throw new Exception("Could not save route info: " + route.Name);
            }

            return task.Value.Object.Route;
        }

        /// <summary>
        /// Example how update route by id
        /// </summary>
        /// <param name="routeId">possible route id</param>
        /// <returns>Updated route by id</returns>
        public Route GetUpdatedRouteById(int routeId)
        {
            GetObjectRequest request = new GetObjectRequest()
            {
                ClientId = SlideMapModule.ClientId,
                ObjectId = routeId,
                ObjectType = "Route",
                RefreshDependencies = true
            };

            request.RefreshExcludes.Add("Vehicle");
            request.RefreshExcludes.Add("PayloadProfile");
            request.RefreshExcludes.Add("Mission");

            var task = SlideMapModule.Executor.Submit<GetObjectResponse>(request);
            task.Wait();


            if (task.Value == null)
            {
                throw new Exception("Could not retrieve route info: " + routeId);
            }

            return task.Value.Object.Route;
        }

        /// <summary>
        /// Example how to calculate route by id
        /// </summary>
        /// <param name="routeId">route ID</param>
        /// <returns>Processed route</returns>
        public ProcessedRoute CalculateRouteById(int routeId)
        {
            var updatedRoute = GetUpdatedRouteById(routeId);
            ProcessRouteRequest request = new ProcessRouteRequest
            {
                ClientId = SlideMapModule.ClientId,
                Route = updatedRoute,
            };

            MessageFuture<ProcessRouteResponse> task = SlideMapModule.Executor.Submit<ProcessRouteResponse>(request);
            task.Wait();

            if (task.Exception != null || task.Value == null)
            {
                //logger.LogException(task.Exception);
                throw new Exception("Calculate error: " + task.Exception.Message);
            }
            return task.Value.ProcessedRoute;
        }

        /// <summary>
        /// Example how to calculate route by id
        /// </summary>
        /// <param name="route"></param>
        /// <returns>Processed route</returns>
        public ProcessedRoute CalculateRoute(Route route)
        {
            if (route == null)
            {
                throw new Exception("Route not specified");
            }
            var updatedRoute = GetUpdatedRouteById(route.Id);
            ProcessRouteRequest request = new ProcessRouteRequest
            {
                ClientId = SlideMapModule.ClientId,
                Route = updatedRoute,
            };

            MessageFuture<ProcessRouteResponse> task = SlideMapModule.Executor.Submit<ProcessRouteResponse>(request);
            task.Wait();

            if (task.Exception != null || task.Value == null)
            {
                //logger.LogException(task.Exception);
                throw new Exception("Calculate error: " + task.Exception.Message);
            }
            return task.Value.ProcessedRoute;
        }
    }
}

