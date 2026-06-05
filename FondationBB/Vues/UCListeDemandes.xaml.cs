using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    /// <summary>
    /// Liste d'attente des demandes d'adoption.
    /// Auteur : Idrizi.
    /// </summary>
    public partial class UCListeDemandes : UserControl
    {
        public UCListeDemandes()
        {
            InitializeComponent();
            ChargerDemandes();
        }

        /// <summary>
        /// Petite classe d'affichage (ligne du DataGrid). On l'utilise pour
        /// présenter proprement les données jointes sans surcharger l'IHM.
        /// </summary>
        public class LigneDemande
        {
            public string NomDemandeur { get; set; }
            public string Telephone { get; set; }
            public string RaceSouhaitee { get; set; }
            public string TrancheAge { get; set; }
            public string DateDemande { get; set; }
            public int NbCorrespondances { get; set; }
        }

        private void ChargerDemandes()
        {
            try
            {
                List<Demande> demandes = DemandeDAO.GetToutesDemandes();
                List<LigneDemande> lignes = new List<LigneDemande>();

                foreach (Demande d in demandes)
                {
                    string race = d.RaceDemande == null
                        ? "Pas de préférence"
                        : $"{d.RaceDemande.EspeceRace.LibelleEspece} / {d.RaceDemande.LibelleRace}";

                    int nbMatch = MatchingDAO.GetAnimauxCorrespondants(d).Count;

                    lignes.Add(new LigneDemande
                    {
                        NomDemandeur = d.PersonneDemande.NomComplet,
                        Telephone = d.PersonneDemande.TelephonePersonne,
                        RaceSouhaitee = race,
                        TrancheAge = d.TrancheAgeDemande,
                        DateDemande = d.DateAdoptionDemande.ToString("dd/MM/yyyy"),
                        NbCorrespondances = nbMatch
                    });
                }

                dgDemandes.ItemsSource = lignes;
                txtCompteur.Text = $"{lignes.Count} demande(s) en attente";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des demandes :\n" + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRafraichir_Click(object sender, RoutedEventArgs e)
        {
            ChargerDemandes();
        }
    }
}
