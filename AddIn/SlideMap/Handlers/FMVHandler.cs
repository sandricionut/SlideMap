using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideMap.Handlers
{
    internal class FMVHandler : Button
    {
        protected override void OnClick()
        {
            try
            {
                string installPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string toolboxPath = Path.Combine(installPath, "Pytools\\LandslidesAI.pyt\\GenerateFMV");

                Geoprocessing.OpenToolDialog(toolboxPath, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Error on open PYT: {ex.Message}");
            }
        }
    }
}
