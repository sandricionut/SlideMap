using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using SlideMap.Enum;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap
{
    internal class SlideMapModule : Module
    {
        private static SlideMapModule _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static SlideMapModule Current
        {
            get
            {
                return _this ?? (_this = (SlideMapModule)FrameworkApplication.FindModule("SlideMap_Module"));
            }
        }

        public static User User { get; set; }
        public static int ClientId { get; set; }
        public static MessageExecutor Executor { get; set; }
        public static string UgCSServer { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static bool HasCredentials => !(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(UgCSServer));

        public static VehicleProfile SelectedVehicleProfile { get; set; }

        public static string FlightSpeed { get; set; }
        public static TurnTypeEnum TurnType { get; set; }
        public static string GroundResolution { get; set; }
        public static AltitudeModeEnum AltitudeMode { get; set; }
        public static string ForwardOverlap { get; set; }
        public static string SideOverlap { get; set; }
        public static string DirectionAngle { get; set; }
        public static string AGLTolerance { get; set; }

        public static Vehicle CurrentVehicle { get; set; }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

    }
}
