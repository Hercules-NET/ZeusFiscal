using System;
using System.Drawing;

namespace NFe.Danfe.Nativo.GraphicsPrinter
{
    internal static class MedidasLinha
    {
        public static Medida GetMedidas(AdicionarTexto adicionarTexto)
        {
            Medida medida = GetMedidas(adicionarTexto.Texto, adicionarTexto.Fonte);

            return medida;
        }

        public static Medida GetMedidas(string texto, Font fonte)
        {
            /*
             * macOS:
             *  Graphics g = Graphics.FromHwnd(IntPtr.Zero);
             *  System.NotSupportedException: Specified method is not supported.
             */
            Bitmap bmp = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bmp);
            SizeF tamanhoDaString = g.MeasureString(texto, fonte);
            int alturaLinha = Convert.ToInt32(tamanhoDaString.Height);
            int larguraLinha = Convert.ToInt32(tamanhoDaString.Width);

            return new Medida(alturaLinha, larguraLinha);
        }

    }

    public class Medida
    {
        public Medida(int altura, int largura)
        {
            Altura = altura;
            Largura = largura;
        }

        public int Altura { get; private set; }
        public int Largura { get; private set; }
    }
}