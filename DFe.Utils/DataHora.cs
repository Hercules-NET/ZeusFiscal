using System;
using System.Globalization;

namespace DFe.Utils
{
    public static class DataHora
    {

        /// <summary>
        /// Retorna uma string no formato AAAA-MM-DD
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataString(this DateTime data)
        {
            return data == DateTime.MinValue ? null : data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retorna uma string no formato AAAA-MM-DD
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataString(this DateTime? data)
        {
            if (data == null) return null;

            return data == DateTime.MinValue ? null : data.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retorna uma string no formato AAAA-MM-DDThh:mm:ssTZD (UTC - Universal Coordinated Time)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringUtc(this DateTime data)
        {
            return data == DateTime.MinValue ? null : data.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retorna uma string no formato AAAA-MM-DDThh:mm:ss
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringSemUtc(this DateTime data)
        {
            return data == DateTime.MinValue ? null : data.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retorna uma string no formato AAAA-MM-DDThh:mm:ss
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringSemUtc(this DateTime? data)
        {
            return ParaDataHoraStringSemUtc(data.GetValueOrDefault());
        }

        /// <summary>
        /// Retorna uma string no formato AAAA-MM-DDThh:mm:ssTZD (UTC - Universal Coordinated Time)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringUtc(this DateTime? data)
        {
            return ParaDataHoraStringUtc(data.GetValueOrDefault());
        }

        /// <summary>
        /// Retorna uma string no formato AAAA-MM-DD HH:mm:ss
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraString(this DateTime data)
        {
            return data.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        public static string ParaHoraString(this TimeSpan hora)
        {
            return hora.ToString(@"hh\:mm\:ss");
        }
    }
}