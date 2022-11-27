using ArcGIS.Desktop.Framework.Controls;
using SlideMap.ViewModels;
using System.Windows;

namespace SlideMap

{
    public partial class ConnectToServerDialog : ProWindow
    {
        
        public ConnectToServerDialog()
        {
            InitializeComponent();
        }

        private void Button_Connect(object sender, RoutedEventArgs e)
        {
            SlideMapModule.UgCSServer = serverTextBox.Text;
            SlideMapModule.UserName = usernameTextBox.Text;
            SlideMapModule.Password = passwordBox.Password;
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            SlideMapModule.UgCSServer = string.Empty;
            SlideMapModule.UserName = string.Empty;
            SlideMapModule.Password = string.Empty;
            this.DialogResult = false;
            this.Close();
        }
    }
}
