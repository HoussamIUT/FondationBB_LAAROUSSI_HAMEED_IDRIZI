using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    // Création d'une demande d'adoption (préférences + coordonnées du demandeur).
    public partial class UCDemandes : UserControl
    {
        private readonly ReferenceDAO refDao = new ReferenceDAO();
        private readonly PersonneDAO personneDao = new PersonneDAO();
        private readonly DemandeDAO demandeDao = new DemandeDAO();
        private bool initialise = false;

        public UCDemandes()
        {
            InitializeComponent();
            Initialiser();
        }

        private void Initialiser()
        {
            try
            {
                // Espèce (avec entrée "indifférente").
                List<Espece> especes = refDao.GetEspeces();
                especes.Insert(0, new Espece(0, "Indifférente"));
                cbEspece.ItemsSource = especes;
                cbEspece.SelectedIndex = 0;

                ChargerRaces(null);

                // Tranches d'âge = valeurs de l'enum TrancheAge.
                foreach (TrancheAge t in Enum.GetValues(typeof(TrancheAge)))
                    cbTranche.Items.Add(t.ToString());
                cbTranche.SelectedIndex = 0;

                initialise = true;
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCDemandes.Initialiser");
                MessageBox.Show("Impossible de charger le formulaire : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChargerRaces(int? idEspece)
        {
            List<Race> races = refDao.GetRaces(idEspece);
            races.Insert(0, new Race(0, "Indifférente", TailleRace.Petit, null));
            cbRace.ItemsSource = races;
            cbRace.SelectedIndex = 0;
        }

        private void cbEspece_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initialise) return;
            int idEspece = cbEspece.SelectedValue == null ? 0 : Convert.ToInt32(cbEspece.SelectedValue);
            ChargerRaces(idEspece == 0 ? (int?)null : idEspece);
        }

        private void butValider_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                Erreur("Le nom et le prénom sont obligatoires."); return;
            }
            // Téléphone obligatoire : NOT NULL + UNIQUE en base (sert aussi de clé de rapprochement).
            if (string.IsNullOrWhiteSpace(txtTelephone.Text))
            {
                Erreur("Le téléphone est obligatoire."); return;
            }

            try
            {
                // Réutilise ou crée le demandeur (rapprochement par téléphone).
                string cle = txtTelephone.Text.Trim();
                Personne? personne = personneDao.Rechercher(cle);
                if (personne == null)
                {
                    Personne nouvelle = new Personne
                    {
                        NomPersonne = txtNom.Text.Trim(),
                        PrenomPersonne = txtPrenom.Text.Trim(),
                        TelephonePersonne = txtTelephone.Text.Trim(),
                        MailPersonne = txtMail.Text.Trim()
                    };
                    nouvelle.IdPersonne = personneDao.CreerPersonne(nouvelle);
                    personne = nouvelle;
                }

                // Race facultative (id 0 = indifférente).
                int idRace = cbRace.SelectedValue == null ? 0 : Convert.ToInt32(cbRace.SelectedValue);
                int? idRaceParam = idRace == 0 ? (int?)null : idRace;

                TrancheAge tranche = Conversions.ToTranche(cbTranche.SelectedItem?.ToString());

                demandeDao.CreerDemande(personne.IdPersonne, tranche, idRaceParam);

                MessageBox.Show("La demande d'adoption a bien été enregistrée.",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                butReset_Click(sender, e);   // vide le formulaire pour une nouvelle demande
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCDemandes.Valider");
                MessageBox.Show("Erreur lors de l'enregistrement de la demande : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void butReset_Click(object sender, RoutedEventArgs e)
        {
            txtNom.Text = txtPrenom.Text = txtTelephone.Text = txtMail.Text = "";
            cbEspece.SelectedIndex = 0;
            cbTranche.SelectedIndex = 0;
            txtMessage.Text = "";
        }

        private void Erreur(string message)
            => MessageBox.Show(message, "Champs obligatoires", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
