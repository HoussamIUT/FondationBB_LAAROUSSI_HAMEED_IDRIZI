using System;

namespace FondationBB
{
    // Utilisateur de l'application : bénévole ou responsable.
    // Pas de mot de passe stocké : l'authentification se fait via les droits (GRANT) du SGBD.
    public class Employe
    {
        public int IdEmploye { get; set; }
        public string NomEmploye { get; set; } = "";
        public string PrenomEmploye { get; set; } = "";
        public string LoginEmploye { get; set; } = "";
        public RoleEmploye Role { get; set; }

        public Employe() { }

        public Employe(int idEmploye, string nomEmploye, string prenomEmploye,
                       string loginEmploye, RoleEmploye role)
        {
            IdEmploye = idEmploye;
            NomEmploye = nomEmploye;
            PrenomEmploye = prenomEmploye;
            LoginEmploye = loginEmploye;
            Role = role;
        }

        public string NomComplet => $"{PrenomEmploye} {NomEmploye}".Trim();

        // Le responsable a des droits étendus (enregistrer animal, correspondances, stats).
        public bool EstResponsable() => Role == RoleEmploye.Responsable;
    }
}
