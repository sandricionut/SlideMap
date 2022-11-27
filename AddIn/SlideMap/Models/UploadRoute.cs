
using System;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Models
{
    public class UploadRoute
    {
        private Vehicle _vehicle;
        private ProcessedRoute _processedRoute;

        public void SetVehicle(Vehicle vehicle)
        {
            _vehicle = vehicle;
        }

        public Vehicle GetVehicle()
        {
            return _vehicle;
        }

        public void SetProcessedRoute(ProcessedRoute route)
        {
            //De verificat daca ruta este procesata
            _processedRoute = route;
        }

        public bool Upload()
        {
            if (_processedRoute == null)
            {
                throw new Exception("Route not specified");
            }
            if (_vehicle == null)
            {
                throw new Exception("Vehicle not specified");
            }
            UploadRouteRequest request = new UploadRouteRequest
            {
                ClientId = SlideMapModule.ClientId,
                ProcessedRoute = _processedRoute,
                Vehicle = _vehicle,
            };

            MessageFuture<UploadRouteResponse> task = SlideMapModule.Executor.Submit<UploadRouteResponse>(request);
            task.Wait();

            if (task.Exception != null || task.Value == null)
            {
                throw new Exception("Upload error: " + task.Exception.Message);
            }
            return true;
        }



    }
}
