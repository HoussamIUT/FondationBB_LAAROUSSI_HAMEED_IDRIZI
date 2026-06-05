using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FondationBB
{
    // Écran de connexion. Authentifie via les droits du SGBD puis prévient la fenêtre
    // principale au moyen du callback OnConnecte.
    public partial class UCLogin : UserControl
    {
        private readonly AuthDAO authDao = new AuthDAO();

        // Renseigné par MainWindow : appelé après une authentification réussie.
        public Action? OnConnecte { get; set; }

        public UCLogin()
        {
            InitializeComponent();
            // Validation au clavier (touche Entrée).
            KeyDown += (s, e) => { if (e.Key == Key.Enter) Connecter(); };
        }

        private void butConnecter_Click(object sender, RoutedEventArgs e) => Connecter();

        private void Connecter()
        {
            string login = txtLogin.Text.Trim();
            string motDePasse = pwdMotDePasse.Password;

            if (login.Length == 0 || motDePasse.Length == 0)
            {
                MessageBox.Show("Veuillez saisir un identifiant et un mot de passe.",
                    "Champs obligatoires", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Employe? employe = authDao.Connecter(login, motDePasse);
            if (employe == null)
            {
                MessageBox.Show(
                    "La connexion a échoué.\n\n" +
                    "Vérifiez votre identifiant et votre mot de passe, et que votre compte " +
                    "PostgreSQL a bien accès à la base.",
                    "Connexion impossible", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Authentification réussie : on prévient l'utilisateur puis on bascule vers l'application.
            MessageBox.Show("Connexion réussie !", "Succès",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Session.EmployeConnecte = employe;
            OnConnecte?.Invoke();
        }
    }
}
