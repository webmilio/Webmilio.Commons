namespace Tests.Webmilio.Commons
{
    public class TestClasses
    {
        public class SmallClass
        {
            public string String1 { get; set; } = "test 123";
            public string String2 { get; set; } = "123 test";
            public string String3 { get; set; }

            public int Int1 { get; set; } = 123;
            public int Int2 { get; set; } = 321;
            public int Int3 { get; set; }
        }

        public class SmallClass2
        {
            public string String1 { get; set; } = "test 123";
            public string String3 { get; set; }

            public bool Int1 { get; set; }
            public int Int2 { get; set; } = 321;
            public int Int3 { get; set; }
        }
    }
}