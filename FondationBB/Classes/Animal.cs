using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    public class Animal
    {
        private int idAnimal;
        private string nomAnimal;
        private DateTime dateNaissanceAnimal;
        private string icadAnimal;
        private string sexeAnimal;
        private string annotationAnimal;
        private DateTime dateArriveAnimal;
        private double poidsAnimal;
        private Employe employeAnimal;
        private Etat etatAnimal;
        private Statut statutAnimal;
        private Race raceAnimal;
        private List<Comportement> comportements;

        public Animal()
        {
            comportements = new List<Comportement>();
        }

        public Animal(int idAnimal, string nomAnimal, DateTime dateNaissanceAnimal,
                      string sexeAnimal, Race raceAnimal, Statut statutAnimal)
        {
            this.idAnimal = idAnimal;
            this.nomAnimal = nomAnimal;
            this.dateNaissanceAnimal = dateNaissanceAnimal;
            this.sexeAnimal = sexeAnimal;
            this.raceAnimal = raceAnimal;
            this.statutAnimal = statutAnimal;
            this.comportements = new List<Comportement>();
        }

        public int IdAnimal { get => idAnimal; set => idAnimal = value; }
        public string NomAnimal { get => nomAnimal; set => nomAnimal = value; }
        public DateTime DateNaissanceAnimal { get => dateNaissanceAnimal; set => dateNaissanceAnimal = value; }
        public string IcadAnimal { get => icadAnimal; set => icadAnimal = value; }
        public string SexeAnimal { get => sexeAnimal; set => sexeAnimal = value; }
        public string AnnotationAnimal { get => annotationAnimal; set => annotationAnimal = value; }
        public DateTime DateArriveAnimal { get => dateArriveAnimal; set => dateArriveAnimal = value; }
        public double PoidsAnimal { get => poidsAnimal; set => poidsAnimal = value; }
        public Employe EmployeAnimal { get => employeAnimal; set => employeAnimal = value; }
        public Etat EtatAnimal { get => etatAnimal; set => etatAnimal = value; }
        public Statut StatutAnimal { get => statutAnimal; set => statutAnimal = value; }
        public Race RaceAnimal { get => raceAnimal; set => raceAnimal = value; }
        public List<Comportement> Comportements { get => comportements; set => comportements = value; }

        /// <summary>Âge en années complètes calculé à partir de la date de naissance.</summary>
        public int CalculerAge()
        {
            int age = DateTime.Now.Year - dateNaissanceAnimal.Year;
            if (DateTime.Now.DayOfYear < dateNaissanceAnimal.DayOfYear) age--;
            return age < 0 ? 0 : age;
        }

        public override string ToString() => nomAnimal;
    }
}
