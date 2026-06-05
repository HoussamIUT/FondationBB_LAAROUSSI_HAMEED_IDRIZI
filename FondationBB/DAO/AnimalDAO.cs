using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    // Accès aux données des animaux : recherche multi-critères, fiche détaillée, création.
    //
    // Schéma réel (script FondationBB.sql) :
    //   animal(id_animal, id_statut, id_race, id_etat, id_adoption, id_employe,
    //          nom_animal, date_naissance_animal, i_cad_animal, sexe_animal,
    //          annotation_animal, date_arrivee_animal, poids_animal)
    //   sexe_animal CHAR(1) 'M'/'F' ; poids_animal > 0 ; i_cad_animal UNIQUE
    //   animal_comportement(id_comportement, id_animal)  -- liaison n-n
    //   recoit(id_soin, id_animal, date_soin, date_rappel)  -- soins reçus
    public class AnimalDAO
    {
        // SELECT commun (liste + fiche) : toutes les jointures de référence.
        private const string SELECT_ANIMAL = @"
            SELECT a.id_animal, a.nom_animal, a.date_naissance_animal, a.sexe_animal, a.poids_animal,
                   a.i_cad_animal, a.date_arrivee_animal, a.annotation_animal,
                   r.id_race, r.libelle_race, r.taille_race,
                   e.id_espece, e.libelle_espece,
                   s.id_statut, s.libelle_statut,
                   et.id_etat, et.libelle_etat
            FROM animal a
            LEFT JOIN race r   ON r.id_race   = a.id_race
            LEFT JOIN espece e ON e.id_espece = r.id_espece
            LEFT JOIN statut s ON s.id_statut = a.id_statut
            LEFT JOIN etat et  ON et.id_etat  = a.id_etat";

        // Recherche multi-critères (tous les paramètres sont facultatifs).
        public List<Animal> GetAnimaux(string? nom = null, int? idEspece = null,
                                       int? idRace = null, string? libelleStatut = null)
        {
            List<Animal> animaux = new List<Animal>();

            StringBuilder sql = new StringBuilder(SELECT_ANIMAL);
            sql.Append(" WHERE 1=1");
            if (!string.IsNullOrWhiteSpace(nom)) sql.Append(" AND lower(a.nom_animal) LIKE @nom");
            if (idEspece.HasValue) sql.Append(" AND e.id_espece = @idEspece");
            if (idRace.HasValue) sql.Append(" AND a.id_race = @idRace");
            if (!string.IsNullOrWhiteSpace(libelleStatut)) sql.Append(" AND s.libelle_statut = @statut");
            sql.Append(" ORDER BY a.nom_animal");

            NpgsqlCommand cmd = new NpgsqlCommand(sql.ToString());
            if (!string.IsNullOrWhiteSpace(nom)) cmd.Parameters.AddWithValue("@nom", "%" + nom.Trim().ToLower() + "%");
            if (idEspece.HasValue) cmd.Parameters.AddWithValue("@idEspece", idEspece.Value);
            if (idRace.HasValue) cmd.Parameters.AddWithValue("@idRace", idRace.Value);
            if (!string.IsNullOrWhiteSpace(libelleStatut)) cmd.Parameters.AddWithValue("@statut", libelleStatut);

            foreach (DataRow row in DataAcces.ExecuteSelect(cmd).Rows)
                animaux.Add(ConstruireAnimal(row));
            return animaux;
        }

        // Fiche complète d'un animal (avec comportements et soins reçus).
        public Animal? GetAnimal(int idAnimal)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(SELECT_ANIMAL + " WHERE a.id_animal = @id");
            cmd.Parameters.AddWithValue("@id", idAnimal);

            DataTable dt = DataAcces.ExecuteSelect(cmd);
            if (dt.Rows.Count == 0) return null;

            Animal animal = ConstruireAnimal(dt.Rows[0]);
            animal.Comportements = GetComportementsAnimal(idAnimal);
            animal.SoinsRecus = GetSoinsRecusAnimal(idAnimal);
            return animal;
        }

        // Enregistre un nouvel animal (réservé au responsable) et renvoie l'id généré.
        public int CreerAnimal(Animal animal)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                INSERT INTO animal (nom_animal, date_naissance_animal, sexe_animal, poids_animal,
                                    i_cad_animal, date_arrivee_animal, annotation_animal,
                                    id_race, id_statut, id_etat, id_employe)
                VALUES (@nom, @naiss, @sexe, @poids, @icad, @arrivee, @annot,
                        @idRace, @idStatut, @idEtat, @idEmploye)
                RETURNING id_animal");

            cmd.Parameters.AddWithValue("@nom", animal.NomAnimal);
            cmd.Parameters.AddWithValue("@naiss", animal.DateNaissanceAnimal);
            cmd.Parameters.AddWithValue("@sexe", Conversions.FromSexe(animal.Sexe));
            cmd.Parameters.AddWithValue("@poids", animal.PoidsAnimal);
            // i_cad_animal est UNIQUE : on insère NULL (et non "") quand il est vide.
            cmd.Parameters.AddWithValue("@icad",
                string.IsNullOrWhiteSpace(animal.IcadAnimal) ? (object)DBNull.Value : animal.IcadAnimal.Trim());
            cmd.Parameters.AddWithValue("@arrivee", animal.DateArriveeAnimal == default ? DateTime.Today : animal.DateArriveeAnimal);
            cmd.Parameters.AddWithValue("@annot", (object?)animal.AnnotationAnimal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idRace", (object?)animal.Race?.IdRace ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idStatut", (object?)animal.Statut?.IdStatut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idEtat", (object?)animal.Etat?.IdEtat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idEmploye", (object?)animal.Employe?.IdEmploye ?? DBNull.Value);

            int idAnimal = DataAcces.ExecuteInsert(cmd);

            // Liaisons n-n vers les comportements cochés.
            foreach (Comportement c in animal.Comportements)
            {
                NpgsqlCommand link = new NpgsqlCommand(
                    "INSERT INTO animal_comportement (id_comportement, id_animal) VALUES (@c, @a)");
                link.Parameters.AddWithValue("@c", c.IdComportement);
                link.Parameters.AddWithValue("@a", idAnimal);
                DataAcces.ExecuteSet(link);
            }
            return idAnimal;
        }

        // Met à jour les informations modifiables d'un animal.
        public int ModifierAnimal(Animal animal)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                UPDATE animal
                SET nom_animal = @nom, poids_animal = @poids, annotation_animal = @annot,
                    id_statut = @idStatut, id_etat = @idEtat
                WHERE id_animal = @id");
            cmd.Parameters.AddWithValue("@nom", animal.NomAnimal);
            cmd.Parameters.AddWithValue("@poids", animal.PoidsAnimal);
            cmd.Parameters.AddWithValue("@annot", (object?)animal.AnnotationAnimal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idStatut", (object?)animal.Statut?.IdStatut ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idEtat", (object?)animal.Etat?.IdEtat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", animal.IdAnimal);
            return DataAcces.ExecuteSet(cmd);
        }

        // --- Construction d'un Animal à partir d'une ligne du SELECT_ANIMAL ---
        private Animal ConstruireAnimal(DataRow row)
        {
            Animal a = new Animal
            {
                IdAnimal = Convert.ToInt32(row["id_animal"]),
                NomAnimal = row["nom_animal"].ToString() ?? "",
                Sexe = Conversions.ToSexe(row["sexe_animal"]),
                PoidsAnimal = row["poids_animal"] == DBNull.Value ? 0 : Convert.ToDouble(row["poids_animal"]),
                // i_cad_animal est un CHAR(15) : on enlève le remplissage d'espaces.
                IcadAnimal = row["i_cad_animal"]?.ToString()?.Trim() ?? "",
                AnnotationAnimal = row["annotation_animal"]?.ToString() ?? ""
            };
            // La date de naissance peut être nulle en base.
            if (row["date_naissance_animal"] != DBNull.Value)
                a.DateNaissanceAnimal = ((DateOnly)row["date_naissance_animal"]).ToDateTime(TimeOnly.MinValue);
            if (row["date_arrivee_animal"] != DBNull.Value)
                a.DateArriveeAnimal = ((DateOnly)row["date_arrivee_animal"]).ToDateTime(TimeOnly.MinValue);

            if (row["id_race"] != DBNull.Value)
            {
                Espece? espece = row["id_espece"] != DBNull.Value
                    ? new Espece(Convert.ToInt32(row["id_espece"]), row["libelle_espece"].ToString() ?? "")
                    : null;
                a.Race = new Race(
                    Convert.ToInt32(row["id_race"]),
                    row["libelle_race"].ToString() ?? "",
                    Conversions.ToTaille(row["taille_race"]),
                    espece);
            }
            if (row["id_statut"] != DBNull.Value)
                a.Statut = new Statut(Convert.ToInt32(row["id_statut"]), row["libelle_statut"].ToString() ?? "");
            if (row["id_etat"] != DBNull.Value)
                a.Etat = new Etat(Convert.ToInt32(row["id_etat"]), row["libelle_etat"].ToString() ?? "");

            return a;
        }

        private List<Comportement> GetComportementsAnimal(int idAnimal)
        {
            List<Comportement> liste = new List<Comportement>();
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                SELECT c.id_comportement, c.libelle_comportement
                FROM animal_comportement ac
                JOIN comportement c ON c.id_comportement = ac.id_comportement
                WHERE ac.id_animal = @id");
            cmd.Parameters.AddWithValue("@id", idAnimal);
            foreach (DataRow row in DataAcces.ExecuteSelect(cmd).Rows)
                liste.Add(new Comportement(Convert.ToInt32(row["id_comportement"]), row["libelle_comportement"].ToString() ?? ""));
            return liste;
        }

        private List<SoinRecu> GetSoinsRecusAnimal(int idAnimal)
        {
            List<SoinRecu> liste = new List<SoinRecu>();
            NpgsqlCommand cmd = new NpgsqlCommand(@"
                SELECT s.id_soin, s.libelle_soin, s.tarif_soin, sr.date_soin, sr.date_rappel
                FROM recoit sr
                JOIN soin s ON s.id_soin = sr.id_soin
                WHERE sr.id_animal = @id
                ORDER BY sr.date_soin DESC");
            cmd.Parameters.AddWithValue("@id", idAnimal);
            foreach (DataRow row in DataAcces.ExecuteSelect(cmd).Rows)
            {
                Soin soin = new Soin(
                    Convert.ToInt32(row["id_soin"]),
                    row["libelle_soin"].ToString() ?? "",
                    row["tarif_soin"] == DBNull.Value ? 0m : Convert.ToDecimal(row["tarif_soin"]));
                liste.Add(new SoinRecu(
                    soin,
                    ((DateOnly)row["date_soin"]).ToDateTime(TimeOnly.MinValue),
                    row["date_rappel"] == DBNull.Value ? (DateTime?)null : ((DateOnly)row["date_rappel"]).ToDateTime(TimeOnly.MinValue)));
            }
            return liste;
        }
    }
}
