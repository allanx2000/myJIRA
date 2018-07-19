using myJIRA.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace myJIRA.UserControls
{
    class AuxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            JIRAItem jira = value as JIRAItem;

            if (jira == null)
                return null;

            AuxFields aux = (AuxFields) Enum.Parse(typeof(AuxFields), (string) parameter);

            return jira.GetAuxField(aux);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
