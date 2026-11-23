using System;
using System.Globalization;
using DFe.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DFe.Testes.Gerais
{
    /// <summary>
    /// Testes da formatação de data/hora usada nos XMLs (dhEmi, dhEvento etc.)
    /// </summary>
    [TestClass]
    public class DataHoraTestes
    {
        [TestMethod]
        public void ParaDataHoraStringSemUtc_DateTime_FormataComSegundos()
        {
            var data = new DateTime(2026, 11, 5, 14, 30, 45);

            Assert.AreEqual("2026-11-05T14:30:45", data.ParaDataHoraStringSemUtc());
        }

        [TestMethod]
        public void ParaDataHoraStringSemUtc_DateTimeNullable_FormataComSegundos()
        {
            DateTime? data = new DateTime(2026, 11, 5, 14, 30, 45);

            Assert.AreEqual("2026-11-05T14:30:45", data.ParaDataHoraStringSemUtc());
        }

        [TestMethod]
        public void ParaDataHoraStringSemUtc_DateTimeOffset_FormataComSegundos()
        {
            var data = new DateTimeOffset(2026, 11, 5, 14, 30, 45, TimeSpan.FromHours(-3));

            Assert.AreEqual("2026-11-05T14:30:45", data.ParaDataHoraStringSemUtc());
        }

        [TestMethod]
        public void ParaDataHoraStringSemUtc_DateTimeOffsetNullable_FormataComSegundos()
        {
            DateTimeOffset? data = new DateTimeOffset(2026, 11, 5, 14, 30, 45, TimeSpan.FromHours(-3));

            Assert.AreEqual("2026-11-05T14:30:45", data.ParaDataHoraStringSemUtc());
        }

        [TestMethod]
        [DataRow("th-TH")]
        [DataRow("ar-SA")]
        public void DataHora_DateTime_ComCulturaDeOutroCalendario_FormataNoCalendarioGregoriano(string cultura)
        {
            var data = new DateTime(2026, 11, 5, 14, 30, 45);
            DateTime? dataNullable = data;

            ExecutarNaCultura(cultura, () =>
            {
                Assert.AreEqual("2026-11-05", data.ParaDataString());
                Assert.AreEqual("2026-11-05", dataNullable.ParaDataString());
                StringAssert.StartsWith(data.ParaDataHoraStringUtc(), "2026-11-05T14:30:45");
                StringAssert.StartsWith(dataNullable.ParaDataHoraStringUtc(), "2026-11-05T14:30:45");
                Assert.AreEqual("2026-11-05T14:30:45", data.ParaDataHoraStringSemUtc());
                Assert.AreEqual("2026-11-05T14:30:45", dataNullable.ParaDataHoraStringSemUtc());
                Assert.AreEqual("20261105143045", data.ParaDataHoraString());
            });
        }

        [TestMethod]
        [DataRow("th-TH")]
        [DataRow("ar-SA")]
        public void DataHoraOffSet_DateTimeOffset_ComCulturaDeOutroCalendario_FormataNoCalendarioGregoriano(string cultura)
        {
            var data = new DateTimeOffset(2026, 11, 5, 14, 30, 45, TimeSpan.FromHours(-3));
            DateTimeOffset? dataNullable = data;

            ExecutarNaCultura(cultura, () =>
            {
                Assert.AreEqual("2026-11-05", data.ParaDataString());
                Assert.AreEqual("2026-11-05", dataNullable.ParaDataString());
                Assert.AreEqual("2026-11-05T14:30:45-03:00", data.ParaDataHoraStringUtc());
                Assert.AreEqual("2026-11-05T14:30:45-03:00", dataNullable.ParaDataHoraStringUtc());
                Assert.AreEqual("2026-11-05T14:30:45", data.ParaDataHoraStringSemUtc());
                Assert.AreEqual("2026-11-05T14:30:45", dataNullable.ParaDataHoraStringSemUtc());
                Assert.AreEqual("20261105143045", data.ParaDataHoraString());
            });
        }

        private static void ExecutarNaCultura(string cultura, Action acao)
        {
            var culturaOriginal = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(cultura);
                acao();
            }
            finally
            {
                CultureInfo.CurrentCulture = culturaOriginal;
            }
        }
    }
}
