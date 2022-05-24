using System;
using System.Threading.Tasks;

namespace Scool.Notification
{
    public interface INoticationHubClient
    {
        Task ReceiveNotification(string message);
    }
}
