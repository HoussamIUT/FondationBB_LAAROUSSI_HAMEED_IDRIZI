using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FondationBB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Affichage de l'accueil au démarrage
            InitializeComponent();
            AfficheAccueil();      
        }
        private void AfficheAccueil()
        {
            // Crée un UC pour login
            UCLogin uc = new UCLogin();
            Accueil.Content = uc;
            uc.LoginReussi += Control_LoginReussi;
            
        }

        private void Control_LoginReussi(object? sender, EventArgs e)
        {
            // Le login est fini, les boutons sont visibles
            this.menuBoutons.Visibility = Visibility.Visible;

            Accueil.Content = new UCCatalogueAnimaux();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void butStatistiques_Click(object sender, RoutedEventArgs e)
        {

        }

        private void butCorrespondances_Click(object sender, RoutedEventArgs e)
        {

        }

        private void butDemandes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void butDeconnexion_Click(object sender, RoutedEventArgs e)
        {

            AfficheAccueil();
        }
    }
}