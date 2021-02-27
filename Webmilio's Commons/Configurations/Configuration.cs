using System;
using System.IO;
using System.Threading.Tasks;

namespace Webmilio.Commons.Configurations
{
    public abstract class Configuration<T> where T : Configuration<T>
    {
        protected Configuration()
        {
        }


        public abstract Task Save(string path);
    }
}