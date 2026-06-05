using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    public class Espece
    {
        public int IdEspece { get; set; }
        public string LibelleEspece { get; set; } = "";

        public Espece() { }

        public Espece(int idEspece, string libelleEspece)
        {
            IdEspece = idEspece;
            LibelleEspece = libelleEspece;
        }

        // Raccourcis utilisés par le barème de frais (chien vs chat).
        public bool EstChat => LibelleEspece.Trim().Equals("Chat", StringComparison.OrdinalIgnoreCase);
        public bool EstChien => LibelleEspece.Trim().Equals("Chien", StringComparison.OrdinalIgnoreCase);

        public override string ToString() => LibelleEspece;
    }
}
