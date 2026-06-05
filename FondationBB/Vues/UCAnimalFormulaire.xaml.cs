using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FondationBB
{
    // Formulaire d'enregistrement (responsable) et de modification d'un animal.
    public partial class UCAnimalFormulaire : UserControl
    {
        private readonly AnimalDAO animalDao = new AnimalDAO();
        private readonly ReferenceDAO refDao = new ReferenceDAO();

        // Associe chaque case à cocher au comportement correspondant.
        private readonly Dictionary<CheckBox, Comportement> casesComportement = new Dictionary<CheckBox, Comportement>();

        private readonly Animal? animalAModifier;   // null = création
        private bool initialise = false;

        // Constructeur création.
        public UCAnimalFormulaire() : this(null) { }

        // Constructeur modification (animal pré-rempli).
        public UCAnimalFormulaire(Animal? animal)
        {
            InitializeComponent();
            animalAModifier = animal;
            Initialiser();
        }

        private void Initialiser()
        {
            try
            {
                // Sexe (valeurs figées).
                cbSexe.Items.Add("Mâle");
                cbSexe.Items.Add("Femelle");
                cbSexe.SelectedIndex = 0;

                // Listes de référence.
                cbEspece.ItemsSource = refDao.GetEspeces();
                cbStatut.ItemsSource = refDao.GetStatuts();
                cbEtat.ItemsSource = refDao.GetEtats();

                // Cases comportements générées depuis la base.
                foreach (Comportement c in refDao.GetComportements())
                {
                    CheckBox cb = new CheckBox
                    {
                        Content = c.LibelleComportement,
                        Width = 200,
                        Margin = new Thickness(0, 4, 0, 4)
                    };
                    casesComportement[cb] = c;
                    wrapComportements.Children.Add(cb);
                }

                initialise = true;

                if (animalAModifier != null)
                    PreRemplir(animalAModifier);
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCAnimalFormulaire.Initialiser");
                MessageBox.Show("Impossible de charger le formulaire : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Recharge les races quand l'espèce change.
        private void cbEspece_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initialise) return;
            int idEspece = cbEspece.SelectedValue == null ? 0 : Convert.ToInt32(cbEspece.SelectedValue);
            cbRace.ItemsSource = idEspece == 0 ? refDao.GetRaces() : refDao.GetRaces(idEspece);
        }

        // Pré-remplissage en mode modification + verrouillage des champs non modifiables.
        private void PreRemplir(Animal a)
        {
            txtTitreForm.Text = "Modifier l'animal";
            butSauvegarder.Content = "Enregistrer les modifications";

            txtNom.Text = a.NomAnimal;
            cbEspece.SelectedValue = a.Race?.Espece?.IdEspece;   // déclenche le chargement des races
            cbRace.SelectedValue = a.Race?.IdRace;
            cbSexe.SelectedIndex = a.Sexe == SexeAnimal.Femelle ? 1 : 0;
            dpNaissance.SelectedDate = a.DateNaissanceAnimal;
            dpArrivee.SelectedDate = a.DateArriveeAnimal == default ? null : a.DateArriveeAnimal;
            txtPoids.Text = a.PoidsAnimal.ToString(CultureInfo.InvariantCulture);
            txtIcad.Text = a.IcadAnimal;
            cbStatut.SelectedValue = a.Statut?.IdStatut;
            cbEtat.SelectedValue = a.Etat?.IdEtat;
            txtAnnotation.Text = a.AnnotationAnimal;

            // Cocher les comportements existants.
            foreach (var paire in casesComportement)
                foreach (Comportement c in a.Comportements)
                    if (c.IdComportement == paire.Value.IdComportement) paire.Key.IsChecked = true;

            // Ces champs identifient l'animal : non modifiables ici (cf. AnimalDAO.ModifierAnimal).
            cbEspece.IsEnabled = false;
            cbRace.IsEnabled = false;
            cbSexe.IsEnabled = false;
            dpNaissance.IsEnabled = false;
            dpArrivee.IsEnabled = false;
            txtIcad.IsEnabled = false;
            wrapComportements.IsEnabled = false;
        }

        private void butSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            // --- Validation des champs obligatoires ---
            if (string.IsNullOrWhiteSpace(txtNom.Text)) { Erreur("Le nom est obligatoire."); return; }
            if (cbRace.SelectedItem is not Race race) { Erreur("Veuillez choisir une race."); return; }
            if (dpNaissance.SelectedDate == null) { Erreur("La date de naissance est obligatoire."); return; }
            if (cbStatut.SelectedItem is not Statut statut) { Erreur("Veuillez choisir un statut."); return; }

            // Le poids est obligatoire et strictement positif (contrainte CHECK en base).
            if (!double.TryParse(txtPoids.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double poids)
                || poids <= 0)
            {
                Erreur("Le poids doit être un nombre supérieur à 0."); return;
            }

            try
            {
                Animal a = animalAModifier ?? new Animal();
                a.NomAnimal = txtNom.Text.Trim();
                a.Race = race;
                a.Sexe = cbSexe.SelectedIndex == 1 ? SexeAnimal.Femelle : SexeAnimal.Male;
                a.DateNaissanceAnimal = dpNaissance.SelectedDate.Value;
                a.DateArriveeAnimal = dpArrivee.SelectedDate ?? DateTime.Today;
                a.PoidsAnimal = poids;
                a.IcadAnimal = txtIcad.Text.Trim();
                a.AnnotationAnimal = txtAnnotation.Text.Trim();
                a.Statut = statut;
                a.Etat = cbEtat.SelectedItem as Etat;
                a.Employe = Session.EmployeConnecte;

                // Comportements cochés (utilisés à la création).
                a.Comportements = new List<Comportement>();
                foreach (var paire in casesComportement)
                    if (paire.Key.IsChecked == true) a.Comportements.Add(paire.Value);

                if (animalAModifier == null)
                    animalDao.CreerAnimal(a);
                else
                    animalDao.ModifierAnimal(a);

                // Confirmation puis retour au catalogue.
                MessageBox.Show(
                    animalAModifier == null ? "L'animal a bien été enregistré." : "Les modifications ont été enregistrées.",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                Navigateur.Afficher(new UCCatalogueAnimaux());
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "UCAnimalFormulaire.Sauvegarder");
                MessageBox.Show("Erreur lors de l'enregistrement : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void butAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Navigateur.Afficher(new UCCatalogueAnimaux());
        }

        private void Erreur(string message)
            => MessageBox.Show(message, "Champs obligatoires", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
