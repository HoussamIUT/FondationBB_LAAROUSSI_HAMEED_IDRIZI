using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    /// <summary>
    /// Écran de correspondance automatique : à gauche les demandes,
    /// à droite les animaux disponibles qui correspondent à la demande sélectionnée.
    /// Auteur : Idrizi.
    /// </summary>
    public partial class UCMatching : UserControl
    {
        private List<Demande> demandes;

        public UCMatching()
        {
            InitializeComponent();
            ChargerDemandes();
        }

        public class LigneDemandeMatch
        {
            public Demande Demande { get; set; }
            public string NomDemandeur { get; set; }
            public string Criteres { get; set; }
        }

        public class LigneAnimalMatch
        {
            public string Nom { get; set; }
            public string Race { get; set; }
            public string Sexe { get; set; }
            public string Age { get; set; }
        }

        private void ChargerDemandes()
        {
            try
            {
                demandes = DemandeDAO.GetToutesDemandes();
                List<LigneDemandeMatch> lignes = new List<LigneDemandeMatch>();

                foreach (Demande d in demandes)
                {
                    string race = d.RaceDemande == null
                        ? "Toute race"
                        : d.RaceDemande.LibelleRace;
                    string criteres = $"{race} • {d.TrancheAgeDemande} • déposée le {d.DateAdoptionDemande:dd/MM/yyyy}";

                    lignes.Add(new LigneDemandeMatch
                    {
                        Demande = d,
                        NomDemandeur = d.PersonneDemande.NomComplet,
                        Criteres = criteres
                    });
                }

                lbDemandes.ItemsSource = lignes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des demandes :\n" + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lbDemandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbDemandes.SelectedItem is not LigneDemandeMatch ligne)
                return;

            try
            {
                List<Animal> animaux = MatchingDAO.GetAnimauxCorrespondants(ligne.Demande);
                List<LigneAnimalMatch> lignesAnimaux = new List<LigneAnimalMatch>();

                foreach (Animal a in animaux)
                {
                    lignesAnimaux.Add(new LigneAnimalMatch
                    {
                        Nom = a.NomAnimal,
                        Race = a.RaceAnimal.LibelleRace,
                        Sexe = a.SexeAnimal,
                        Age = a.CalculerAge() + " an(s)"
                    });
                }

                dgAnimaux.ItemsSource = lignesAnimaux;
                txtTitreResultats.Text = $"Animaux compatibles ({lignesAnimaux.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du calcul des correspondances :\n" + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
