using System.Threading.Tasks;

namespace Yooocan.Logic.Recaptchas
{
    public interface IRecaptchaApi
    {
        Task<bool> ValidateAsync(string clientIp, string responseToken);
    }
}