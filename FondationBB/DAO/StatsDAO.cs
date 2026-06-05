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
    /// Accès aux données agrégées pour le tableau de bord statistiques.
    /// Auteur : Idrizi.
    /// </summary>
    public class StatsDAO
    {
        /// <summary>Nombre total d'animaux présents au refuge (non adoptés, non décédés).</summary>
        public static int CompterAnimauxPresents()
        {
            string sql = @"
                SELECT COUNT(*)
                FROM animal a
                JOIN statut s ON s.id_statut = a.id_statut
                WHERE s.libelle_statut NOT IN ('Adopté', 'Décédé')";
            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            string res = DataAcces.ExecuteSelectOneValue(cmd);
            return string.IsNullOrEmpty(res) ? 0 : Convert.ToInt32(res);
        }

        /// <summary>Nombre d'animaux disponibles à l'adoption.</summary>
        public static int CompterAnimauxDisponibles()
        {
            string sql = @"
                SELECT COUNT(*)
                FROM animal a
                JOIN statut s ON s.id_statut = a.id_statut
                WHERE s.libelle_statut = 'Disponible'";
            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            string res = DataAcces.ExecuteSelectOneValue(cmd);
            return string.IsNullOrEmpty(res) ? 0 : Convert.ToInt32(res);
        }

        /// <summary>Nombre total d'adoptions enregistrées.</summary>
        public static int CompterAdoptions()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT COUNT(*) FROM adoption");
            string res = DataAcces.ExecuteSelectOneValue(cmd);
            return string.IsNullOrEmpty(res) ? 0 : Convert.ToInt32(res);
        }

        /// <summary>Nombre de demandes en attente.</summary>
        public static int CompterDemandesEnAttente()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT COUNT(*) FROM demande");
            string res = DataAcces.ExecuteSelectOneValue(cmd);
            return string.IsNullOrEmpty(res) ? 0 : Convert.ToInt32(res);
        }

        /// <summary>
        /// Répartition des animaux présents par espèce.
        /// Retourne une liste de couples (libellé espèce, nombre).
        /// </summary>
        public static List<KeyValuePair<string, int>> RepartitionParEspece()
        {
            var liste = new List<KeyValuePair<string, int>>();
            string sql = @"
                SELECT e.libelle_espece, COUNT(*) AS nb
                FROM animal a
                JOIN race r   ON r.id_race = a.id_race
                JOIN espece e ON e.id_espece = r.id_espece
                JOIN statut s ON s.id_statut = a.id_statut
                WHERE s.libelle_statut NOT IN ('Adopté', 'Décédé')
                GROUP BY e.libelle_espece
                ORDER BY nb DESC";
            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            DataTable dt = DataAcces.ExecuteSelect(cmd);
            foreach (DataRow row in dt.Rows)
            {
                liste.Add(new KeyValuePair<string, int>(
                    row["libelle_espece"].ToString(),
                    Convert.ToInt32(row["nb"])));
            }
            return liste;
        }

        /// <summary>
        /// Répartition des animaux présents par statut (pour graphique en barres).
        /// </summary>
        public static List<KeyValuePair<string, int>> RepartitionParStatut()
        {
            var liste = new List<KeyValuePair<string, int>>();
            string sql = @"
                SELECT s.libelle_statut, COUNT(*) AS nb
                FROM animal a
                JOIN statut s ON s.id_statut = a.id_statut
                GROUP BY s.libelle_statut
                ORDER BY nb DESC";
            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            DataTable dt = DataAcces.ExecuteSelect(cmd);
            foreach (DataRow row in dt.Rows)
            {
                liste.Add(new KeyValuePair<string, int>(
                    row["libelle_statut"].ToString(),
                    Convert.ToInt32(row["nb"])));
            }
            return liste;
        }

        /// <summary>
        /// Adoptions par mois sur l'année en cours (pour graphique en barres).
        /// Retourne 12 valeurs indexées de janvier (0) à décembre (11).
        /// </summary>
        public static int[] AdoptionsParMois(int annee)
        {
            int[] mois = new int[12];
            string sql = @"
                SELECT EXTRACT(MONTH FROM date_adoption) AS m, COUNT(*) AS nb
                FROM adoption
                WHERE EXTRACT(YEAR FROM date_adoption) = @annee
                GROUP BY m
                ORDER BY m";
            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("@annee", annee);
            DataTable dt = DataAcces.ExecuteSelect(cmd);
            foreach (DataRow row in dt.Rows)
            {
                int m = Convert.ToInt32(row["m"]);
                if (m >= 1 && m <= 12)
                    mois[m - 1] = Convert.ToInt32(row["nb"]);
            }
            return mois;
        }
    }
}
