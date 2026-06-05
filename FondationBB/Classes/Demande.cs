using System;

namespace FondationBB
{
    // Demande d'adoption en attente : une famille décrit l'animal souhaité.
    // La race est facultative (cardinalité 0,1 dans le MCD).
    public class Demande
    {
        public int IdDemande { get; set; }
        public DateTime DateDemande { get; set; }
        public TrancheAge TrancheAgeDemande { get; set; }
        public Personne? PersonneDemande { get; set; }
        public Race? RaceDemande { get; set; }

        public Demande() { }

        public Demande(int idDemande, DateTime dateDemande, TrancheAge trancheAge,
                       Personne? personne, Race? race)
        {
            IdDemande = idDemande;
            DateDemande = dateDemande;
            TrancheAgeDemande = trancheAge;
            PersonneDemande = personne;
            RaceDemande = race;
        }

        // Cœur de l'algorithme de matching : un animal correspond à la demande si :
        //   - il est disponible à l'adoption,
        //   - sa race correspond (ou la demande ne précise aucune race),
        //   - sa tranche d'âge correspond à celle demandée.
        public bool Correspond(Animal animal)
        {
            if (!animal.EstDisponible()) return false;

            if (RaceDemande != null &&
                (animal.Race == null || animal.Race.IdRace != RaceDemande.IdRace))
                return false;

            return animal.GetTrancheAge() == TrancheAgeDemande;
        }
    }
}
