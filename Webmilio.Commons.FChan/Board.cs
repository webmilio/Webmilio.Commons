namespace Webmilio.Commons.FChan
{
    public class Board
    {
        public override string ToString()
        {
            return $"{title} ({board})";
        }


        public string board { get; set; }
        public string title { get; set; }

        public int ws_board { get; set; }
        public uint per_page { get; set; }
        public uint pages { get; set; }
        
        public ulong max_filesize { get; set; }
        public ulong max_web_filesize { get; set; }
        public int max_comment_chars { get; set; }
        public int max_webm_duration { get; set; }

        public int bump_limit { get; set; }
        public int image_limit { get; set; }

        public string meta_description { get; set; }

        public byte is_archived { get; set; }
        public bool IsArchived => is_archived > 0;

        public byte troll_flags { get; set; }
        public bool TrollFlags => troll_flags > 0;

        public byte spoilers { get; set; }
        public bool Spoilers => spoilers > 0;

        public byte custom_spoilers { get; set; }
        public bool CustomSpoilers => custom_spoilers > 0;

        public byte country_flags { get; set; }
        public bool CountryFlags => country_flags > 0;

        public byte user_ids { get; set; }
        public bool UserIds => user_ids > 0;

        public byte oekaki { get; set; }
        public bool Oekaki => oekaki > 0;

        public byte sjis_tags { get; set; }
        public bool SJISTags => sjis_tags > 0;

        public byte code_tags { get; set; }
        public bool CodeTags => code_tags > 0;

        public byte math_tags { get; set; }
        public bool MathTags => math_tags > 0;

        public byte text_only { get; set; }
        public bool TextOnly => text_only > 0;

        public byte forced_anon { get; set; }
        public bool ForcedAnon => forced_anon > 0;

        public byte webm_audio { get; set; }
        public bool WebMAudio => webm_audio > 0;

        public byte require_subject { get; set; }
        public bool RequireSubject => require_subject > 0;

        public uint min_image_width { get; set; }
        public uint min_image_height { get; set; }


        public class Cooldown
        {
            public int threads { get; set; }
            public int replies { get; set; }
            public int images { get; set; }
        }


        public class Response
        {
            public Board[] boards { get; set; }
        }
    }
}