using System.Threading.Tasks;

namespace Webmilio.Commons.Configurations;

public abstract class Configuration
{
    protected Configuration()
    {
    }


    public abstract Task Save(string path);
}