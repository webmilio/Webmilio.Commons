using System;
using System.Threading.Tasks;

namespace Webmilio.Commons
{
    // Taken from Magicka.
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static volatile object _lock = new object();


        private Singleton() { }


        public static void Lose()
        {
            lock (_lock)
            {
                if (!HasInstance)
                    return;

                _instance = null;
            }
        }


        public static async ValueTask DisposeAsync()
        {
            if (_instance is IAsyncDisposable disposable)
                await disposable.DisposeAsync();
        }


        public static void Dispose()
        {
            lock (_lock)
            {
                if (_instance is IDisposable disposable)
                    disposable.Dispose();
            }
        }


        public static bool HasInstance => _instance != default;

        public static T Instance
        {
            get
            {
                if (HasInstance)
                    return _instance;

                lock (_lock)
                    _instance = new T();

                return _instance;
            }
        }
    }
}