using System;

namespace FondationBB
{
    // Énumérations de la couche modèle.
    // On utilise des enums pour les valeurs FIGÉES (typage fort, pas de saisie libre).
    // Les listes ÉDITABLES côté BD (Statut, Etat, Comportement, Soin) restent des classes.

    public enum SexeAnimal
    {
        Male,
        Femelle
    }

    public enum RoleEmploye
    {
        Benevole,
        Responsable
    }

    public enum TailleRace
    {
        Petit,
        Moyen,
        Grand
    }

    // Tranches d'âge utilisées pour les demandes et le matching.
    // Junior < 2 ans ; Adulte 2 à 7 ans ; Senior > 7 ans.
    public enum TrancheAge
    {
        Junior,
        Adulte,
        Senior
    }
}
