using Microsoft.UI.Xaml.Data;
using System;

namespace RaidTeam.Converters
{
    public class PositionToColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int position)
            {
                return position % 3;  // Módulo 3 para obtener la columna (0, 1 o 2)
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}