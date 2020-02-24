namespace Webmilio.Commons.Networking
{
    public abstract class Packet
    {
        protected Packet()
        {
        }


        public int Id => Singleton<PacketLoader>.Instance.Id(GetType());
    }
}