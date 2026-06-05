using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    // Trait de caractère / comportement (liste figée côté sujet mais stockée en BD).
    public class Comportement
    {
        public int IdComportement { get; set; }
        public string LibelleComportement { get; set; } = "";

        public Comportement() { }

        public Comportement(int idComportement, string libelleComportement)
        {
            IdComportement = idComportement;
            LibelleComportement = libelleComportement;
        }

        public override string ToString() => LibelleComportement;
    }
}

