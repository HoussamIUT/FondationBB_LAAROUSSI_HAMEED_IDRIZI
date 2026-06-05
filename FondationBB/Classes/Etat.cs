using System;

namespace FondationBB
{
    // État de santé : Sain, En soin, Convalescent, Maladie chronique, Handicapé, Contagieux.
    public class Etat
    {
        public int IdEtat { get; set; }
        public string LibelleEtat { get; set; } = "";

        public Etat() { }

        public Etat(int idEtat, string libelleEtat)
        {
            IdEtat = idEtat;
            LibelleEtat = libelleEtat;
        }

        public override string ToString() => LibelleEtat;
    }
}
