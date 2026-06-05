using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    // Écran de recherche/consultation des animaux (liste de cartes + filtres).
    public partial class UCCatalogueAnimaux : UserControl
    {
        private readonly AnimalDAO animalDao = new AnimalDAO();
        private readonly ReferenceDAO refDao = new ReferenceDAO();
        private bool initialise = false;

        public UCCatalogueAnimaux()
        {
            InitializeComponent();
            // L'enregistrement d'un animal est réservé au responsable.
            butNouvel.Visibility = Session.EstResponsable ? Visibility.Visible : Visibility.Collapsed;

            ChargerFiltres();
            initialise = true;
            Rechercher();
        }

        private void ChargerFiltres()
        {
            try
            {
                // Espèces (avec une entrée "toutes" d'id 0).
                List<Espece> especes = refDao.GetEspeces();
                especes.Insert(0, new Espece(0, "Toutes les espèces"));
                cbEspece.ItemsSource = especes;
                cbEspece.SelectedIndex = 0;

                ChargerRaces(null);

                List<Statut> statuts = refDao.GetStatuts();
                statuts.Insert(0, new Statut(0, "Tous les statuts"));
                cbStatut.ItemsSource = statuts;
                cbStatut.SelectedIndex = 0;

                // Tranches d'âge (Junior < 2 ans, Adulte 2-7 ans, Senior > 7 ans).
                // Filtre appliqué côté application car la tranche est calculée depuis la date de naissance.
                cbTranche.ItemsSource = new List<string> { "Toutes les tranches", "Junior", "Adulte", "Senior" };
                cbTranche.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCCatalogueAnimaux.ChargerFiltres");
                txtResultats.Text = "Impossible de charger les filtres.";
                MessageBox.Show("Impossible de charger les filtres : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Recharge la liste des races selon l'espèce sélectionnée (ou toutes).
        private void ChargerRaces(int? idEspece)
        {
            List<Race> races = refDao.GetRaces(idEspece);
            races.Insert(0, new Race(0, "Toutes les races", TailleRace.Petit, null));
            cbRace.ItemsSource = races;
            cbRace.SelectedIndex = 0;
        }

        private void cbEspece_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initialise) return; // évite de déclencher pendant l'initialisation
            int idEspece = ToInt(cbEspece.SelectedValue);
            ChargerRaces(idEspece == 0 ? (int?)null : idEspece);
        }

        // Recherche live : déclenchée à la frappe dans le champ nom.
        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (initialise) Rechercher();
        }

        // Recherche live : déclenchée au changement d'un filtre (race, statut, tranche d'âge).
        private void cbFiltre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialise) Rechercher();
        }

        private void Rechercher()
        {
            try
            {
                string? nom = string.IsNullOrWhiteSpace(txtRecherche.Text) ? null : txtRecherche.Text.Trim();
                int idEspece = ToInt(cbEspece.SelectedValue);
                int idRace = ToInt(cbRace.SelectedValue);
                string? statut = (cbStatut.SelectedItem as Statut)?.IdStatut > 0
                    ? ((Statut)cbStatut.SelectedItem).LibelleStatut : null;

                // Tranche d'âge : null = toutes (l'entrée 0 est "Toutes les tranches").
                string? tranche = cbTranche.SelectedIndex > 0 ? cbTranche.SelectedItem as string : null;

                List<Animal> animaux = animalDao.GetAnimaux(
                    nom: nom,
                    idEspece: idEspece == 0 ? (int?)null : idEspece,
                    idRace: idRace == 0 ? (int?)null : idRace,
                    libelleStatut: statut);

                List<LigneAnimal> lignes = new List<LigneAnimal>();
                foreach (Animal a in animaux)
                {
                    // Filtre tranche d'âge appliqué ici (calcul depuis la date de naissance).
                    if (tranche != null && a.GetTrancheAge().ToString() != tranche) continue;
                    lignes.Add(EnLigne(a));
                }

                grilleAnimaux.ItemsSource = lignes;
                txtResultats.Text = $"{lignes.Count} animal(aux) trouvé(s)";
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCCatalogueAnimaux.Rechercher");
                grilleAnimaux.ItemsSource = null;
                txtResultats.Text = "Erreur lors du chargement des animaux.";
                MessageBox.Show("Erreur lors du chargement des animaux : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Voir_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is LigneAnimal ligne)
                Navigateur.Afficher(new UCFicheAnimal(ligne.IdAnimal));
        }

        private void butNouvel_Click(object sender, RoutedEventArgs e)
        {
            Navigateur.Afficher(new UCAnimalFormulaire());
        }

        // Transforme un Animal du modèle en ligne du DataGrid.
        private LigneAnimal EnLigne(Animal a)
        {
            return new LigneAnimal
            {
                IdAnimal = a.IdAnimal,
                Nom = a.NomAnimal,
                Espece = a.Race?.Espece?.LibelleEspece ?? "",
                Race = a.Race?.LibelleRace ?? "",
                Sexe = a.Sexe == SexeAnimal.Femelle ? "Femelle" : "Mâle",
                Age = a.AgeLisible(),
                Statut = a.Statut?.LibelleStatut ?? "—"
            };
        }

        private static int ToInt(object? valeur) => valeur == null ? 0 : Convert.ToInt32(valeur);

        // Ligne affichée dans le DataGrid (public pour le binding WPF).
        public class LigneAnimal
        {
            public int IdAnimal { get; set; }
            public string Nom { get; set; } = "";
            public string Espece { get; set; } = "";
            public string Race { get; set; } = "";
            public string Sexe { get; set; } = "";
            public string Age { get; set; } = "";
            public string Statut { get; set; } = "";
        }
    }
}
