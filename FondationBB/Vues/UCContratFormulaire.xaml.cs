using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    // Création d'un contrat d'adoption pour un animal donné.
    // L'utilisateur choisit un adoptant existant ou en crée un nouveau,
    // saisit le montant des frais et la date de l'adoption.
    public partial class UCContratFormulaire : UserControl
    {
        private readonly PersonneDAO personneDao = new PersonneDAO();
        private readonly AdoptionDAO adoptionDao = new AdoptionDAO();

        private readonly Animal animal;

        public UCContratFormulaire(Animal animal)
        {
            InitializeComponent();
            this.animal = animal;
            Initialiser();
        }

        private void Initialiser()
        {
            // Récapitulatif de l'animal (date de naissance plutôt qu'un âge "0 an").
            txtAnimal.Text = animal.NomAnimal;
            txtAnimalMeta.Text =
                $"{animal.Race?.Espece?.LibelleEspece} · {animal.Race?.LibelleRace} · " +
                $"né(e) le {animal.DateNaissanceAnimal:dd/MM/yyyy} ({animal.AgeLisible()})";

            // Liste des adoptants existants, précédée d'une entrée "Nouvel adoptant".
            try
            {
                List<Personne> adoptants = personneDao.GetToutes();
                adoptants.Insert(0, new Personne { IdPersonne = 0, NomPersonne = "➕ Nouvel adoptant" });
                cbAdoptant.DisplayMemberPath = "NomComplet";
                cbAdoptant.ItemsSource = adoptants;
                cbAdoptant.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCContratFormulaire.Initialiser");
                MessageBox.Show("Impossible de charger la liste des adoptants : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Frais : on propose le barème automatique, mais le montant reste modifiable.
            decimal propose = Adoption.CalculerFraisAuto(animal);
            txtFrais.Text = propose.ToString("0", CultureInfo.InvariantCulture);
            txtFraisIndic.Text = $"Montant suggéré par le barème : {propose:0} € (modifiable).";

            // Date par défaut : aujourd'hui.
            dpDateAdoption.SelectedDate = DateTime.Today;
        }

        // Affiche les champs "nouvel adoptant" seulement quand cette option est choisie.
        private void cbAdoptant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool nouveau = (cbAdoptant.SelectedItem as Personne)?.IdPersonne == 0;
            panneauNouveau.Visibility = nouveau ? Visibility.Visible : Visibility.Collapsed;
        }

        private void butValider_Click(object sender, RoutedEventArgs e)
        {
            // Sécurité : on n'adopte qu'un animal disponible.
            if (!animal.EstDisponible())
            {
                MessageBox.Show("Cet animal n'est pas disponible à l'adoption.",
                    "Adoption impossible", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Personne? selection = cbAdoptant.SelectedItem as Personne;
            bool nouveau = selection == null || selection.IdPersonne == 0;

            // Montant des frais (saisie manuelle).
            if (!decimal.TryParse(txtFrais.Text.Replace(',', '.'), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out decimal frais) || frais < 0)
            {
                MessageBox.Show("Le montant des frais doit être un nombre positif.",
                    "Saisie invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Date de l'adoption.
            if (dpDateAdoption.SelectedDate == null)
            {
                MessageBox.Show("Veuillez choisir la date de l'adoption.",
                    "Champs obligatoires", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DateTime dateAdoption = dpDateAdoption.SelectedDate.Value;

            try
            {
                Personne personne;
                if (nouveau)
                {
                    if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text))
                    {
                        MessageBox.Show("Le nom et le prénom du nouvel adoptant sont obligatoires.",
                            "Champs obligatoires", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(txtTelephone.Text))
                    {
                        MessageBox.Show("Le téléphone du nouvel adoptant est obligatoire.",
                            "Champs obligatoires", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Personne nouvelle = new Personne
                    {
                        NomPersonne = txtNom.Text.Trim(),
                        PrenomPersonne = txtPrenom.Text.Trim(),
                        TelephonePersonne = txtTelephone.Text.Trim(),
                        MailPersonne = txtMail.Text.Trim(),
                        RuePersonne = txtRue.Text.Trim(),
                        CpPersonne = txtCp.Text.Trim(),
                        VillePersonne = txtVille.Text.Trim()
                    };
                    nouvelle.IdPersonne = personneDao.CreerPersonne(nouvelle);
                    personne = nouvelle;
                }
                else
                {
                    personne = selection!;
                }

                // Enregistre l'adoption (l'animal passe au statut "Adopté" côté DAO).
                Adoption adoption = new Adoption(animal, personne, Session.EmployeConnecte!, frais)
                {
                    DateAdoption = dateAdoption
                };
                adoptionDao.CreerAdoption(adoption);

                MessageBox.Show(
                    $"Adoption de {animal.NomAnimal} enregistrée ({frais:0} €) pour {personne.NomComplet}.",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                Navigateur.Afficher(new UCCatalogueAnimaux());
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCContratFormulaire.Valider");
                MessageBox.Show("Erreur lors de l'enregistrement de l'adoption : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void butAnnuler_Click(object sender, RoutedEventArgs e)
            => Navigateur.Afficher(new UCCatalogueAnimaux());
    }
}
