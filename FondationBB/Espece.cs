using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    public class Espece
    {
        private int idEspece;
        private string libelleEspece;

        public Espece()
        {
        }

        public Espece(int idEspece, string libelleEspece)
        {
            this.idEspece = idEspece;
            this.libelleEspece = libelleEspece;
        }

        public int IdEspece { get => idEspece; set => idEspece = value; }
        public string LibelleEspece { get => libelleEspece; set => libelleEspece = value; }

        public override string ToString() => libelleEspece;
    }
}
