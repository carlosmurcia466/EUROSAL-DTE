using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EURO_CAPTURA
{

    public class Conexion
    {
    }

    public class Conexiones
    {
        public string servidor { get; set; }
        public string basededatos { get; set; }
        public string usuario { get; set; }
        public string pwd { get; set; }

        public string ambiente { get; set; }
    }

    public class principal
    {
        public List<Conexiones> conexiones { get; set; }
    }



}
