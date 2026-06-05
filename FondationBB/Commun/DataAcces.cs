using System;
using System.Data;
using Npgsql;

namespace FondationBB
{
    // Couche d'accès technique à PostgreSQL (Npgsql).
    // Authentification par GRANT : on ouvre la connexion avec le login/mot de passe saisis ;
    // si la connexion s'ouvre, l'utilisateur est authentifié au niveau du SGBD.
    //
    // ⚠️ À adapter au serveur réel de l'équipe (host / base / schéma) :
    //    Host=srv-peda-new ; Port=5433 ; Database=laaroush_fondationBB ; search_path=laaroush
    public static class DataAcces
    {
        private static string? connectionString;
        private static NpgsqlConnection? connection;

        // Construit la chaîne de connexion à partir des identifiants saisis au login.
        public static void InitializeConnection(string username, string password)
        {
            connectionString =
                $"Host=srv-peda-new; Port=5433;Username={username};Password={password};" +
                "Database=laaroush_fondationBB;Options='-c search_path=laaroush'";
            connection = new NpgsqlConnection(connectionString);
        }

        // Récupère la connexion en l'ouvrant si nécessaire.
        public static NpgsqlConnection GetConnection()
        {
            if (connection == null)
                throw new InvalidOperationException(
                    "La connexion n'a pas été initialisée (appeler InitializeConnection au login).");

            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
            {
                try { connection.Open(); }
                catch (Exception ex) { LogError.Log(ex, "Problème à l'ouverture de la connexion"); throw; }
            }
            return connection;
        }

        // SELECT renvoyant plusieurs lignes/colonnes.
        public static DataTable ExecuteSelect(NpgsqlCommand cmd)
        {
            DataTable table = new DataTable();
            try
            {
                cmd.Connection = GetConnection();
                using (var adapter = new NpgsqlDataAdapter(cmd))
                    adapter.Fill(table);
            }
            catch (Exception ex) { LogError.Log(ex, "ExecuteSelect\n" + cmd.CommandText); throw; }
            return table;
        }

        // INSERT ... RETURNING id : renvoie l'identifiant généré.
        public static int ExecuteInsert(NpgsqlCommand cmd)
        {
            try
            {
                cmd.Connection = GetConnection();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex) { LogError.Log(ex, "ExecuteInsert\n" + cmd.CommandText); throw; }
        }

        // UPDATE / DELETE / INSERT sans retour : renvoie le nombre de lignes affectées.
        public static int ExecuteSet(NpgsqlCommand cmd)
        {
            try
            {
                cmd.Connection = GetConnection();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { LogError.Log(ex, "ExecuteSet\n" + cmd.CommandText); throw; }
        }

        // Requête renvoyant une seule valeur (COUNT, SUM, MAX...).
        public static object? ExecuteScalar(NpgsqlCommand cmd)
        {
            try
            {
                cmd.Connection = GetConnection();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex) { LogError.Log(ex, "ExecuteScalar\n" + cmd.CommandText); throw; }
        }

        public static void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }
}
