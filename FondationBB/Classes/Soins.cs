using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    // Soin vétérinaire pratiqué au refuge (catalogue avec prix moyen).
    public class Soin
    {
        public int IdSoin { get; set; }
        public string LibelleSoin { get; set; } = "";
        public decimal PrixSoin { get; set; }

        public Soin() { }

        public Soin(int idSoin, string libelleSoin, decimal prixSoin)
        {
            IdSoin = idSoin;
            LibelleSoin = libelleSoin;
            PrixSoin = prixSoin;
        }

        public override string ToString() => LibelleSoin;
    }
}
