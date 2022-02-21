using Picking_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Picking_Web.Contexto {
    public class QrCode {
        public int Width { get; set; } = 272;
        public int Height { get; set; } = 252;
        public string Directory { get; set; } = GlobalHelper.RelatoriosPath + "/QrCode";


        public Bitmap GenerateQrCode(int width, int height, string text, ZXing.BarcodeFormat format) {
            try {
                var bw = new ZXing.BarcodeWriter();
                var encOptions = new ZXing.Common.EncodingOptions() { Width = width, Height = height, Margin = 0 };
                bw.Options = encOptions;
                bw.Format = format;
                var resultado = new Bitmap(bw.Write(text));
                return resultado;
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}