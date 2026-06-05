using System;

namespace FondationBB
{
    // Statut courant de l'animal : En soin, Disponible, Réservé, Adopté, Décédé.
    // Classe (et non enum) car la liste est éditable côté base de données.
    public class Statut
    {
        public int IdStatut { get; set; }
        public string LibelleStatut { get; set; } = "";

        public Statut() { }

        public Statut(int idStatut, string libelleStatut)
        {
            IdStatut = idStatut;
            LibelleStatut = libelleStatut;
        }

        public override string ToString() => LibelleStatut;
    }
}
