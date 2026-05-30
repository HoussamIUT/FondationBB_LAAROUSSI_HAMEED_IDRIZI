using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace FondationBB
{
    /// <summary>
    /// Logique d'interaction pour UCLogin.xaml
    /// </summary>
    public partial class UCLogin : UserControl
    {
        public UCLogin()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        public class DataAccess
        {
            private static string connectionString;
            private static NpgsqlConnection connection;

            // =========================================================================
            // NOUVELLE MÉTHODE : On l'appelle quand on clique sur "Se connecter"
            // =========================================================================
            public static bool ConnectUser(string username, string password)
            {
                // On fabrique la chaîne de connexion avec les variables tapées dans l'interface
                // (⚠️ Pense à adapter le Host, Port et Database si ce n'est plus laaroush_pension)
                connectionString = $"Host=srv-peda-new;Port=5433;Database=laaroush_fondationBB;Username={username};Password={password};Options='-c search_path=laaroush'";

                try
                {
                    // Si une ancienne connexion était ouverte, on la ferme par sécurité
                    if (connection != null && connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    connection = new NpgsqlConnection(connectionString);

                    // On tente d'ouvrir. 
                    // Si le mot de passe est faux, PostgreSQL va rejeter et déclencher le "catch" !
                    connection.Open();

                    return true; // Si on arrive ici, la connexion a réussi
                }
                catch (Exception ex)
                {
                    // Le mot de passe est faux, ou le serveur est éteint
                    // LogError.Log(ex, "Pb à la connexion"); // (Décommente si tu as la classe LogError)
                    return false;
                }
            }

            // =========================================================================
            // LE RESTE DU CODE DU PROFESSEUR (Inchangé)
            // =========================================================================

            public static NpgsqlConnection GetConnection()
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        // LogError.Log(ex, "Pb à la connexion \n");
                        throw;
                    }
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
                    // LogError.Log(ex, "Pb de executeSelect \n" + cmd.CommandText);
                    throw;
                }
                return dataTable;
            }

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
                    // LogError.Log(ex, "Pb de executeInsert \n" + cmd.CommandText);
                    throw;
                }
                return nb;
            }

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
                    // LogError.Log(ex, "Pb de executeSet \n" + cmd.CommandText);
                    throw;
                }
                return nb;
            }

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
                    // LogError.Log(ex, "Pb de ExecuteSelectOneValue \n" + cmd.CommandText);
                    throw;
                }
                return res?.ToString();
            }

            public static void CloseConnection()
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
