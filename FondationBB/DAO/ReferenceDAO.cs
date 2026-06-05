using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace FondationBB
{
    // Accès aux tables de référence (listes déroulantes des formulaires et filtres).
    //
    // Schéma supposé (à vérifier avec le script SQL réel) :
    //   espece(id_espece, libelle_espece)
    //   race(id_race, libelle_race, taille_race, id_espece)
    //   statut(id_statut, libelle_statut) / etat(id_etat, libelle_etat)
    //   comportement(id_comportement, libelle_comportement)
    //   soin(id_soin, libelle_soin, tarif_soin, frequence_soin)
    public class ReferenceDAO
    {
        public List<Espece> GetEspeces()
        {
            List<Espece> liste = new List<Espece>();
            DataTable dt = DataAcces.ExecuteSelect(
                new NpgsqlCommand("SELECT id_espece, libelle_espece FROM espece ORDER BY libelle_espece"));
            foreach (DataRow row in dt.Rows)
                liste.Add(new Espece(Convert.ToInt32(row["id_espece"]), row["libelle_espece"].ToString() ?? ""));
            return liste;
        }

        public List<Race> GetRaces(int? idEspece = null)
        {
            List<Race> liste = new List<Race>();
            string sql = @"SELECT r.id_race, r.libelle_race, r.taille_race,
                                  e.id_espece, e.libelle_espece
                           FROM race r
                           JOIN espece e ON e.id_espece = r.id_espece";
            if (idEspece.HasValue) sql += " WHERE r.id_espece = @idEspece";
            sql += " ORDER BY r.libelle_race";

            NpgsqlCommand cmd = new NpgsqlCommand(sql);
            if (idEspece.HasValue) cmd.Parameters.AddWithValue("@idEspece", idEspece.Value);

            foreach (DataRow row in DataAcces.ExecuteSelect(cmd).Rows)
            {
                Espece espece = new Espece(Convert.ToInt32(row["id_espece"]), row["libelle_espece"].ToString() ?? "");
                liste.Add(new Race(
                    Convert.ToInt32(row["id_race"]),
                    row["libelle_race"].ToString() ?? "",
                    Conversions.ToTaille(row["taille_race"]),
                    espece));
            }
            return liste;
        }

        public List<Statut> GetStatuts()
        {
            List<Statut> liste = new List<Statut>();
            DataTable dt = DataAcces.ExecuteSelect(
                new NpgsqlCommand("SELECT id_statut, libelle_statut FROM statut ORDER BY id_statut"));
            foreach (DataRow row in dt.Rows)
                liste.Add(new Statut(Convert.ToInt32(row["id_statut"]), row["libelle_statut"].ToString() ?? ""));
            return liste;
        }

        public List<Etat> GetEtats()
        {
            List<Etat> liste = new List<Etat>();
            DataTable dt = DataAcces.ExecuteSelect(
                new NpgsqlCommand("SELECT id_etat, libelle_etat FROM etat ORDER BY id_etat"));
            foreach (DataRow row in dt.Rows)
                liste.Add(new Etat(Convert.ToInt32(row["id_etat"]), row["libelle_etat"].ToString() ?? ""));
            return liste;
        }

        public List<Comportement> GetComportements()
        {
            List<Comportement> liste = new List<Comportement>();
            DataTable dt = DataAcces.ExecuteSelect(
                new NpgsqlCommand("SELECT id_comportement, libelle_comportement FROM comportement ORDER BY libelle_comportement"));
            foreach (DataRow row in dt.Rows)
                liste.Add(new Comportement(Convert.ToInt32(row["id_comportement"]), row["libelle_comportement"].ToString() ?? ""));
            return liste;
        }
    }
}
