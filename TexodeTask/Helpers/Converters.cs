using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TexodeTask.Helpers
{
    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 0 ? "Жен." : "Муж.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "Жен." ? 1 : 0;
        }
    }

    public class AgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int age = (int)value;
            string s_age = age.ToString();
            string postfix = String.Empty;
            switch(s_age[s_age.Length-1])
            {
                //По условию ТЗ возраст должен быть в промежутке [16, 100], но в кейсах проверку
                //сделал для всех случаев, так сказать, для полноты картины.
                case '1':
                    {
                        postfix = s_age.EndsWith("11") ? " лет" : " год";
                        break;
                    }
                case '2':
                    {
                        postfix = s_age.EndsWith("12") ? " лет" : " года";
                        break;
                    }
                case '3':
                    {
                        postfix = s_age.EndsWith("13") ? " лет" : " года";
                        break;
                    }
                case '4':
                    {
                        postfix = s_age.EndsWith("14") ? " лет" : " года";
                        break;
                    }
                //для значений, заканчивающихся нулём или цифрой от 5 до 9:
                default:
                    {
                        postfix = " лет";
                        break;
                    }
            }
            return s_age + postfix;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return uint.Parse(((string)value).Split(' ')[0]);
        }
    }

    public class SelectionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int)
                return (int)value == 1;
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StudentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
                return (int)value > 0;
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
