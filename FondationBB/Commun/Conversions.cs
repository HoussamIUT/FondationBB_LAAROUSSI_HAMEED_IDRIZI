using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    // Conversions entre les valeurs stockées en base (chaînes) et les enums du modèle.
    // Centralisé ici pour éviter la duplication dans les DAO.
    public static class Conversions
    {
        public static SexeAnimal ToSexe(object? v)
        {
            string s = v?.ToString()?.Trim().ToUpperInvariant() ?? "";
            return (s == "F" || s == "FEMELLE") ? SexeAnimal.Femelle : SexeAnimal.Male;
        }

        public static string FromSexe(SexeAnimal sexe) => sexe == SexeAnimal.Femelle ? "F" : "M";

        public static TailleRace ToTaille(object? v)
        {
            string s = v?.ToString()?.Trim().ToUpperInvariant() ?? "";
            if (s.StartsWith("G")) return TailleRace.Grand;   // GRAND
            if (s.StartsWith("M")) return TailleRace.Moyen;   // MOYEN
            return TailleRace.Petit;                          // PETIT
        }

        public static TrancheAge ToTranche(object? v)
        {
            string s = v?.ToString()?.Trim().ToLowerInvariant() ?? "";
            if (s.StartsWith("s")) return TrancheAge.Senior;
            if (s.StartsWith("a")) return TrancheAge.Adulte;
            return TrancheAge.Junior;
        }

        public static RoleEmploye ToRole(object? v)
        {
            string s = v?.ToString()?.Trim().ToLowerInvariant() ?? "";
            return s.StartsWith("r") ? RoleEmploye.Responsable : RoleEmploye.Benevole;
        }
    }
}

