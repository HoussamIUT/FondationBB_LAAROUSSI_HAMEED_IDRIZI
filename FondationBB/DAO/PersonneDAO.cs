using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace FondationBB
{
    // Accès aux adoptants/demandeurs.
    //
    // Schéma supposé : personne(id_personne, nom_personne, prenom_personne, numero_personne,
    //   rue_personne, cp_personne, ville_personne, telephone_personne, mail_personne,
    //   date_naissance_personne, date_creation_personne)
    public class PersonneDAO
    {
        // Liste de tous les adoptants (pour le choix d'un adoptant existant).
        public List<Personne> GetToutes()
        {
            List<Personne> liste = new List<Personne>();
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                SELECT id_personne, nom_personne, prenom_personne, telephone_personne, mail_personne
                FROM personne
                ORDER BY nom_personne, prenom_personne");
            foreach (DataRow row in DataAcces.ExecuteSelect(cmd).Rows)
                liste.Add(new Personne(
                    Convert.ToInt32(row["id_personne"]),
                    row["nom_personne"].ToString() ?? "",
                    row["prenom_personne"].ToString() ?? "",
                    row["telephone_personne"]?.ToString()?.Trim() ?? "",
                    row["mail_personne"]?.ToString() ?? ""));
            return liste;
        }

        // Recherche un adoptant existant par téléphone ou mail (évite les doublons).
        public Personne? Rechercher(string telephoneOuMail)
        {
            // telephone_personne est un CHAR(10) (complété par des espaces) : on compare en trim().
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                SELECT id_personne, nom_personne, prenom_personne, telephone_personne, mail_personne
                FROM personne
                WHERE trim(telephone_personne) = @v OR trim(mail_personne) = @v
                LIMIT 1");
            cmd.Parameters.AddWithValue("@v", telephoneOuMail.Trim());

            DataTable dt = DataAcces.ExecuteSelect(cmd);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new Personne(
                Convert.ToInt32(row["id_personne"]),
                row["nom_personne"].ToString() ?? "",
                row["prenom_personne"].ToString() ?? "",
                row["telephone_personne"]?.ToString() ?? "",
                row["mail_personne"]?.ToString() ?? "");
        }

        // Crée un adoptant et renvoie l'id généré.
        public int CreerPersonne(Personne p)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                INSERT INTO personne (nom_personne, prenom_personne, numero_personne, rue_personne,
                                      cp_personne, ville_personne, telephone_personne, mail_personne,
                                      date_creation_personne)
                VALUES (@nom, @prenom, @num, @rue, @cp, @ville, @tel, @mail, CURRENT_DATE)
                RETURNING id_personne");
            cmd.Parameters.AddWithValue("@nom", p.NomPersonne);
            cmd.Parameters.AddWithValue("@prenom", p.PrenomPersonne);
            cmd.Parameters.AddWithValue("@num", (object?)p.NumeroPersonne ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@rue", (object?)p.RuePersonne ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cp", (object?)p.CpPersonne ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ville", (object?)p.VillePersonne ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tel", (object?)p.TelephonePersonne ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mail", (object?)p.MailPersonne ?? DBNull.Value);
            return DataAcces.ExecuteInsert(cmd);
        }
    }
}
