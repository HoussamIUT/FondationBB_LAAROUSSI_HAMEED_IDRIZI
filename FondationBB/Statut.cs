using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    public class Statut
    {
        private int idStatut;
        private string libelleStatut;

        public Statut()
        {
        }

        public Statut(int idStatut, string libelleStatut)
        {
            this.idStatut = idStatut;
            this.libelleStatut = libelleStatut;
        }

        public int IdStatut { get => idStatut; set => idStatut = value; }
        public string LibelleStatut { get => libelleStatut; set => libelleStatut = value; }

        public override string ToString() => libelleStatut;
    }
}
