using myJIRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myJIRA.Exporters
{
    public interface IExporter
    {
        void Export(List<JIRAItem> jiras, string outputFile);
    }
}
