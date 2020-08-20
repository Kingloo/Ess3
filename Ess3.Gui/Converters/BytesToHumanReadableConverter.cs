using System;
using System.Globalization;
using System.Windows.Data;

namespace Ess3.Gui.Converters
{
    [ValueConversion(typeof(Int64), typeof(string))]
    public class BytesToHumanReadableConverter : IValueConverter
    {
        private const decimal oneKiB = 1024m;
        private const decimal oneMiB = oneKiB * oneKiB;
        private const decimal oneGiB = oneKiB * oneMiB;
        private const decimal oneTiB = oneKiB * oneGiB;
        private const decimal onePiB = oneKiB * oneTiB;
        private const decimal oneEiB = oneKiB * onePiB;
        private const decimal oneZiB = oneKiB * oneEiB;
        private const decimal oneYiB = oneKiB * oneZiB;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cc = CultureInfo.CurrentCulture;

            if (value is Int64 bytesLong)
            {
                decimal bytes = System.Convert.ToDecimal(bytesLong);

                return bytes switch
                {
                    _ when bytes < oneKiB => string.Format(cc, "{0} bytes", bytes),
                    _ when bytes < oneMiB => string.Format(cc, "{0:0.##} KiB", bytes / oneKiB),
                    _ when bytes < oneGiB => string.Format(cc, "{0:0.##} MiB", bytes / oneMiB),
                    _ when bytes < oneTiB => string.Format(cc, "{0:0.###} GiB", bytes / oneGiB),
                    _ when bytes < onePiB => string.Format(cc, "{0:0.###} TiB", bytes / oneTiB),
                    _ when bytes < oneEiB => string.Format(cc, "{0:0.###} PiB", bytes / onePiB),
                    _ when bytes < oneZiB => string.Format(cc, "{0:0.###} EiB", bytes / oneEiB),
                    _ when bytes < oneYiB => string.Format(cc, "{0:0.###} ZiB", bytes / oneZiB),
                    _ => string.Format(cc, "{0} YiB", bytes / oneYiB)
                };
            }
            else
            {
                throw new ArgumentException($"method requires {typeof(Int64)}, you provided {value.GetType()}", nameof(value));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
