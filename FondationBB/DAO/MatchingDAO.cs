using System;
using System.Collections.Generic;

namespace FondationBB
{
    // Algorithme de correspondance entre demandes en attente et animaux disponibles.
    // La règle métier elle-même vit dans Demande.Correspond(Animal) ; ce DAO orchestre
    // la récupération des animaux disponibles et applique le filtre.
    public class MatchingDAO
    {
        private readonly AnimalDAO animalDao = new AnimalDAO();

        // Animaux disponibles qui correspondent à la demande (race + tranche d'âge).
        public List<Animal> GetAnimauxCorrespondants(Demande demande)
        {
            List<Animal> correspondants = new List<Animal>();

            // On ne charge que les animaux "Disponible" (filtré en SQL), puis on applique
            // la règle de correspondance (race et tranche d'âge) côté métier.
            foreach (Animal animal in animalDao.GetAnimaux(libelleStatut: "Disponible"))
            {
                if (demande.Correspond(animal))
                    correspondants.Add(animal);
            }
            return correspondants;
        }
    }
}
