using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FondationBB
{
    /// <summary>
    /// Tableau de bord statistiques. Graphiques dessinés en WPF natif
    /// (pas de package additionnel, conformément à la consigne).
    /// Auteur : Idrizi.
    /// </summary>
    public partial class UCDashboard : UserControl
    {
        private static readonly string[] MoisLabels =
            { "J", "F", "M", "A", "M", "J", "J", "A", "S", "O", "N", "D" };

        public UCDashboard()
        {
            InitializeComponent();
            Loaded += (s, e) => ChargerStats();
        }

        private void ChargerStats()
        {
            try
            {
                kpiPresents.Text = StatsDAO.CompterAnimauxPresents().ToString();
                kpiDispo.Text = StatsDAO.CompterAnimauxDisponibles().ToString();
                kpiAdoptions.Text = StatsDAO.CompterAdoptions().ToString();
                kpiDemandes.Text = StatsDAO.CompterDemandesEnAttente().ToString();

                DessinerBarres(StatsDAO.AdoptionsParMois(DateTime.Now.Year));
                DessinerStatuts(StatsDAO.RepartitionParStatut());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des statistiques :\n" + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Dessine un histogramme des adoptions par mois dans la grille zoneBarres.
        /// </summary>
        private void DessinerBarres(int[] valeurs)
        {
            zoneBarres.Children.Clear();
            zoneBarres.ColumnDefinitions.Clear();

            int max = valeurs.Length > 0 ? valeurs.Max() : 0;
            if (max == 0) max = 1; // évite la division par zéro

            for (int i = 0; i < 12; i++)
            {
                zoneBarres.ColumnDefinitions.Add(new ColumnDefinition());

                // Conteneur vertical : barre + label mois
                Grid colonne = new Grid();
                colonne.RowDefinitions.Add(new RowDefinition()); // barre
                colonne.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) }); // label

                // Barre (alignée en bas, hauteur proportionnelle)
                double ratio = (double)valeurs[i] / max;
                Border barre = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x3C, 0x3F, 0x87)),
                    CornerRadius = new CornerRadius(3, 3, 0, 0),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(4, 0, 4, 0),
                    Height = Math.Max(2, ratio * 180),
                    ToolTip = $"{valeurs[i]} adoption(s)"
                };
                Grid.SetRow(barre, 0);
                colonne.Children.Add(barre);

                TextBlock lbl = new TextBlock
                {
                    Text = MoisLabels[i],
                    FontSize = 11,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(lbl, 1);
                colonne.Children.Add(lbl);

                Grid.SetColumn(colonne, i);
                zoneBarres.Children.Add(colonne);
            }
        }

        /// <summary>
        /// Affiche la répartition par statut sous forme de barres horizontales.
        /// </summary>
        private void DessinerStatuts(List<KeyValuePair<string, int>> donnees)
        {
            zoneStatuts.Children.Clear();
            int total = donnees.Sum(d => d.Value);
            if (total == 0) total = 1;

            // Couleur par statut (cohérent avec la charte des écrans)
            Dictionary<string, Color> couleurs = new Dictionary<string, Color>
            {
                { "Disponible", Color.FromRgb(0x4C, 0xA0, 0x32) },
                { "Réservé",    Color.FromRgb(0xD9, 0x8A, 0x1A) },
                { "En soin",    Color.FromRgb(0x18, 0x5F, 0xA5) },
                { "Adopté",     Color.FromRgb(0x95, 0x95, 0x95) },
                { "Décédé",     Color.FromRgb(0x55, 0x55, 0x55) }
            };

            foreach (var d in donnees)
            {
                Color c = couleurs.ContainsKey(d.Key)
                    ? couleurs[d.Key]
                    : Color.FromRgb(0x3C, 0x3F, 0x87);
                double pourcent = (double)d.Value / total;

                StackPanel ligne = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };

                // Ligne libellé + valeur
                DockPanel entete = new DockPanel();
                TextBlock lbl = new TextBlock { Text = d.Key, FontSize = 13, Foreground = Brushes.Black };
                TextBlock val = new TextBlock
                {
                    Text = $"{d.Value}",
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                DockPanel.SetDock(val, Dock.Right);
                entete.Children.Add(val);
                entete.Children.Add(lbl);
                ligne.Children.Add(entete);

                // Barre de progression (fond gris + remplissage coloré)
                Border fond = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0xEC, 0xEC, 0xEC)),
                    CornerRadius = new CornerRadius(4),
                    Height = 10,
                    Margin = new Thickness(0, 4, 0, 0)
                };
                Grid g = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch };
                Border remplissage = new Border
                {
                    Background = new SolidColorBrush(c),
                    CornerRadius = new CornerRadius(4),
                    Height = 10,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    // largeur fixée après rendu via le ratio sur la largeur dispo
                    Width = double.NaN
                };
                // On utilise un ColumnDefinition en étoile pour le ratio
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(pourcent, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1 - pourcent, GridUnitType.Star) });
                Border barreColoree = new Border
                {
                    Background = new SolidColorBrush(c),
                    CornerRadius = new CornerRadius(4),
                    Height = 10
                };
                Grid.SetColumn(barreColoree, 0);
                g.Children.Add(barreColoree);
                fond.Child = g;
                ligne.Children.Add(fond);

                zoneStatuts.Children.Add(ligne);
            }
        }
    }
}
