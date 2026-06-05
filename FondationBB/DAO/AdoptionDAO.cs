using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    // Création de contrats d'adoption et consultation de l'historique.
    //
    // Schéma réel (script FondationBB.sql) :
    //   adoption(id_adoption, id_personne, id_animal, id_employe, frais_adoption, date_adoption)
    //   animal.id_adoption : lien retour vers l'adoption (mis à jour ici).
    //   (pas de colonne "contrat signé" dans ce schéma)
    public class AdoptionDAO
    {
        // Enregistre une adoption, relie l'animal à l'adoption et le bascule au statut "Adopté".
        // Renvoie l'id de l'adoption créée.
        public int CreerAdoption(Adoption adoption)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                INSERT INTO adoption (id_personne, id_animal, id_employe, frais_adoption, date_adoption)
                VALUES (@idPersonne, @idAnimal, @idEmploye, @frais, @date)
                RETURNING id_adoption");
            cmd.Parameters.AddWithValue("@idPersonne", adoption.Personne?.IdPersonne ?? 0);
            cmd.Parameters.AddWithValue("@idAnimal", adoption.Animal?.IdAnimal ?? 0);
            cmd.Parameters.AddWithValue("@idEmploye", adoption.Employe?.IdEmploye ?? 0);
            cmd.Parameters.AddWithValue("@frais", adoption.FraisAdoption);
            cmd.Parameters.AddWithValue("@date", adoption.DateAdoption);
            int idAdoption = DataAcces.ExecuteInsert(cmd);

            // L'animal pointe vers son adoption et passe au statut "Adopté".
            NpgsqlCommand maj = new NpgsqlCommand(@"
                UPDATE animal
                SET id_adoption = @idAdoption,
                    id_statut   = (SELECT id_statut FROM statut WHERE libelle_statut = 'Adopté')
                WHERE id_animal = @idAnimal");
            maj.Parameters.AddWithValue("@idAdoption", idAdoption);
            maj.Parameters.AddWithValue("@idAnimal", adoption.Animal?.IdAnimal ?? 0);
            DataAcces.ExecuteSet(maj);

            return idAdoption;
        }

        // Historique des adoptions (le plus récent d'abord).
        public List<Adoption> GetHistorique()
        {
            List<Adoption> liste = new List<Adoption>();
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                SELECT ad.id_adoption, ad.date_adoption, ad.frais_adoption,
                       a.id_animal, a.nom_animal,
                       p.id_personne, p.nom_personne, p.prenom_personne,
                       p.telephone_personne, p.mail_personne
                FROM adoption ad
                JOIN animal a   ON a.id_animal = ad.id_animal
                JOIN personne p ON p.id_personne = ad.id_personne
                ORDER BY ad.date_adoption DESC");

            foreach (DataRow row in DataAcces.ExecuteSelect(cmd).Rows)
            {
                Animal animal = new Animal { IdAnimal = Convert.ToInt32(row["id_animal"]), NomAnimal = row["nom_animal"].ToString() ?? "" };
                Personne personne = new Personne(
                    Convert.ToInt32(row["id_personne"]),
                    row["nom_personne"].ToString() ?? "",
                    row["prenom_personne"].ToString() ?? "",
                    row["telephone_personne"]?.ToString()?.Trim() ?? "",
                    row["mail_personne"]?.ToString() ?? "");

                liste.Add(new Adoption
                {
                    IdAdoption = Convert.ToInt32(row["id_adoption"]),
                    DateAdoption = ((DateOnly)row["date_adoption"]).ToDateTime(TimeOnly.MinValue),
                    FraisAdoption = row["frais_adoption"] == DBNull.Value ? 0m : Convert.ToDecimal(row["frais_adoption"]),
                    Animal = animal,
                    Personne = personne
                });
            }
            return liste;
        }
    }
}
