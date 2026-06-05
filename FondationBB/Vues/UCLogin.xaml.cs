using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace FondationBB
{
    /// <summary>
    /// Logique d'interaction pour UCLogin.xaml
    /// </summary>
    public partial class UCLogin : UserControl
    {
        public event EventHandler LoginReussi;
        public UCLogin()
        {
            InitializeComponent();
        }
        private void AfficherCatalogueAnimaux(object sender, RoutedEventArgs e)
        {
            //UCCatalogueAnimaux uc = new UCCatalogueAnimaux();
            //Accueil.Content = uc;
            //uc.butAnimaux.Click += AfficherJeu;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void butConnecter_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez saisir vos identifiants.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // On remplace la vérification "admin"/"1234" par la vraie connexion BDD
                DataAcces.InitializeConnection(username, password);
                var connection = DataAcces.GetConnection();
                MessageBox.Show("connecté", "valide", MessageBoxButton.OK, MessageBoxImage.Information);
                // Si on arrive ici, la connexion PostgreSQL a réussi !
                LoginReussi?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception)
            {
                // En cas d'erreur de connexion à la BDD
                MessageBox.Show("Identifiants incorrects ou serveur indisponible.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); // [cite: 36]
            }
        }
    }
}
