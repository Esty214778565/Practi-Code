using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_cli
{
    internal class ResponseFile
    {
        public string Command { get; set; }
        public string FileName { get; set; }
        public ResponseFile(string path)
        {
            CreateResponse(path);
        }
        static void CreateResponse(string path)
        {
            File.Create(path).Close();
        }

    }
}
