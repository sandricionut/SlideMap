using SlideMap.Exceptions;
using SlideMap.Interfaces;
using System;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Services
{
    public class Connect : IConnect
    {
        private MessageExecutor _executor;
        private AuthorizeHciRequest _request;
        private TcpClient _tcpClient;

        public MessageExecutor Executor
        {
            get
            {
                return _executor;
            }
        }
        public AuthorizeHciResponse AuthorizeHciResponse { get; set; }
        public LoginResponse LoginResponce { get; set; }

        public Connect(MessageExecutor executor,
            AuthorizeHciRequest request,
            TcpClient tcpClient)
        {
            _executor = executor;
            _request = request;
            _tcpClient = tcpClient;
        }

        /// <summary>
        /// Async authorization in UCS server
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <param name="onConnect">on connection callback without paramaters</param>
        /// <returns></returns>
        public async Task ConnectUgcs(String login, String password, System.Action onConnect)
        {
            var task = Task<TheadExceptionModel>.Factory.StartNew(() =>
            {
                LoginRequest loginRequest = new LoginRequest();
                loginRequest.UserLogin = login;
                loginRequest.UserPassword = password;

                _request.ClientId = -1;
                var future = _executor.Submit<AuthorizeHciResponse>(_request);
                if (future.Exception != null)
                {
                    return new TheadExceptionModel() { Message = future.Exception.Message, Status = 400 };
                }
                AuthorizeHciResponse = future.Value;

                loginRequest.ClientId = AuthorizeHciResponse.ClientId;
                LoginResponce = (LoginResponse)_executor.Submit<LoginResponse>(loginRequest).Value;
                if (LoginResponce == null || LoginResponce.User == null)
                {
                    return new TheadExceptionModel() { Message = "Invalid login or password", Status = 300 };
                }
                return new TheadExceptionModel() { Message = "OK", Status = 200 };
            });
            await task.ContinueWith((result) =>
            {
                if (result.Exception != null)
                {
                    result.Exception.Data.Clear();
                    throw new ConnectionException(result.Exception.Message);
                }
                if (result.Result.Status == 200)
                {
                    onConnect();
                }
                else if (result.Result.Status == 300)
                {
                    throw new ConnectionException(result.Result.Message);
                }
                else if (result.Result.Status == 400)
                {
                    throw new ConnectionException(result.Result.Message);
                }
            }).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
