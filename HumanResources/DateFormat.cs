using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HumanResources

{
	static class  DateFormat
	{
		/// <summary>
		/// Ustala format daty na 0001-01-01
		/// </summary>
		/// <returns></returns>
		public static DateTimeFormatInfo TakeDateFormat()
		{
			DateTimeFormatInfo dTFormatInfo = new DateTimeFormatInfo();

			dTFormatInfo.ShortDatePattern = "yyyy/MM/dd";
			dTFormatInfo.DateSeparator = "-";
			return dTFormatInfo;
		}
        public static DateTimeFormatInfo TakeDateFormatDayFirst()
        {
            DateTimeFormatInfo dTFormatInfo = new DateTimeFormatInfo();

            dTFormatInfo.ShortDatePattern = "dd/MM/yyyy";
            dTFormatInfo.DateSeparator = ".";
            return dTFormatInfo;
        }
        public static DateTimeFormatInfo TakeTimeFormat()
        {
            DateTimeFormatInfo dTFormatInfo = new DateTimeFormatInfo();//CultureInfo.GetCultureInfo("pl-PL").DateTimeFormat;

           // CultureInfo cultureInfo = new CultureInfo("pl-PL");
            dTFormatInfo.ShortTimePattern = "H:mm";
            dTFormatInfo.TimeSeparator = ":";
           // cultureInfo.DateTimeFormat = dTFormatInfo;
            return dTFormatInfo;
        }

        public static string TakeTimeSpanFormat()
        {
            return @"h\:mm";
        }
    }
}
