using System;
using DFe.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DFe.Testes.Gerais
{
    /// <summary>
    /// Testes da formatação de data/hora usada nos leiautes sem fuso horário (CT-e 2.00 e MDF-e 1.00)
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
    }
}
