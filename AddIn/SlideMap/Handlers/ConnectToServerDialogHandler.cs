using System;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Tasks;

namespace SlideMap.Handlers
{
    internal class ConnectToServerDialogHandler : Button
    {
        protected override void OnClick()
        {
            try
            {
                ConnectToServerDialog authenticationDialog = new ConnectToServerDialog
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };
                bool? dialogResult = authenticationDialog.ShowDialog();
                if (SlideMapModule.HasCredentials == false) return;

                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(SlideMapModule.UgCSServer, 3334);
                MessageSender messageSender = new MessageSender(tcpClient.Session);
                MessageReceiver messageReceiver = new MessageReceiver(tcpClient.Session);
                MessageExecutor messageExecutor = new MessageExecutor(messageSender, messageReceiver, new InstantTaskScheduler());
                messageExecutor.Configuration.DefaultTimeout = 10000;
                var notificationListener = new NotificationListener();
                messageReceiver.AddListener(-1, notificationListener);

                SlideMapModule.Executor = messageExecutor;

                AuthorizeHciRequest request = new AuthorizeHciRequest();
                request.ClientId = -1;
                request.Locale = "en-US";
                var future = SlideMapModule.Executor.Submit<AuthorizeHciResponse>(request);
                future.Wait();
                AuthorizeHciResponse AuthorizeHciResponse = future.Value;
                SlideMapModule.ClientId = AuthorizeHciResponse.ClientId;

                LoginRequest loginRequest = new LoginRequest();
                loginRequest.UserLogin = SlideMapModule.UserName;
                loginRequest.UserPassword = SlideMapModule.Password;
                loginRequest.ClientId = SlideMapModule.ClientId;

                SlideMapModule.UgCSServer = string.Empty;
                SlideMapModule.UserName = string.Empty;
                SlideMapModule.Password = string.Empty;

                var loginResponseTask = SlideMapModule.Executor.Submit<LoginResponse>(loginRequest);
                var result = loginResponseTask.Wait();
                var loginResponse = result.Value as LoginResponse;
                SlideMapModule.User = loginResponse.User;

                MessageBox.Show($@"Login was successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Error: {ex.Message}");
            }
        }
    }
}