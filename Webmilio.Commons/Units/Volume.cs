using System;

namespace Webmilio.Commons.Units
{
    public struct Volume
    {
        public const double
            // Not Stupid
            LToMl = 1000,
            LToCups = 4,
            LToTbsp = LToMl / 15,
            TbspToTsp = 3, //  15 / 5

            // Stupid
            LToGal = 1 / 3.785d,
            GalToQt = 4,
            QtToPt = 2,
            PtToUSCup = 2,
            USCupToTbsp = 16,
            USTbspToTsp = 3,
            GalToOz = 128;


        public Volume(double milliliters)
        {
            Milliliters = milliliters;
            Liters = Milliliters / LToMl;
            Cups = Liters * LToCups;
            Tablespoons = Liters * LToTbsp;
            Teaspoons = Tablespoons * TbspToTsp;

            Gallons = Liters * LToGal;
            Quarts = Gallons * GalToQt;
            Pints = Quarts * QtToPt;
            USCups = Pints * PtToUSCup;
            USTablespoons = USCups * USCupToTbsp;
            USTeaspoons = USTablespoons * USTbspToTsp;

            Ounces = Gallons * GalToOz;
        }


        public static Volume FromLiters(double amount) => FromMilliliters(amount / LToMl);

        public static Volume FromMilliliters(double amount) => new(amount);
        public static Volume FromCups(double amount) => FromLiters(amount / LToCups);
        public static Volume FromTablespoons(double amount) => FromLiters(amount / LToTbsp);
        public static Volume FromTeaspoons(double amount) => FromTablespoons(amount / TbspToTsp);

        public static Volume FromGallons(double amount) => FromLiters(amount / LToGal);
        public static Volume FromQuarts(double amount) => FromGallons(amount / GalToQt);
        public static Volume FromPints(double amount) => FromQuarts(amount / QtToPt);
        public static Volume FromUSCups(double amount) => FromPints(amount / PtToUSCup);
        public static Volume FromUSTbsp(double amount) => FromUSCups(amount / USCupToTbsp);
        public static Volume FromUSTsp(double amount) => FromUSTbsp(amount / USTbspToTsp);
        public static Volume FromOunces(double amount) => FromGallons(amount / GalToOz);


        public double Liters { get; }
        public double Milliliters { get; }
        public double Cups { get; }
        public double Tablespoons { get; }
        public double Teaspoons { get; }

        public double Gallons { get; }
        public double Quarts { get; }
        public double Pints { get; }
        public double USCups { get; }
        public double USTablespoons { get; }
        public double USTeaspoons { get; }
        public double Ounces { get; }
    }
}