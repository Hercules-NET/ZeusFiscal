using Shared.NFe.Utils.InfRespTec;
using Xunit;

namespace NFe.Utils.Testes.InfRespTec
{
    public class GerarHashCSRTTestes
    {
        [Fact]
        public void HashCSRT_confere_com_exemplo_publicado_para_o_responsavel_tecnico()
        {
            var hash = GerarHashCSRT.HashCSRT("G8063VRTNDMO886SFNK5LDUDEI24XJ22YIPO", "41180678393592000146558900000006041028190697");

            Assert.Equal("aWv6LeEM4X6u4+qBI2OYZ8grigw=", hash);
        }
    }
}
