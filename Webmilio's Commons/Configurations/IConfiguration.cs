using System.Threading.Tasks;

namespace Webmilio.Commons.Configurations
{
    public interface IConfiguration
    {
        Task Save(string path);
    }
}