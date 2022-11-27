using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using SlideMap.Services;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Handlers
{
    internal class ContinueVehicleHandler : Button
    {

        protected override void OnClick()
        {
            if (SlideMapModule.Executor == null)
            {
                MessageBox.Show("No execution context could be found. Please login to UgCS server.");
                return;
            }

            if (SlideMapModule.CurrentVehicle == null)
            {
                MessageBox.Show("No route uploaded to a vehicle. Please upload a route.");
                return;
            }
            var vehicleCommandService = new VehicleCommandService();
            SendCommandResponse response = vehicleCommandService.SendFlightCommandToVehicle("mission_resume");
            if(response.CommandResults[0].CommandStatus== CommandStatus.CS_SUCCEEDED)
            {
                MessageBox.Show("Vehicle will resume mission.");
            } else if (response.CommandResults[0].ErrorMessageSpecified)
            {
                MessageBox.Show(string.Format("Error while sending command. Error message: {0}",
                    response.CommandResults[0].ErrorMessage));
            } else
            {
                MessageBox.Show("Command wasn't sent successfully.");
            }
        }

    }
}
