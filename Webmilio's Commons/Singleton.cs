namespace Webmilio.Commons
{
    // Taken from Magicka.
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static volatile object _lock = new object();


        private Singleton() { }


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