using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace PTR
{

    public class StringToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                bool blnDate = DateTime.TryParse(value.ToString(), out DateTime enteredDate);
                if (blnDate)
                    return enteredDate;
                else
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month,1);
            }
            else
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    public class StringToIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                bool blnInteger = int.TryParse(value.ToString(), out int enteredInteger);
                if (blnInteger)
                    return enteredInteger;
                else
                    return 0;
            }
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                bool blnDecimal = decimal.TryParse(value.ToString(), out decimal enteredDecimal);
                if (blnDecimal)
                    return enteredDecimal;
                else
                    return 0.0;
            }
            else
                return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    //public class CultureFormatConverter : IMultiValueConverter
    //{
    //    private CultureInfo cultinfo;
    //    private string cult;

    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {         
    //        bool bln = decimal.TryParse(values[0].ToString(), out decimal dvalue);
    //        cult = values[1] as string; //culture                      
    //        if (cult.ToString() == string.Empty)
    //            cult = "en-US";

    //        cultinfo = new CultureInfo(cult);
    //        return string.Format(cultinfo, "{0:C0}", dvalue);                        
    //    }
    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        bool bln = decimal.TryParse(value.ToString(), NumberStyles.Currency, cultinfo, out decimal dd);            
    //        object[] ret = new object[2];
    //        ret[0] = dd;
    //        ret[1] = cult;
    //        return ret;            
    //    }
    //}

    //public class CurrencyCheckConverter : IValueConverter
    //{
    //    private CultureInfo cultinfo;
    //    private readonly string cult = "en-US";

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (!string.IsNullOrEmpty(value.ToString()))
    //        {
    //            cultinfo = new CultureInfo(cult);
    //            bool blnDecimal = decimal.TryParse(value.ToString(), NumberStyles.Currency, cultinfo, out decimal enteredDecimal);
    //            if (blnDecimal)
    //                return enteredDecimal;
    //            else
    //                return 0;
    //        }
    //        else
    //            return 0;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (!string.IsNullOrEmpty(value.ToString()))
    //        {
    //            cultinfo = new CultureInfo(cult);
    //            bool blnDecimal = decimal.TryParse(value.ToString(), NumberStyles.Currency, cultinfo, out decimal enteredDecimal);
    //            if (blnDecimal)
    //                return enteredDecimal;
    //            else
    //                return 0;
    //        }
    //        else
    //            return 0;
    //    }
    //}

    //public class IntCheckConverter : IValueConverter
    //{
    //    private CultureInfo cultinfo;
    //    private readonly string cult = "en-US";

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (!string.IsNullOrEmpty(value.ToString()))
    //        {
    //            cultinfo = new CultureInfo(cult);
    //            bool blnInt = int.TryParse(value.ToString(), NumberStyles.AllowThousands, cultinfo, out int enteredInt);
    //            if (blnInt)
    //                return enteredInt;
    //            else
    //                return 0;
    //        }
    //        else
    //            return 0;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (!string.IsNullOrEmpty(value.ToString()))
    //        {
    //            cultinfo = new CultureInfo(cult);
    //            bool blnInt = int.TryParse(value.ToString(), NumberStyles.AllowThousands, cultinfo, out int enteredInt);
    //            if (blnInt)
    //                return enteredInt;
    //            else
    //                return 0;
    //        }
    //        else
    //            return 0;
    //    }
    //}

    public class DecCheckConverter : IValueConverter
    {
        private CultureInfo cultinfo;
        private readonly string cult = "en-US";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                cultinfo = new CultureInfo(cult);
                bool blnDec = decimal.TryParse(value.ToString(), NumberStyles.Number, cultinfo, out decimal enteredDec);
                if (blnDec)
                    return enteredDec;
                else
                    return 0;
            }
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                cultinfo = new CultureInfo(cult);
                bool blnDec = decimal.TryParse(value.ToString(), NumberStyles.Number, cultinfo, out decimal enteredDec);
                if (blnDec)
                    return enteredDec;
                else
                    return 0;
            }
            else
                return 0;
        }
    }

    //public class MultiValueEqualityConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (values != null)
    //        {
    //            if (!string.IsNullOrEmpty(values[0].ToString()) && !string.IsNullOrEmpty(values[1].ToString()))                
    //                return values[0].ToString() == values[1].ToString();                
    //            else
    //                return false;
    //        }
    //        else
    //            return false;           
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class CurCheckConverter : IValueConverter
    {
        private CultureInfo cultinfo;
        private readonly string cult = "en-US";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                cultinfo = new CultureInfo(cult);
                bool blnInt = int.TryParse(value.ToString(), NumberStyles.Currency, cultinfo, out int enteredInt);
                if (blnInt)
                    return enteredInt;
                else
                    return 0;
            }
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                cultinfo = new CultureInfo(cult);
                bool blnInt = int.TryParse(value.ToString(), NumberStyles.Currency, cultinfo, out int enteredInt);
                if (blnInt)
                    return enteredInt;
                else
                    return 0;
            }
            else
                return 0;
        }
    }

}

