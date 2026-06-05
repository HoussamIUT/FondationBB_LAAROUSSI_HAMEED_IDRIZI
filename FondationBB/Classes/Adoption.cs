using System;

namespace FondationBB
{
    // Contrat d'adoption : lie un animal, un adoptant et l'employé qui réalise l'adoption.
    public class Adoption
    {
        public int IdAdoption { get; set; }
        public DateTime DateAdoption { get; set; }
        public decimal FraisAdoption { get; set; }

        public Animal? Animal { get; set; }
        public Personne? Personne { get; set; }   // l'adoptant
        public Employe? Employe { get; set; }      // qui réalise l'adoption

        public Adoption() { }

        public Adoption(Animal animal, Personne personne, Employe employe, decimal frais)
        {
            Animal = animal;
            Personne = personne;
            Employe = employe;
            FraisAdoption = frais;
            DateAdoption = DateTime.Today;
        }

        // Calcule automatiquement les frais d'adoption selon le barème du sujet.
        // Barème :
        //   Chat  < 6 mois (non stérilisé) ............ 90 €
        //   Chat  > 6 mois (stérilisé) ................ 120 €
        //   Chien < 1 an .............................. 150 €
        //   Chien 1 à 7 ans ........................... 180 €
        //   Chien > 7 ans ............................. 100 €
        // Méthode statique = barème centralisé et testable indépendamment.
        public static decimal CalculerFraisAuto(Animal animal)
        {
            bool estChat = animal.Race?.Espece?.EstChat ?? false;

            if (estChat)
            {
                int mois = animal.CalculerAgeEnMois();
                if (mois < 6 && !animal.EstSterilise()) return 90m;
                return 120m;
            }
            else // chien
            {
                int ans = animal.CalculerAge();
                if (ans < 1) return 150m;
                if (ans <= 7) return 180m;
                return 100m;
            }
        }
    }
}
