using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace FondationBB
{
    // Accès aux demandes d'adoption en attente.
    //
    // Schéma supposé : demande(id_demande, date_demande, tranche_age_demande, id_personne, id_race)
    //   tranche_age_demande in {'Junior','Adulte','Senior'} ; id_race facultatif (0,1)
    public class DemandeDAO
    {
        // Toutes les demandes, de la plus ancienne à la plus récente.
        public List<Demande> GetToutesDemandes()
        {
            List<Demande> demandes = new List<Demande>();
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                SELECT d.id_demande, d.date_demande, d.tranche_age_demande,
                       p.id_personne, p.nom_personne, p.prenom_personne,
                       p.telephone_personne, p.mail_personne,
                       r.id_race, r.libelle_race, r.taille_race,
                       e.id_espece, e.libelle_espece
                FROM demande d
                JOIN personne p ON p.id_personne = d.id_personne
                LEFT JOIN race r   ON r.id_race   = d.id_race
                LEFT JOIN espece e ON e.id_espece = r.id_espece
                ORDER BY d.date_demande ASC");

            foreach (DataRow row in DataAcces.ExecuteSelect(cmd).Rows)
            {
                Personne personne = new Personne(
                    Convert.ToInt32(row["id_personne"]),
                    row["nom_personne"].ToString() ?? "",
                    row["prenom_personne"].ToString() ?? "",
                    row["telephone_personne"]?.ToString() ?? "",
                    row["mail_personne"]?.ToString() ?? "");

                Race? race = null;
                if (row["id_race"] != DBNull.Value)
                {
                    Espece? espece = row["id_espece"] != DBNull.Value
                        ? new Espece(Convert.ToInt32(row["id_espece"]), row["libelle_espece"].ToString() ?? "")
                        : null;
                    race = new Race(
                        Convert.ToInt32(row["id_race"]),
                        row["libelle_race"].ToString() ?? "",
                        Conversions.ToTaille(row["taille_race"]),
                        espece);
                }

                demandes.Add(new Demande(
                    Convert.ToInt32(row["id_demande"]),
                    ((DateOnly)row["date_demande"]).ToDateTime(TimeOnly.MinValue),
                    Conversions.ToTranche(row["tranche_age_demande"]),
                    personne,
                    race));
            }
            return demandes;
        }

        // Crée une demande et renvoie l'id généré. idRace null = aucune race imposée.
        public int CreerDemande(int idPersonne, TrancheAge tranche, int? idRace)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                INSERT INTO demande (date_demande, tranche_age_demande, id_personne, id_race)
                VALUES (CURRENT_DATE, @tranche, @idPersonne, @idRace)
                RETURNING id_demande");
            cmd.Parameters.AddWithValue("@tranche", tranche.ToString());
            cmd.Parameters.AddWithValue("@idPersonne", idPersonne);
            cmd.Parameters.AddWithValue("@idRace", (object?)idRace ?? DBNull.Value);
            return DataAcces.ExecuteInsert(cmd);
        }
    }
}
