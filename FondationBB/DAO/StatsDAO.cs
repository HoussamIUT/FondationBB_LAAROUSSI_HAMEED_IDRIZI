using System;
using Npgsql;

namespace FondationBB
{
    // Indicateurs du tableau de bord (réservé au responsable).
    public class StatsDAO
    {
        // Animaux encore présents au refuge (ni adoptés, ni décédés).
        public int CompterAnimauxPresents()
        {
            return LireEntier(new NpgsqlCommand(@"
                SELECT COUNT(*) FROM animal a
                JOIN statut s ON s.id_statut = a.id_statut
                WHERE s.libelle_statut NOT IN ('Adopté', 'Décédé')"));
        }

        public int CompterAnimauxDisponibles()
        {
            return LireEntier(new NpgsqlCommand(@"
                SELECT COUNT(*) FROM animal a
                JOIN statut s ON s.id_statut = a.id_statut
                WHERE s.libelle_statut = 'Disponible'"));
        }

        public int CompterAdoptions()
        {
            return LireEntier(new NpgsqlCommand("SELECT COUNT(*) FROM adoption"));
        }

        public int CompterDemandesEnAttente()
        {
            return LireEntier(new NpgsqlCommand("SELECT COUNT(*) FROM demande"));
        }

        // Adoptions enregistrées pendant le mois civil en cours.
        public int CompterAdoptionsCeMois()
        {
            return LireEntier(new NpgsqlCommand(@"
                SELECT COUNT(*) FROM adoption
                WHERE EXTRACT(MONTH FROM date_adoption) = EXTRACT(MONTH FROM CURRENT_DATE)
                  AND EXTRACT(YEAR  FROM date_adoption) = EXTRACT(YEAR  FROM CURRENT_DATE)"));
        }

        // Total des frais d'adoption récoltés pendant le mois civil en cours.
        public decimal FraisRecoltesCeMois()
        {
            return LireDecimal(new NpgsqlCommand(@"
                SELECT COALESCE(SUM(frais_adoption), 0) FROM adoption
                WHERE EXTRACT(MONTH FROM date_adoption) = EXTRACT(MONTH FROM CURRENT_DATE)
                  AND EXTRACT(YEAR  FROM date_adoption) = EXTRACT(YEAR  FROM CURRENT_DATE)"));
        }

        private int LireEntier(NpgsqlCommand cmd)
        {
            object? res = DataAcces.ExecuteScalar(cmd);
            return res == null || res == DBNull.Value ? 0 : Convert.ToInt32(res);
        }

        private decimal LireDecimal(NpgsqlCommand cmd)
        {
            object? res = DataAcces.ExecuteScalar(cmd);
            return res == null || res == DBNull.Value ? 0m : Convert.ToDecimal(res);
        }
    }
}
