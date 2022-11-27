using System;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Interfaces
{
    public interface IConnect
    {
        AuthorizeHciResponse AuthorizeHciResponse { get; set; }
        LoginResponse LoginResponce { get; set; }
        MessageExecutor Executor { get; }
        Task ConnectUgcs(String login, String password, System.Action onConnect);
    }
}