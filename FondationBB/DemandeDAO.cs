using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace FondationBB
{
    /// <summary>
    /// Accès aux données pour les Demandes d'adoption (liste d'attente).
    /// Auteur : Idrizi.
    /// </summary>
    public class DemandeDAO
    {
        /// <summary>
        /// Récupère toutes les demandes en attente, jointes à la personne et à la race souhaitée.
        /// La race est optionnelle (LEFT JOIN) : une demande peut ne pas préciser de race.
        /// </summary>
        public static List<Demande> GetToutesDemandes()
        {
            List<Demande> liste = new List<Demande>();

            string sql = @"
                SELECT d.id_demande,
                       d.date_adoption_demande,
                       d.tranche_age_demande,
                       p.id_personne, p.nom_personne, p.prenom_personne,
                       p.telephone_personne, p.mail_personne,
                       r.id_race, r.libelle_race, r.taille_race,
                       e.id_espece, e.libelle_espece
                FROM demande d
                JOIN personne p ON p.id_personne = d.id_personne
                LEFT JOIN race r ON r.id_race = d.id_race
                LEFT JOIN espece e ON e.id_espece = r.id_espece
                ORDER BY d.date_adoption_demande ASC";

            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            DataTable dt = DataAcces.ExecuteSelect(cmd);

            foreach (DataRow row in dt.Rows)
            {
                Personne personne = new Personne(
                    Convert.ToInt32(row["id_personne"]),
                    row["nom_personne"].ToString(),
                    row["prenom_personne"].ToString(),
                    row["telephone_personne"].ToString(),
                    row["mail_personne"].ToString());

                Race race = null;
                if (row["id_race"] != DBNull.Value)
                {
                    Espece espece = new Espece(
                        Convert.ToInt32(row["id_espece"]),
                        row["libelle_espece"].ToString());
                    race = new Race(
                        Convert.ToInt32(row["id_race"]),
                        row["libelle_race"].ToString(),
                        row["taille_race"].ToString(),
                        espece);
                }

                Demande d = new Demande(
                    Convert.ToInt32(row["id_demande"]),
                    Convert.ToDateTime(row["date_adoption_demande"]),
                    row["tranche_age_demande"].ToString(),
                    personne,
                    race);

                liste.Add(d);
            }
            return liste;
        }

        /// <summary>
        /// Crée une nouvelle demande. Retourne l'id généré.
        /// id_race peut être null si pas de préférence de race.
        /// </summary>
        public static int CreerDemande(int idPersonne, string trancheAge, int? idRace)
        {
            string sql = @"
                INSERT INTO demande (date_adoption_demande, tranche_age_demande, id_personne, id_race)
                VALUES (@date, @tranche, @idPersonne, @idRace)
                RETURNING id_demande";

            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@tranche", trancheAge);
            cmd.Parameters.AddWithValue("@idPersonne", idPersonne);
            cmd.Parameters.AddWithValue("@idRace", (object)idRace ?? DBNull.Value);

            return DataAcces.ExecuteInsert(cmd);
        }

        /// <summary>Supprime une demande (par exemple une fois satisfaite).</summary>
        public static int SupprimerDemande(int idDemande)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM demande WHERE id_demande = @id");
            cmd.Parameters.AddWithValue("@id", idDemande);
            return DataAcces.ExecuteSet(cmd);
        }
    }
}
