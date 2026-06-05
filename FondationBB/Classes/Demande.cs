using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    public class Demande
    {
        private int idDemande;
        private DateTime dateAdoptionDemande;
        private string trancheAgeDemande;
        private Personne personneDemande;
        private Race raceDemande;

        public Demande()
        {
        }

        public Demande(int idDemande, DateTime dateAdoptionDemande, string trancheAgeDemande,
                       Personne personneDemande, Race raceDemande)
        {
            this.idDemande = idDemande;
            this.dateAdoptionDemande = dateAdoptionDemande;
            this.trancheAgeDemande = trancheAgeDemande;
            this.personneDemande = personneDemande;
            this.raceDemande = raceDemande;
        }

        public int IdDemande { get => idDemande; set => idDemande = value; }
        public DateTime DateAdoptionDemande { get => dateAdoptionDemande; set => dateAdoptionDemande = value; }
        public string TrancheAgeDemande { get => trancheAgeDemande; set => trancheAgeDemande = value; }
        public Personne PersonneDemande { get => personneDemande; set => personneDemande = value; }
        public Race RaceDemande { get => raceDemande; set => raceDemande = value; }
    }
}
