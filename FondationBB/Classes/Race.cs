using System;

namespace FondationBB
{
    public class Race
    {
        public int IdRace { get; set; }
        public string LibelleRace { get; set; } = "";
        public TailleRace Taille { get; set; }
        public Espece? Espece { get; set; }

        public Race() { }

        public Race(int idRace, string libelleRace, TailleRace taille, Espece? espece)
        {
            IdRace = idRace;
            LibelleRace = libelleRace;
            Taille = taille;
            Espece = espece;
        }

        public override string ToString() => LibelleRace;
    }
}
