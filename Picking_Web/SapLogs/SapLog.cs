using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CenterlabWEB.Log
{
    public class SapLog
    {
        public SapLog()
        {
            this.Path = $@"{AppDomain.CurrentDomain.BaseDirectory}LogPicking\";
            this.File = this.Path + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }
        public SapLog(string file)
        {
            this.Path = $@"{AppDomain.CurrentDomain.BaseDirectory}LogPicking\";
            this.File = file;

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }
        private string File { get; set; }
        private string Path { get; set; }
        private string[] Lines { get; set; }

        public string Escrever(string registro)
        {
            try
            {
                StreamWriter writer = new StreamWriter(File, true);
                using (writer)
                {
                    // Escreve uma nova linha no final do arquivo
                    writer.WriteLine(registro);
                }
            }
            catch (Exception ex)
            {
            }
            return File;
        }

        public void AbrirLog()
        {
            using (var fileStream = new FileStream(File, FileMode.Open))
            {

            }
        }
        public void GerarBat(string registro)
        {
            try
            {
                StreamWriter writer = new StreamWriter(File, false);
                using (writer)
                {
                    // Escreve uma nova linha no final do arquivo
                    writer.WriteLine(registro);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}