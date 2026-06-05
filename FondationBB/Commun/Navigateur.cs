using System.Windows.Controls;

namespace FondationBB
{
    // Navigation interne à la zone de contenu : permet à un écran d'en ouvrir un autre
    // (ex. catalogue -> fiche animal -> formulaire) sans connaître la fenêtre principale.
    public static class Navigateur
    {
        public static ContentControl? Zone { get; set; }

        public static void Afficher(UserControl ecran)
        {
            if (Zone != null) Zone.Content = ecran;
        }
    }
}
