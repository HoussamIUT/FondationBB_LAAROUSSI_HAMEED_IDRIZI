using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace FondationBB
{
    public class DataAcces
    {
        private static string connectionString;
        private static NpgsqlConnection connection;

        static DataAcces()
        {
        }


        public static void InitializeConnection(string username, string password)
        {
            connectionString = $"Host=srv-peda-new;Port=5433;Username={username};Password={password};Database=laaroush_fondationBB;Options='-c search_path=laaroush'";

            connection = new NpgsqlConnection(connectionString);
        }

        // pour récupérer la connexion (et l'ouvrir si nécessaire)
        public static NpgsqlConnection GetConnection()
        {

            if (connection == null)
            {
                throw new InvalidOperationException("La connexion n'a pas été initialisée avec un login et un mot de passe.");
            }

            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)

                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    LogError.Log(ex, "Pb à la connexion  \n");
                    throw;
                }

            return connection;
        }


        public static DataTable ExecuteSelect(NpgsqlCommand cmd)
        {
            DataTable dataTable = new DataTable();
            try
            {
                cmd.Connection = GetConnection();
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "Pb de executeSelect \n" + cmd.CommandText);
                throw;
            }

            return dataTable;
        }

        //   pour requêtes INSERT et renvoie l'ID généré

        public static int ExecuteInsert(NpgsqlCommand cmd)
        {
            int nb = 0;
            try
            {
                cmd.Connection = GetConnection();
                nb = (int)cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                LogError.Log(ex, "Pb de executeInsert \n" + cmd.CommandText);
                throw;
            }
            return nb;

        }




        //  pour requêtes UPDATE, DELETE
        public static int ExecuteSet(NpgsqlCommand cmd)
        {
            int nb = 0;
            try
            {
                cmd.Connection = GetConnection();
                nb = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "Pb de executeSet \n" + cmd.CommandText);
                throw;
            }
            return nb;

        }

        // pour requêtes avec une seule valeur retour  (ex : 1 colonne, ou COUNT, SUM) 
        public static string ExecuteSelectOneValue(NpgsqlCommand cmd)
        {
            object res = null;
            try
            {
                cmd.Connection = GetConnection();
                res = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LogError.Log(ex, "Pb de ExecuteSelectOneValue \n" + cmd.CommandText);
                throw;
            }
            return res.ToString();

        }

        //  Fermer la connexion 
        public static void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
