using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.PrinterLayout
{
    public class TagParams
    {
        public TagParams() { }

        public enum tipo
        {
            Qr,
            Bar13,
            Bar128,
            DataMtx,
            PrintDirectory
        }

        public int IdTipo { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Directory { get; set; }

        public List<TagParams> GetAll()
        {
            var lst = new List<TagParams>();
            lst.Add(new TagParams
            {
                IdTipo = (int)TagParams.tipo.Qr,
                Directory = $@"{AppDomain.CurrentDomain.BaseDirectory}\QrCode",
                Width = 200,
                Height = 200
            });
            lst.Add(new TagParams
            {
                IdTipo = (int)TagParams.tipo.Bar13,
                Directory = $@"{AppDomain.CurrentDomain.BaseDirectory}\Bar13",
                Width = 50,
                Height = 100
            });
            lst.Add(new TagParams
            {
                IdTipo = (int)TagParams.tipo.Bar128,
                Directory = $@"{AppDomain.CurrentDomain.BaseDirectory}\Bar128.rpt",
                Width = 550,
                Height = 220
            });
            lst.Add(new TagParams
            {
                IdTipo = (int)TagParams.tipo.DataMtx,
                Directory = $@"{AppDomain.CurrentDomain.BaseDirectory}\DataMatrix",
                Width = 500,
                Height = 500
            });
            lst.Add(new TagParams
            {
                IdTipo = (int)TagParams.tipo.PrintDirectory,
                Directory = $@"{AppDomain.CurrentDomain.BaseDirectory}\PrintTag",
                Width = 500,
                Height = 500
            });

            return lst;
        }
    }
}