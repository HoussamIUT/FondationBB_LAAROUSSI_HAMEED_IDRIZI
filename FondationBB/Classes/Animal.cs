using System;
using System.Collections.Generic;

namespace FondationBB
{
    // Entité centrale du modèle : un animal du refuge.
    public class Animal
    {
        public int IdAnimal { get; set; }
        public string NomAnimal { get; set; } = "";
        public DateTime DateNaissanceAnimal { get; set; }
        public SexeAnimal Sexe { get; set; }
        public double PoidsAnimal { get; set; }
        public string IcadAnimal { get; set; } = "";
        public DateTime DateArriveeAnimal { get; set; }
        public string AnnotationAnimal { get; set; } = "";
        public bool SteriliseAnimal { get; set; }

        public Race? Race { get; set; }
        public Statut? Statut { get; set; }
        public Etat? Etat { get; set; }
        public Employe? Employe { get; set; }   // employé qui a enregistré l'animal

        public List<Comportement> Comportements { get; set; } = new List<Comportement>();
        public List<SoinRecu> SoinsRecus { get; set; } = new List<SoinRecu>();

        public Animal() { }

        public Animal(int idAnimal, string nomAnimal, DateTime dateNaissance,
                      SexeAnimal sexe, Race? race, Statut? statut)
        {
            IdAnimal = idAnimal;
            NomAnimal = nomAnimal;
            DateNaissanceAnimal = dateNaissance;
            Sexe = sexe;
            Race = race;
            Statut = statut;
        }

        // Âge en années révolues.
        public int CalculerAge()
        {
            DateTime today = DateTime.Today;
            int age = today.Year - DateNaissanceAnimal.Year;
            if (DateNaissanceAnimal.Date > today.AddYears(-age)) age--;
            return age < 0 ? 0 : age;
        }

        // Âge en mois (utile pour le barème chat < 6 mois).
        public int CalculerAgeEnMois()
        {
            DateTime today = DateTime.Today;
            int mois = (today.Year - DateNaissanceAnimal.Year) * 12 + today.Month - DateNaissanceAnimal.Month;
            if (today.Day < DateNaissanceAnimal.Day) mois--;
            return mois < 0 ? 0 : mois;
        }

        // Âge lisible : en mois tant que l'animal a moins d'un an (évite "0 an(s)"),
        // puis en années. Ex. : "5 mois", "1 an", "3 ans".
        public string AgeLisible()
        {
            int mois = CalculerAgeEnMois();
            if (mois < 1) return "moins d'un mois";
            if (mois < 12) return mois + " mois";
            int ans = CalculerAge();
            return ans + (ans > 1 ? " ans" : " an");
        }

        // Un animal est proposable à l'adoption uniquement s'il est "Disponible".
        public bool EstDisponible()
        {
            return Statut != null &&
                   Statut.LibelleStatut.Trim().Equals("Disponible", StringComparison.OrdinalIgnoreCase);
        }

        public bool EstSterilise() => SteriliseAnimal;

        // Tranche d'âge dérivée pour le matching avec les demandes.
        public TrancheAge GetTrancheAge()
        {
            int ans = CalculerAge();
            if (ans < 2) return TrancheAge.Junior;
            if (ans <= 7) return TrancheAge.Adulte;
            return TrancheAge.Senior;
        }
    }
}
