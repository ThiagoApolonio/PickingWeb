using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Picking_Web.PrinterLayout
{
    public class PrintPickingTag
    {
        public PrintPickingTag() { }

        public string  Print(string lote)
        {
            string rpt = ($@"{AppDomain.CurrentDomain.BaseDirectory}\Relatorio\Etiqueta.rpt");
            if (File.Exists(rpt))
            {
                var tag = new TagParams().GetAll();
                var CodigoBArras128 = tag.Where(x => x.IdTipo == (int)TagParams.tipo.Bar128).FirstOrDefault();
                if (!Directory.Exists(CodigoBArras128.Directory))
                    Directory.CreateDirectory(CodigoBArras128.Directory);

                var barcode = CodigoBArras128.Directory + "\\code_" + lote + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                
                var img = GenerateCode(CodigoBArras128.Width, CodigoBArras128.Height, lote, ZXing.BarcodeFormat.CODE_128);
                img.Save(barcode);
                return barcode;
            }
            else
            {
                return "";
            }

        }

        public string PrintQrCode(string lote)
        {
            String leitura =  ConfigurationManager.AppSettings["LEITURA"].ToString();

            //Leitura QRCode
            if (leitura == "QRCODE")
            {
                string rpt = ($@"{AppDomain.CurrentDomain.BaseDirectory}\Relatorio\EtiquetaQrCode.rpt");
                if (File.Exists(rpt))
                {
                    var tag = new TagParams().GetAll();
                    var QrCode = tag.Where(x => x.IdTipo == (int)TagParams.tipo.Qr).FirstOrDefault();
                    if (!Directory.Exists(QrCode.Directory))
                        Directory.CreateDirectory(QrCode.Directory);

                    var qrCode = QrCode.Directory + "\\code_" + lote + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";

                    var img = GenerateCode(QrCode.Width, QrCode.Height, lote, ZXing.BarcodeFormat.QR_CODE);

                    img.Save(qrCode);
                    return qrCode;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                //Leitura cógido de barras
                string rpt = ($@"{AppDomain.CurrentDomain.BaseDirectory}\Relatorio\Etiqueta.rpt");
                if (File.Exists(rpt))
                {
                    var tag = new TagParams().GetAll();
                    var CodigoBArras128 = tag.Where(x => x.IdTipo == (int)TagParams.tipo.Bar128).FirstOrDefault();
                    if (!Directory.Exists(CodigoBArras128.Directory))
                        Directory.CreateDirectory(CodigoBArras128.Directory);

                    var barcode = CodigoBArras128.Directory + "\\code_" + lote + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";

                    var img = GenerateCode(CodigoBArras128.Width, CodigoBArras128.Height, lote, ZXing.BarcodeFormat.CODE_128);
                    img.Save(barcode);
                    return barcode;
                }
                else
                {
                    return "";
                }
            }

            

        }

        public Bitmap GenerateCode(int width, int height, string text, ZXing.BarcodeFormat format)
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