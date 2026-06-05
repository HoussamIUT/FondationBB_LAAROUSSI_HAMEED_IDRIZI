using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    // Mise en relation des demandes en attente avec les animaux disponibles (responsable).
    public partial class UCCorrespondances : UserControl
    {
        private readonly DemandeDAO demandeDao = new DemandeDAO();
        private readonly MatchingDAO matchingDao = new MatchingDAO();
        private List<Demande> demandes = new List<Demande>();

        public UCCorrespondances()
        {
            InitializeComponent();
            ChargerDemandes();
        }

        private void ChargerDemandes()
        {
            try
            {
                demandes = demandeDao.GetToutesDemandes();
                List<LigneDemande> lignes = new List<LigneDemande>();
                foreach (Demande d in demandes)
                {
                    lignes.Add(new LigneDemande
                    {
                        Titre = d.PersonneDemande?.NomComplet ?? "—",
                        SousTitre = (d.RaceDemande != null ? d.RaceDemande.LibelleRace : "Race indifférente")
                                    + " · " + d.TrancheAgeDemande
                    });
                }
                lbDemandes.ItemsSource = lignes;

                if (lignes.Count == 0)
                    MessageBox.Show("Aucune demande d'adoption en attente pour le moment.",
                        "Correspondances", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCCorrespondances.ChargerDemandes");
                MessageBox.Show("Impossible de charger les demandes : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lbDemandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lbDemandes.SelectedIndex;
            if (index < 0 || index >= demandes.Count)
            {
                grilleAnimaux.ItemsSource = null;
                return;
            }

            try
            {
                List<Animal> animaux = matchingDao.GetAnimauxCorrespondants(demandes[index]);
                List<LigneAnimal> lignes = new List<LigneAnimal>();
                foreach (Animal a in animaux)
                {
                    lignes.Add(new LigneAnimal
                    {
                        Nom = a.NomAnimal,
                        Espece = a.Race?.Espece?.LibelleEspece ?? "",
                        Race = a.Race?.LibelleRace ?? "",
                        Sexe = a.Sexe == SexeAnimal.Femelle ? "Femelle" : "Mâle",
                        Age = a.AgeLisible()
                    });
                }
                grilleAnimaux.ItemsSource = lignes;
                txtTitreAnimaux.Text = $"Animaux compatibles ({lignes.Count})";
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCCorrespondances.Selection");
                MessageBox.Show("Impossible de calculer les correspondances : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class LigneDemande
        {
            public string Titre { get; set; } = "";
            public string SousTitre { get; set; } = "";
        }

        public class LigneAnimal
        {
            public string Nom { get; set; } = "";
            public string Espece { get; set; } = "";
            public string Race { get; set; } = "";
            public string Sexe { get; set; } = "";
            public string Age { get; set; } = "";
        }
    }
}
