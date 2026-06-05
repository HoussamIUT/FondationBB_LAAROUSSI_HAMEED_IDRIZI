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
    /// Algorithme de correspondance (matching) entre une demande et les animaux disponibles.
    /// Auteur : Idrizi.
    /// Règles de correspondance :
    ///  - L'animal doit être au statut "Disponible".
    ///  - Si la demande précise une race, l'animal doit avoir cette race.
    ///  - L'âge de l'animal doit tomber dans la tranche d'âge demandée.
    /// </summary>
    public class MatchingDAO
    {
        /// <summary>
        /// Retourne les animaux disponibles qui correspondent à une demande donnée.
        /// </summary>
        public static List<Animal> GetAnimauxCorrespondants(Demande demande)
        {
            List<Animal> resultat = new List<Animal>();

            // On récupère tous les animaux disponibles avec leur race/espèce/statut,
            // puis on filtre l'âge côté C# (plus lisible et défendable en soutenance
            // que de coder les bornes d'âge en SQL).
            string sql = @"
                SELECT a.id_animal, a.nom_animal, a.date_naissance_animal, a.sexe_animal,
                       r.id_race, r.libelle_race, r.taille_race,
                       e.id_espece, e.libelle_espece,
                       s.id_statut, s.libelle_statut
                FROM animal a
                JOIN race r   ON r.id_race = a.id_race
                JOIN espece e ON e.id_espece = r.id_espece
                JOIN statut s ON s.id_statut = a.id_statut
                WHERE s.libelle_statut = 'Disponible'";

            // Filtre race si la demande en précise une
            if (demande.RaceDemande != null)
            {
                sql += " AND a.id_race = @idRace";
            }

            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            if (demande.RaceDemande != null)
            {
                cmd.Parameters.AddWithValue("@idRace", demande.RaceDemande.IdRace);
            }

            DataTable dt = DataAcces.ExecuteSelect(cmd);

            foreach (DataRow row in dt.Rows)
            {
                Espece espece = new Espece(
                    Convert.ToInt32(row["id_espece"]),
                    row["libelle_espece"].ToString());
                Race race = new Race(
                    Convert.ToInt32(row["id_race"]),
                    row["libelle_race"].ToString(),
                    row["taille_race"].ToString(),
                    espece);
                Statut statut = new Statut(
                    Convert.ToInt32(row["id_statut"]),
                    row["libelle_statut"].ToString());

                Animal a = new Animal(
                    Convert.ToInt32(row["id_animal"]),
                    row["nom_animal"].ToString(),
                    Convert.ToDateTime(row["date_naissance_animal"]),
                    row["sexe_animal"].ToString(),
                    race,
                    statut);

                // Filtre tranche d'âge
                if (CorrespondTrancheAge(a, demande.TrancheAgeDemande))
                {
                    resultat.Add(a);
                }
            }
            return resultat;
        }

        /// <summary>
        /// Vérifie si l'âge de l'animal correspond à la tranche d'âge demandée.
        /// Les libellés de tranche doivent correspondre à ceux du jeu de tests.
        /// </summary>
        private static bool CorrespondTrancheAge(Animal animal, string tranche)
        {
            if (string.IsNullOrWhiteSpace(tranche)) return true; // pas de préférence
            int age = animal.CalculerAge();

            // On normalise pour être tolérant sur la casse / accents éventuels
            string t = tranche.Trim().ToLower();

            if (t.Contains("jeune") || t.Contains("chiot") || t.Contains("chaton"))
                return age < 2;
            if (t.Contains("adulte"))
                return age >= 2 && age <= 7;
            if (t.Contains("senior") || t.Contains("âgé") || t.Contains("age"))
                return age > 7;

            return true; // valeur inconnue : on ne filtre pas plutôt que de tout exclure
        }

        /// <summary>
        /// Pour le tableau de bord responsable : compte les demandes qui ont
        /// au moins un animal correspondant disponible.
        /// </summary>
        public static int CompterDemandesSatisfaisables()
        {
            int compteur = 0;
            foreach (Demande d in DemandeDAO.GetToutesDemandes())
            {
                if (GetAnimauxCorrespondants(d).Count > 0)
                    compteur++;
            }
            return compteur;
        }
    }
}
