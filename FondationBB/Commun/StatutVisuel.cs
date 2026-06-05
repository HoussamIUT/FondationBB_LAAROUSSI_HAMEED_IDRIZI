using System;
using System.Windows.Media;

namespace FondationBB
{
    // Couleurs des pastilles de statut (status pills) telles que définies dans la maquette.
    // Renvoie le fond et la couleur de texte associés à un libellé de statut.
    public static class StatutVisuel
    {
        public static Brush Fond(string libelleStatut)
        {
            switch ((libelleStatut ?? "").Trim().ToLowerInvariant())
            {
                case "disponible": return Pinceau("#EAF3DE");
                case "réservé":
                case "reserve":    return Pinceau("#FAEEDA");
                case "en soin":    return Pinceau("#E6F1FB");
                case "adopté":
                case "adopte":     return Pinceau("#EFEFEC");
                case "décédé":
                case "decede":     return Pinceau("#F0E2E2");
                default:           return Pinceau("#EFEFEC");
            }
        }

        public static Brush Texte(string libelleStatut)
        {
            switch ((libelleStatut ?? "").Trim().ToLowerInvariant())
            {
                case "disponible": return Pinceau("#27500A");
                case "réservé":
                case "reserve":    return Pinceau("#633806");
                case "en soin":    return Pinceau("#0C447C");
                case "adopté":
                case "adopte":     return Pinceau("#555555");
                case "décédé":
                case "decede":     return Pinceau("#7A2E2E");
                default:           return Pinceau("#555555");
            }
        }

        private static SolidColorBrush Pinceau(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
    }
}
