using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    // Consultation de l'historique des adoptions (accessible à tous).
    public partial class UCHistoriqueAdoptions : UserControl
    {
        private readonly AdoptionDAO adoptionDao = new AdoptionDAO();

        public UCHistoriqueAdoptions()
        {
            InitializeComponent();
            Charger();
        }

        private void Charger()
        {
            try
            {
                List<Adoption> adoptions = adoptionDao.GetHistorique();
                List<LigneAdoption> lignes = new List<LigneAdoption>();
                foreach (Adoption ad in adoptions)
                {
                    lignes.Add(new LigneAdoption
                    {
                        Date = ad.DateAdoption.ToString("dd/MM/yyyy"),
                        Animal = ad.Animal?.NomAnimal ?? "—",
                        Adoptant = ad.Personne?.NomComplet ?? "—",
                        Contact = !string.IsNullOrWhiteSpace(ad.Personne?.TelephonePersonne)
                                    ? ad.Personne!.TelephonePersonne
                                    : ad.Personne?.MailPersonne ?? "",
                        Frais = $"{ad.FraisAdoption:0} €"
                    });
                }
                grilleAdoptions.ItemsSource = lignes;
                txtTotal.Text = $"{lignes.Count} adoption(s) enregistrée(s).";
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCHistoriqueAdoptions.Charger");
                txtTotal.Text = "Erreur lors du chargement de l'historique.";
                MessageBox.Show("Erreur lors du chargement de l'historique : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class LigneAdoption
        {
            public string Date { get; set; } = "";
            public string Animal { get; set; } = "";
            public string Adoptant { get; set; } = "";
            public string Contact { get; set; } = "";
            public string Frais { get; set; } = "";
        }
    }
}
