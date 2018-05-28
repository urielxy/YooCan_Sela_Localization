using System.Threading.Tasks;

namespace Alto.Logic.Messaging
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
