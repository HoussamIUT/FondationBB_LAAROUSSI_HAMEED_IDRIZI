using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    // Tableau de bord : indicateurs clés du refuge (réservé au responsable).
    public partial class UCDashboard : UserControl
    {
        private readonly StatsDAO statsDao = new StatsDAO();

        public UCDashboard()
        {
            InitializeComponent();
            Charger();
        }

        // Recharge les indicateurs depuis la base (lecture en temps réel).
        private void butActualiser_Click(object sender, RoutedEventArgs e) => Charger();

        private void Charger()
        {
            try
            {
                kpiPresents.Text = statsDao.CompterAnimauxPresents().ToString();
                kpiDispo.Text = statsDao.CompterAnimauxDisponibles().ToString();
                kpiAdoptions.Text = statsDao.CompterAdoptions().ToString();
                kpiDemandes.Text = statsDao.CompterDemandesEnAttente().ToString();
                kpiAdoptesMois.Text = statsDao.CompterAdoptionsCeMois().ToString();

                // Frais récoltés sur le mois civil en cours.
                CultureInfo fr = new CultureInfo("fr-FR");
                string mois = DateTime.Today.ToString("MMMM yyyy", fr);
                mois = char.ToUpper(mois[0]) + mois.Substring(1); // "Mai 2026"
                txtMoisCourant.Text = $"Mois en cours ({mois})";
                txtFraisMois.Text = $"{statsDao.FraisRecoltesCeMois():0} €";
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCDashboard.Charger");
                MessageBox.Show("Impossible de charger le tableau de bord : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
