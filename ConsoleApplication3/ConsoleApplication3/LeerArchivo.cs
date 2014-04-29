using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatoConsola
{
    class LeerArchivo
    {
        public List<String> leerFile(String ruta) {

            System.IO.StreamReader sr = new System.IO.StreamReader(ruta, System.Text.Encoding.Default);
            string texto;
            List<String> listConocimiento = new List<String>();
            texto = sr.ReadLine();
            while(texto != null ){
                listConocimiento.Add(texto);
                texto = sr.ReadLine();
            }
            sr.Close();
            return listConocimiento;
        }

    }
}
