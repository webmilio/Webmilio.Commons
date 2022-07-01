namespace Tests.Webmilio.Commons
{
    public class TestClasses
    {
        public class SmallClass : SmallClassBase
        {
            public string String1 { get; set; } = "test 123";
            public string String2 { get; set; } = "123 test";
            public string String3 { get; set; }

            public int Int1 { get; set; } = 123;
            public int Int2 { get; set; } = 321;
            public int Int3 { get; set; }
        }

        public class SmallClass2 : SmallClassBase
        {
            public string String1 { get; set; } = "test 123";
            public string String3 { get; set; }

            public bool Int1 { get; set; }
            public int Int2 { get; set; } = 321;
            public int Int3 { get; set; }
        }

        public abstract class SmallClassBase
        {
            
        }
    }

    public abstract class X { }
    public class Y : X { }
    public class Z : X { }
}