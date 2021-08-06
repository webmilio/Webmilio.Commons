using System;
using System.Text.Json.Serialization;

namespace Webmilio.Commons.FChan
{
    public class Post
    {

        public bool HasAttachment => ext != default;
        public Uri AttachmentUrl => new Uri($"https://i.4cdn.org/{Board}/{tim}{ext}");


        [JsonIgnore]
        public string Board { get; set; }

        [JsonIgnore]
        public ulong Thread { get; set; }


        public ulong no { get; set; }

        public ulong resto { get; set; }

        public byte sticky { get; set; }

        public byte closed { get; set; }

        public string now { get; set; }

        public ulong time { get; set; }

        public string name { get; set; }

        public string trip { get; set; }

        public string id { get; set; }

        public string capcode { get; set; }

        public string country_name { get; set; }

        public string sub { get; set; }
        public string com { get; set; }
        public ulong? tim { get; set; }

        public string filename { get; set; }
        public string ext { get; set; }
        public ulong fsize { get; set; }
        public string md5 { get; set; }

        public ulong w { get; set; }
        public ulong h { get; set; }
        public ulong tn_w { get; set; }
        public ulong tn_h { get; set; }
        public byte filedeleted { get; set; }

        public byte spoiler { get; set; }
        public byte custom_spoiler { get; set; }

        public ulong replies { get; set; }
        public ulong images { get; set; }

        public byte bumplimit { get; set; }
        public byte imagelimit { get; set; }

        public string tag { get; set; }
        public string semantic_url { get; set; }

        public short since4pass { get; set; }
        public ulong unique_ips { get; set; }
        public byte m_img { get; set; }

        public byte archived { get; set; }
        public ulong archived_on { get; set; }


        public class Response
        {
            public Post[] posts { get; set; }
        }
    }
}