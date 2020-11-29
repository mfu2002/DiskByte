using System;
using System.Globalization;
using System.Windows.Data;

namespace DiskByte.Helper
{
    class LongToSizeConverter : IValueConverter
    {
        /// <summary>
        /// Units compitable by the software
        /// </summary>
        readonly string[] Units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// converts bytes to readable units.
        /// </summary>
        /// <param name="value">size in bytes.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Size in a userfriendly unit (PRECISION IS LOST!)</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long size = (long)value;
            int unitIndex = 0;
            while (size >= 1024 && unitIndex < Units.Length)
            {
                size /= 1024;
                unitIndex++;
            }

            return String.Format("{0:0.#} {1}", size, Units[unitIndex]);
        }


        /// <summary>
        /// Converts to bytes. 
        /// </summary>
        /// <param name="value">size with its units.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] sizeSepereation = ((string)value).Split(' ');
            int unitIndex = 0;
            decimal size = decimal.Parse(sizeSepereation[0]);
            while (sizeSepereation[1] != Units[unitIndex])
            {
                size *= 1024;
                unitIndex++;
            }
            return (long)size;

        }
    }
}
