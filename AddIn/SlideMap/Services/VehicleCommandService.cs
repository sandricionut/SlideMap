using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Services
{
    class VehicleCommandService
    {
        public SendCommandResponse SendFlightCommandToVehicle(string commandCode)
        {
            var sendCommandRequestGuided = new SendCommandRequest
            {
                ClientId = SlideMapModule.ClientId,
                Command = new Command
                {
                    Code = commandCode,
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER
                }
            };
            sendCommandRequestGuided.Vehicles.Add(SlideMapModule.CurrentVehicle);
            var sendCommandResponseGuided = SlideMapModule.Executor.Submit<SendCommandResponse>(sendCommandRequestGuided);
            sendCommandResponseGuided.Wait();
            return sendCommandResponseGuided.Value;
        }
    }
}
