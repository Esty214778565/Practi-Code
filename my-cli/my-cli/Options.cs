using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_cli
{
    internal class Options
    {
        public List<string> Languages { get; set; } = new List<string>();
        public string Output { get; set; } = string.Empty;
        public bool RemoveEmptyLines { get; set; } = false;
        public bool Note { get; set; } = false;
        public bool WithPath { get; set; } = false;
        public bool SortByABOrder { get; set; } = false;
        public bool SortByExtension { get; set; } = false;
        public string Author { get; set; } = string.Empty;
        public override string ToString()
        {
            return this.Output+" "+ Languages.ToString()+this.Note+this.Languages+this.Author+this.SortByABOrder;
        }
    }
}
