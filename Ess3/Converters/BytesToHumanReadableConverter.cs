using System;
using System.Globalization;
using System.Windows.Data;

namespace Ess3.Converters
{
    [ValueConversion(typeof(long), typeof(string))]
    public class BytesToHumanReadableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /*
             * S3 object maximum size is 5 tebibytes (ca. April 2017)
             * no need for peta, exa etc.
             * we do it anyway, cos why not?
             */

            long original = (long)value;
            decimal number = System.Convert.ToDecimal(original);

            // first order of magnitude in binary
            decimal bMag = System.Convert.ToDecimal(Math.Pow(2d, 10d));

            decimal oneKiB = bMag;
            decimal oneMiB = oneKiB * bMag;
            decimal oneGiB = oneMiB * bMag;
            decimal oneTiB = oneGiB * bMag;
            decimal onePiB = oneTiB * bMag;
            decimal oneEiB = onePiB * bMag;
            decimal oneZiB = onePiB * bMag;
            decimal oneYiB = oneZiB * bMag;

            if (number < bMag)
            {
                return string.Format(culture, "{0:0.##} bytes", number);
            }

            if (number < oneMiB)
            {
                return string.Format(culture, "{0:0.##} KiB", number / oneKiB);
            }

            if (number < oneGiB)
            {
                return string.Format(culture, "{0:0.##} MiB", number / oneMiB);
            }

            if (number < oneTiB)
            {
                return string.Format(culture, "{0:0.##} GiB", number / oneGiB);
            }

            if (number < onePiB)
            {
                return string.Format(culture, "{0:0.##} TiB", number / oneTiB);
            }

            if (number < oneEiB)
            {
                return string.Format(culture, "{0:0.##} PiB", number / onePiB);
            }

            if (number < oneZiB)
            {
                return string.Format(culture, "{0:0.##} EiB", number / oneEiB);
            }

            if (number < oneYiB)
            {
                return string.Format(culture, "{0:0.##} ZiB", number / oneZiB);
            }

            return string.Format(culture, "{0:0.##} YiB", number / oneYiB);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => -1L;
    }
}
