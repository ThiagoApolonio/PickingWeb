using Picking_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Picking_Web.Contexto
{
    public class CodigoDeBarras
    {
        public int Width { get; set; } = 80;
        public int Height { get; set; } = 816;
        public string Directory { get; set; } = GlobalHelper.RelatoriosPath + "/CodigoDeBarras";


        public Bitmap GenerateCodigoDeBarras(int width, int height, string text, ZXing.BarcodeFormat format)
        {
            try
            {
                var bw = new ZXing.BarcodeWriter();
                var encOptions = new ZXing.Common.EncodingOptions() { Width = width, Height = height, Margin = 0 };
                bw.Options = encOptions;
                bw.Format = format;
                var resultado = new Bitmap(bw.Write(text));
                return resultado;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}