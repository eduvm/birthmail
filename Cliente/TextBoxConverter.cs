using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cliente {
    class TextBoxConverter : IValueConverter {



        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            int val = (int)(double)value - 80;

            return val;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
