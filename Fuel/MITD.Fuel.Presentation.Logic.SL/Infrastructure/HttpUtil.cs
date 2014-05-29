using System;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MITD.Fuel.Presentation.Logic.SL.Infrastructure
{

    public class HttpUtil
    {
        public const string WEBAPI_DATE_PARAMETER_STRING_FORMAT = "yyyy/MM/dd";
        public const string WEBAPI_DATETIME_PARAMETER_STRING_FORMAT = "yyyy/MM/dd HH:mm:ss";

        public static string DateToString(DateTime dt)
        {
            return Uri.EscapeDataString(dt.ToString(WEBAPI_DATE_PARAMETER_STRING_FORMAT));
        }

        public static string DateTimeToString(DateTime dt)
        {
            return Uri.EscapeDataString(dt.ToString(WEBAPI_DATETIME_PARAMETER_STRING_FORMAT));
        }
    }


}
