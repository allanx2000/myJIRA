using myJIRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myJIRA.Importers
{
    interface Importer
    {
        JIRAItem ReadJIRAs(string source);
    }
}
