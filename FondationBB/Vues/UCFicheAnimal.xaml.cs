using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FondationBB
{
    // Fiche détaillée d'un animal : informations, comportements, soins, et actions.
    public partial class UCFicheAnimal : UserControl
    {
        private readonly AnimalDAO animalDao = new AnimalDAO();
        private Animal? animal;

        public UCFicheAnimal(int idAnimal)
        {
            InitializeComponent();
            Charger(idAnimal);
        }

        private void Charger(int idAnimal)
        {
            try
            {
                animal = animalDao.GetAnimal(idAnimal);
                if (animal == null)
                {
                    txtNom.Text = "Animal introuvable";
                    return;
                }
                Remplir(animal);
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCFicheAnimal.Charger");
                txtNom.Text = "Erreur de chargement";
                MessageBox.Show("Impossible d'afficher la fiche de l'animal : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Remplir(Animal a)
        {
            txtNom.Text = a.NomAnimal;

            string statut = a.Statut?.LibelleStatut ?? "—";
            txtStatut.Text = statut;
            txtStatut.Foreground = StatutVisuel.Texte(statut);
            bdStatut.Background = StatutVisuel.Fond(statut);

            AjouterInfo("Espèce", a.Race?.Espece?.LibelleEspece ?? "—");
            AjouterInfo("Race", a.Race?.LibelleRace ?? "—");
            AjouterInfo("Taille", a.Race != null ? a.Race.Taille.ToString() : "—");
            AjouterInfo("Sexe", a.Sexe == SexeAnimal.Femelle ? "Femelle" : "Mâle");
            AjouterInfo("Âge", a.AgeLisible());
            AjouterInfo("Date de naissance", a.DateNaissanceAnimal.ToString("dd/MM/yyyy"));
            AjouterInfo("Poids", $"{a.PoidsAnimal} kg");
            AjouterInfo("N° I-CAD", string.IsNullOrWhiteSpace(a.IcadAnimal) ? "—" : a.IcadAnimal);
            AjouterInfo("Date d'arrivée", a.DateArriveeAnimal == default ? "—" : a.DateArriveeAnimal.ToString("dd/MM/yyyy"));
            AjouterInfo("État de santé", a.Etat?.LibelleEtat ?? "—");
            if (!string.IsNullOrWhiteSpace(a.AnnotationAnimal))
                AjouterInfo("Annotation", a.AnnotationAnimal);

            // Comportements sous forme de "chips".
            if (a.Comportements.Count == 0)
                wrapComportements.Children.Add(TexteSecondaire("Aucun comportement renseigné."));
            foreach (Comportement c in a.Comportements)
                wrapComportements.Children.Add(Chip(c.LibelleComportement));

            // Soins reçus.
            if (a.SoinsRecus.Count == 0)
                panneauSoins.Children.Add(TexteSecondaire("Aucun soin enregistré."));
            foreach (SoinRecu sr in a.SoinsRecus)
            {
                string rappel = sr.DateRappel.HasValue ? $"  (rappel : {sr.DateRappel:dd/MM/yyyy})" : "";
                panneauSoins.Children.Add(new TextBlock
                {
                    Text = $"{sr.DateSoin:dd/MM/yyyy} — {sr.Soin?.LibelleSoin}{rappel}",
                    Margin = new Thickness(0, 3, 0, 3),
                    FontSize = 13
                });
            }

            // On ne propose l'adoption que si l'animal est disponible.
            butAdopter.Visibility = a.EstDisponible() ? Visibility.Visible : Visibility.Collapsed;
        }

        // Ajoute une ligne "libellé / valeur" au panneau d'informations.
        private void AjouterInfo(string libelle, string valeur)
        {
            Grid g = new Grid { Margin = new Thickness(0, 5, 0, 5) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock l = new TextBlock { Text = libelle, FontSize = 12, Foreground = Pinceau("#6B7280") };
            TextBlock v = new TextBlock { Text = valeur, FontSize = 13, TextWrapping = TextWrapping.Wrap };
            Grid.SetColumn(v, 1);
            g.Children.Add(l);
            g.Children.Add(v);
            panneauInfos.Children.Add(g);
        }

        private Border Chip(string texte)
        {
            return new Border
            {
                Background = Pinceau("#E6F1FB"),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12, 5, 12, 5),
                Margin = new Thickness(0, 0, 8, 8),
                Child = new TextBlock { Text = texte, FontSize = 12, Foreground = Pinceau("#0C447C") }
            };
        }

        private TextBlock TexteSecondaire(string texte) =>
            new TextBlock { Text = texte, FontSize = 13, Foreground = Pinceau("#6B7280") };

        private void butModifier_Click(object sender, RoutedEventArgs e)
        {
            if (animal != null) Navigateur.Afficher(new UCAnimalFormulaire(animal));
        }

        private void butAdopter_Click(object sender, RoutedEventArgs e)
        {
            if (animal != null) Navigateur.Afficher(new UCContratFormulaire(animal));
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            Navigateur.Afficher(new UCCatalogueAnimaux());
        }

        private static SolidColorBrush Pinceau(string hex) =>
            new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
    }
}
