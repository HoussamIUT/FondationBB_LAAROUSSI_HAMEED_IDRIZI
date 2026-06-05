using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FondationBB
{
    // Fenêtre principale : héberge l'écran de connexion puis le shell (barre latérale + contenu).
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // La zone de contenu sert aussi de cible aux sous-écrans (fiche, formulaires).
            Navigateur.Zone = Zone;
            AfficherLogin();
        }

        // Affiche l'écran de connexion plein cadre et masque le shell.
        private void AfficherLogin()
        {
            UCLogin login = new UCLogin();
            login.OnConnecte = OnConnecte;   // callback déclenché après authentification réussie
            ZoneLogin.Content = login;
            ZoneLogin.Visibility = Visibility.Visible;
            RacineApp.Visibility = Visibility.Collapsed;
        }

        // Appelé par UCLogin une fois l'employé authentifié et stocké dans Session.
        private void OnConnecte()
        {
            ZoneLogin.Visibility = Visibility.Collapsed;
            ZoneLogin.Content = null;
            RacineApp.Visibility = Visibility.Visible;

            // En-tête utilisateur
            Employe? e = Session.EmployeConnecte;
            txtUtilisateur.Text = e?.NomComplet ?? "";
            txtRole.Text = Session.EstResponsable ? "Responsable" : "Bénévole";

            // Gestion des droits : les écrans responsables sont masqués pour un bénévole.
            Visibility droitResp = Session.EstResponsable ? Visibility.Visible : Visibility.Collapsed;
            navCorrespondances.Visibility = droitResp;
            navStatistiques.Visibility = droitResp;

            // Écran d'accueil par défaut
            Naviguer(navAnimaux, "Animaux", new UCCatalogueAnimaux());
        }

        // Place un écran dans la zone de contenu et met l'item de nav correspondant en actif.
        private void Naviguer(Button itemActif, string titre, UserControl ecran)
        {
            Zone.Content = ecran;
            ActiverNav(itemActif);
        }

        // Réinitialise le visuel de tous les items puis surligne l'item actif (état primaryLight).
        private void ActiverNav(Button actif)
        {
            Button[] items = { navAnimaux, navAdoptions, navDemandes, navCorrespondances, navStatistiques };
            foreach (Button b in items)
            {
                b.Background = Brushes.Transparent;
                b.Foreground = (Brush)FindResource("BrushTextSec");
                b.FontWeight = FontWeights.Normal;
            }
            actif.Background = (Brush)FindResource("BrushPrimaryLight");
            actif.Foreground = (Brush)FindResource("BrushPrimary");
            actif.FontWeight = FontWeights.Medium;
        }

        private void navAnimaux_Click(object sender, RoutedEventArgs e)
            => Naviguer(navAnimaux, "Animaux", new UCCatalogueAnimaux());

        private void navAdoptions_Click(object sender, RoutedEventArgs e)
            => Naviguer(navAdoptions, "Historique des adoptions", new UCHistoriqueAdoptions());

        private void navDemandes_Click(object sender, RoutedEventArgs e)
            => Naviguer(navDemandes, "Nouvelle demande d'adoption", new UCDemandes());

        // Les deux écrans suivants sont réservés au responsable (double sécurité avec le masquage).
        private void navCorrespondances_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.EstResponsable) return;
            Naviguer(navCorrespondances, "Correspondances demandes / animaux", new UCCorrespondances());
        }

        private void navStatistiques_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.EstResponsable) return;
            Naviguer(navStatistiques, "Tableau de bord", new UCDashboard());
        }

        private void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            Session.Deconnecter();
            AfficherLogin();
        }
    }
}
