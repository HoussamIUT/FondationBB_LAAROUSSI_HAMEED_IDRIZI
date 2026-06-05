using System;
using System.Data;
using Npgsql;

namespace FondationBB
{
    // Authentification par les droits du SGBD (GRANT) :
    //   1. on initialise la connexion avec le login/mot de passe saisis ;
    //   2. si la connexion s'ouvre, l'utilisateur est authentifié ;
    //   3. on lit son rôle dans la table employe (login_employe = login de connexion).
    //
    // Schéma supposé : employe(id_employe, nom_employe, prenom_employe, login_employe, role_employe)
    public class AuthDAO
    {
        // Renvoie l'employé connecté, ou null si l'authentification échoue.
        public Employe? Connecter(string login, string password)
        {
            try
            {
                DataAcces.InitializeConnection(login, password);
                DataAcces.GetConnection(); // ouvre réellement la connexion -> lève si identifiants invalides

                NpgsqlCommand cmd = new NpgsqlCommand(@"
                    SELECT id_employe, nom_employe, prenom_employe, login_employe, role_employe
                    FROM employe
                    WHERE login_employe = @login");
                cmd.Parameters.AddWithValue("@login", login);

                DataTable dt = DataAcces.ExecuteSelect(cmd);
                if (dt.Rows.Count == 0)
                    return null; // connecté au SGBD mais aucune fiche employé associée

                DataRow row = dt.Rows[0];
                return new Employe(
                    Convert.ToInt32(row["id_employe"]),
                    row["nom_employe"].ToString() ?? "",
                    row["prenom_employe"].ToString() ?? "",
                    row["login_employe"].ToString() ?? "",
                    Conversions.ToRole(row["role_employe"]));
            }
            catch (Exception ex)
            {
                // Identifiants invalides ou serveur injoignable : on journalise et on refuse.
                LogError.Log(ex, "Échec d'authentification pour le login : " + login);
                return null;
            }
        }
    }
}
