using System;

namespace FondationBB
{
    // Adoptant / demandeur (personne extérieure au refuge).
    public class Personne
    {
        public int IdPersonne { get; set; }
        public string NomPersonne { get; set; } = "";
        public string PrenomPersonne { get; set; } = "";
        public DateTime DateNaissancePersonne { get; set; }
        public string NumeroPersonne { get; set; } = "";
        public string RuePersonne { get; set; } = "";
        public string CpPersonne { get; set; } = "";
        public string VillePersonne { get; set; } = "";
        public string TelephonePersonne { get; set; } = "";
        public string MailPersonne { get; set; } = "";
        public DateTime DateCreationPersonne { get; set; }

        public Personne() { }

        public Personne(int idPersonne, string nomPersonne, string prenomPersonne,
                        string telephonePersonne, string mailPersonne)
        {
            IdPersonne = idPersonne;
            NomPersonne = nomPersonne;
            PrenomPersonne = prenomPersonne;
            TelephonePersonne = telephonePersonne;
            MailPersonne = mailPersonne;
        }

        public string NomComplet => $"{PrenomPersonne} {NomPersonne}".Trim();
    }
}
