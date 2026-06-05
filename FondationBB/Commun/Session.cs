using System;

namespace FondationBB
{
    // Porteur global de l'utilisateur connecté (renseigné après authentification réussie).
    // Sert à la gestion des droits : seules les fonctionnalités autorisées sont accessibles.
    public static class Session
    {
        public static Employe? EmployeConnecte { get; set; }

        // true si un responsable est connecté (droits étendus).
        public static bool EstResponsable =>
            EmployeConnecte != null && EmployeConnecte.EstResponsable();

        public static void Deconnecter()
        {
            EmployeConnecte = null;
            DataAcces.CloseConnection();
        }
    }
}
