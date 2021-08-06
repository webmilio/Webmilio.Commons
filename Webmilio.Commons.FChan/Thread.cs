using System;
using System.Text.Json.Serialization;

namespace Webmilio.Commons.FChan
{
    public class Thread
    {
        [JsonIgnore]
        public string Board { get; set; }

        public ulong no { get; set; }

        public long last_modified { get; set; }
        public DateTime LastModified => new DateTime(last_modified);

        public ulong replies { get; set; }


        public class Response
        {
            public int page { get; set; }

            public Thread[] threads { get; set; }
        }
    }
}