using System;

namespace Webmilio.Commons.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DecimalPrecisionAttribute : AnnotationAttribute
    {
        public DecimalPrecisionAttribute(int precision, int scale)
        {
            Precision = precision;
            Scale = scale;

            PropertyMetadatas = new PropertyMetadatas
            {
                Precision = Precision,
                Scale = Scale
            };
        }


        public int Precision { get; set; }
        public int Scale { get; set; }
    }
}