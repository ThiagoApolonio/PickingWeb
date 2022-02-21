using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace Picking_Web
{
    public class GravarLog
    {
        private string File { get; set; }
        private string Path { get; set; }
        private string[] Lines { get; set; }

        public GravarLog()
        {
            try
            {
                this.Path = $@"{AppDomain.CurrentDomain.BaseDirectory}Log\";
                this.File = this.Path + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                    //Concedendo permissões
                    concederPermissao(Path);

                }
            }
            catch (Exception e) { }
        }

        private void concederPermissao(string file)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = @"/C Cacls " + file + " /E /P Todos:F";
            process.StartInfo = startInfo;
            process.Start();
        }

        public GravarLog(bool log)
        {
            this.Path = $@"{AppDomain.CurrentDomain.BaseDirectory}Log\";
            this.File = Startup._logFile;

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }

        public string Escrever(string registro)
        {
            try
            {
                StreamWriter writer = new StreamWriter(this.File, true);
                using (writer)
                {
                    // Escreve uma nova linha no final do arquivo
                    writer.WriteLine(" ## " + DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss") + " ### " + registro);
                }
                return File;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}