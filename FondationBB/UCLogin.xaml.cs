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
    }
}
