using System.Windows;
using System.Windows.Data;

namespace Ess3.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : BooleanConverterBase<Visibility> { }
}
