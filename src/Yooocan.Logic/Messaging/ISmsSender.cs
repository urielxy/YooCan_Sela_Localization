using System.Threading.Tasks;

namespace Yooocan.Logic.Messaging
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
