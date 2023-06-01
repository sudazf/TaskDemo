using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskDemo.Converter
{
	public class IntToSolidColorBrushConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int colorIndex = (int)value;
			SolidColorBrush brush= Brushes.Orange;
			switch (colorIndex)
			{
				case 0:
					brush = Brushes.DeepSkyBlue;
					break;
				case 1:
					brush= Brushes.Orange;
					break;
				case 2:
					brush = Brushes.Green;
					break;
				case 3:
					brush = Brushes.Brown;
					break;
				case 4:
					brush = Brushes.Pink;
					break;
				default:
					brush = Brushes.SlateBlue;
					break;
			}
			return brush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
