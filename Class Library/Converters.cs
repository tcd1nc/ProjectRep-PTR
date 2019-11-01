using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Linq;

namespace PTR
{
   
    //public class MultiParameterConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //      //  if (values[1] != null)
    //      //  {                
    //           return values.Clone();
    //       // }            
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
   
    //public class SalesDivisionSelectedConverter : IMultiValueConverter
    //{

    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        string temp = values[0].ToString();
    //        bool IsNumber = int.TryParse(temp, out int ID);
                       
    //        if (IsNumber)
    //        {                
    //            try
    //            {
    //            FullyObservableCollection<Models.SalesDivisionModel> selecteddivs = values[1] as FullyObservableCollection<Models.SalesDivisionModel>;
           
    //            var description = selecteddivs.FirstOrDefault(x => x.GOM.ID == ID);
    //            if (description !=null)
    //                return true;
    //            else
    //                return false;                                  

    //            }
    //            catch
    //            {
    //                return false;
    //            }
    //        }
    //        else
    //            return false;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }        
    //}
   
    //public class TextBlockForegroundConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null || (value as bool?) == true)
    //            if ((string)parameter == "1")
    //                return Brushes.Blue;
    //            else
    //                return Brushes.Black;
    //        else
    //            return Brushes.Gray;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class StatusTextboxBackgroundConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value != null)
    //        {
    //            string strvalue = value.ToString();
    //            if (!string.IsNullOrEmpty(strvalue))                                
    //                return StaticCollections.ColorDictionary[strvalue];                
    //            else
    //                return Brushes.White;
    //        }
    //        else
    //            return Brushes.White;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class MultiValueHighlightConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        Dictionary<string, string> selectedstatuses = values[1] as Dictionary<string, string>;

    //        if (selectedstatuses.ContainsKey(values[0].ToString()))
    //            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedstatuses[values[0].ToString()]));
    //        else
    //            return null;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class CultureFormatConverter : IMultiValueConverter
    {
        private CultureInfo cultinfo;
        private string cult;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {         
            bool bln = decimal.TryParse(values[0].ToString(), out decimal dvalue);
            cult = values[1] as string; //culture                      
            if (cult.ToString() == string.Empty)
                cult = "en-US";

            cultinfo = new CultureInfo(cult);
            return string.Format(cultinfo, "{0:C0}", dvalue);                        
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {          
            bool bln = decimal.TryParse(value.ToString(), NumberStyles.Currency, cultinfo, out decimal dd);            
            object[] ret = new object[2];
            ret[0] = dd;
            ret[1] = cult;
            return ret;            
        }
    }

}

