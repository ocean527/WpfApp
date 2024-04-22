using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Model
{
    public class HSVColorModel
    {
        public string ColorName { get; set; }
        public int H_min { get; set; }
        public int H_max { get; set; }
        public int S_min { get; set; }
        public int S_max { get; set; }
        public int V_min { get; set; }
        public int V_max { get; set; }
    }
}
