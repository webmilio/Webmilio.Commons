namespace Webmilio.Commons.Units
{
    public struct Mass
    {
        public const double
            KgToMg = 1000,
            KgToLb = 2.20462262185,
            
            LbToOz = 16;


        public Mass(double milligrams)
        {
            Milligrams = milligrams;
            Kilograms = milligrams / KgToMg;

            Pounds = Kilograms * KgToLb;
            Ounces = Pounds * 16;
        }


        public static Mass FromKilograms(double kilograms) => FromMilligrams(kilograms * KgToMg);
        public static Mass FromMilligrams(double milligrams) => new(milligrams);

        public static Mass FromPounds(double pounds) => FromKilograms(pounds / KgToLb);
        public static Mass FromOunces(double ounces) => FromPounds(ounces / LbToOz);


        public double Kilograms { get; set; }
        public double Milligrams { get; set; }

        public double Pounds { get; set; }
        public double Ounces { get; set; }
    }
}