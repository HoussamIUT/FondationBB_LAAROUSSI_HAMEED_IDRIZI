using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    public class Race
    {
        private int idRace;
        private string libelleRace;
        private string tailleRace;
        private Espece especeRace;

        public Race()
        {
        }

        public Race(int idRace, string libelleRace, string tailleRace, Espece especeRace)
        {
            this.idRace = idRace;
            this.libelleRace = libelleRace;
            this.tailleRace = tailleRace;
            this.especeRace = especeRace;
        }

        public int IdRace { get => idRace; set => idRace = value; }
        public string LibelleRace { get => libelleRace; set => libelleRace = value; }
        public string TailleRace { get => tailleRace; set => tailleRace = value; }
        public Espece EspeceRace { get => especeRace; set => especeRace = value; }

        public override string ToString() => libelleRace;
    }
}
