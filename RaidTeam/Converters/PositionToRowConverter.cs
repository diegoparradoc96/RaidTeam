using Microsoft.UI.Xaml.Data;
using System;

namespace RaidTeam.Converters
{
    public class PositionToRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int position)
            {
                return position / 3;  // Divide por el número de columnas (3) para obtener la fila (0 o 1)
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}