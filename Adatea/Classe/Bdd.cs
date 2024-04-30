using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Adatea.Classe
{
    public class BDD
    {
        private string connectionString = "Server=localhost;Database=Adatea;User ID=root;Password=root;CharSet=utf8;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        #region Admin
        public bool AuthentifierAdmin(string username, string password)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM Admin WHERE Username = @Username AND Password = @Password";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // Ici, vous devez hasher le mot de passe si vous stockez des hash dans la base de données

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        #endregion
        #region Client
        public List<Client> GetClients()
        {
            List<Client> clients = new List<Client>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Client WHERE Type = 1", conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Client client = new Client(
                    Convert.ToInt32(reader["ID_Client"]),
                    Convert.ToString(reader["Lastname"]),
                    Convert.ToString(reader["Firstname"]),
                    Convert.ToString(reader["Email"]),
                    Convert.ToString(reader["Street"]),
                    Convert.ToString(reader["PostalCode"]),
                    Convert.ToString(reader["City"]),
                    Convert.ToString(reader["Country"]),
                    Convert.ToInt32(reader["Phone_Number"]),
                    Convert.ToInt32(reader["ID_Prospect"])
                    );
                    clients.Add(client);
                }
            }

            return clients;
        }
        public void AjouterClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Client (ID_Client, Lastname, Firstname, Email, Street, PostalCode, City, Country, Phone_Number, ID_Prospect) VALUES (@id, @last, @first, @email, @street, @postalCode, @city, @country, @phone, @prospect)"; using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", client.ID_Client);
                    cmd.Parameters.AddWithValue("@last", client.Lastname);
                    cmd.Parameters.AddWithValue("@first", client.Firstname);
                    cmd.Parameters.AddWithValue("@email", client.Email);
                    cmd.Parameters.AddWithValue("@street", client.Street);
                    cmd.Parameters.AddWithValue("@postalCode", client.PostalCode);
                    cmd.Parameters.AddWithValue("@city", client.City);
                    cmd.Parameters.AddWithValue("@country", client.Country);
                    cmd.Parameters.AddWithValue("@phone", client.Phone_Number);
                    cmd.Parameters.AddWithValue("@prospect", client.ID_Prospect);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // Méthode pour mettre à jour l'état d'un prospect lors de sa conversion en client
        public void ConvertirProspectEnClient(int idProspect)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "UPDATE Client SET type = 1 WHERE ID_Prospect = @idProspect";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idProspect", idProspect);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public void MettreAJourClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "UPDATE Client SET Lastname=@last, Firstname=@first, Email=@email, Street=@street, PostalCode=@postalCode, City=@city, Country=@country, Phone_Number=@phone, ID_Prospect=@prospect WHERE ID_Client=@id"; using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", client.ID_Client);
                    cmd.Parameters.AddWithValue("@last", client.Lastname);
                    cmd.Parameters.AddWithValue("@first", client.Firstname);
                    cmd.Parameters.AddWithValue("@email", client.Email);
                    cmd.Parameters.AddWithValue("@street", client.Street);
                    cmd.Parameters.AddWithValue("@postalCode", client.PostalCode);
                    cmd.Parameters.AddWithValue("@city", client.City);
                    cmd.Parameters.AddWithValue("@country", client.Country);
                    cmd.Parameters.AddWithValue("@phone", client.Phone_Number);
                    cmd.Parameters.AddWithValue("@prospect", client.ID_Prospect);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SupprimerClient(int idClient)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Client WHERE ID_Client=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idClient);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public int GetNextClientID()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT MAX(ID_Client) FROM Client";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result) + 1;
                    }
                    else
                    {
                        return 1; // ou une autre valeur de départ si votre ID ne commence pas à 1
                    }
                }
            }
        }

        public Client GetClientByID(int idClient)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Client WHERE ID_Client = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idClient);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Client(
                                reader.GetInt32(reader.GetOrdinal("ID_Client")),
                                reader.GetString(reader.GetOrdinal("Lastname")),
                                reader.GetString(reader.GetOrdinal("Firstname")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("Street")),
                                reader.GetString(reader.GetOrdinal("PostalCode")),
                                reader.GetString(reader.GetOrdinal("City")),
                                reader.GetString(reader.GetOrdinal("Country")),
                                reader.GetInt32(reader.GetOrdinal("Phone_Number")),
                                reader.GetInt32(reader.GetOrdinal("ID_Prospect"))
                            );
                        }
                    }
                }
            }
            return null; // ou gestion d'erreur si l'ID n'est pas trouvé
        }

        public bool VerifierSiClientExiste(int idClient)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM Client WHERE ID_Client = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idClient);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        #endregion

        #region Prospects
        public List<Prospect> GetProspects()
        {
            List<Prospect> prospects = new List<Prospect>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM client WHERE type = 0 ", conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Prospect prospect = new Prospect(
                                    Convert.ToInt32(reader["ID_Prospect"]),
                                    Convert.ToString(reader["Lastname"]),
                                    Convert.ToString(reader["Firstname"]),
                                    Convert.ToString(reader["Email"]),
                                    Convert.ToString(reader["Street"]),
                                    Convert.ToString(reader["PostalCode"]),
                                    Convert.ToString(reader["City"]),
                                    Convert.ToString(reader["Country"]),
                                    Convert.ToInt32(reader["Phone_Number"])
                    );
                    prospects.Add(prospect);
                }
            }

            return prospects;
        }

        public int GetNextProspectID()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT MAX(ID_Prospect) FROM Client";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result) + 1;
                    }
                    else
                    {
                        return 1; // ou une autre valeur de départ si votre ID ne commence pas à 1
                    }
                }
            }
        }

        public List<int> GetProspectIDs()
        {
            List<int> ids = new List<int>();
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT ID_Prospect FROM Prospect";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return ids;
        }

        public Prospect GetProspectByID(int idProspect)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Client WHERE ID_Prospect = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idProspect);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Prospect(
                                 reader.GetInt32(reader.GetOrdinal("ID_Prospect")),
                                    reader.GetString(reader.GetOrdinal("Lastname")),
                                    reader.GetString(reader.GetOrdinal("Firstname")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("Street")),
                                    reader.GetString(reader.GetOrdinal("PostalCode")),
                                    reader.GetString(reader.GetOrdinal("City")),
                                    reader.GetString(reader.GetOrdinal("Country")),
                                    reader.GetInt32(reader.GetOrdinal("Phone_Number"))
                            );
                        }
                    }
                }
            }
            return null; // ou gestion d'erreur si l'ID n'est pas trouvé
        }





        public void AjouterProspect(Prospect prospect)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Client (ID_Prospect, Lastname, Firstname, Email, Street, PostalCode, City, Country, Phone_Number, Type) VALUES (@id, @last, @first, @email, @street, @postalCode, @city, @country, @phone, @type)"; using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", prospect.ID_Prospect);
                    cmd.Parameters.AddWithValue("@last", prospect.Lastname);
                    cmd.Parameters.AddWithValue("@first", prospect.Firstname);
                    cmd.Parameters.AddWithValue("@email", prospect.Email);
                    cmd.Parameters.AddWithValue("@street", prospect.Street);
                    cmd.Parameters.AddWithValue("@postalCode", prospect.PostalCode);
                    cmd.Parameters.AddWithValue("@city", prospect.City);
                    cmd.Parameters.AddWithValue("@country", prospect.Country);
                    cmd.Parameters.AddWithValue("@phone", prospect.Phone_Number);
                    cmd.Parameters.AddWithValue("@type", 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void MettreAJourProspect(Prospect prospect)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE Client SET Lastname=@last, Firstname=@first, Email=@email, Street=@street, PostalCode=@postalCode, City=@city, Country=@country, Phone_Number=@phone WHERE ID_Prospect=@id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", prospect.ID_Prospect);
                    cmd.Parameters.AddWithValue("@last", prospect.Lastname);
                    cmd.Parameters.AddWithValue("@first", prospect.Firstname);
                    cmd.Parameters.AddWithValue("@email", prospect.Email);
                    cmd.Parameters.AddWithValue("@street", prospect.Street);
                    cmd.Parameters.AddWithValue("@postalCode", prospect.PostalCode);
                    cmd.Parameters.AddWithValue("@city", prospect.City);
                    cmd.Parameters.AddWithValue("@country", prospect.Country);
                    cmd.Parameters.AddWithValue("@phone", prospect.Phone_Number);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SupprimerProspect(int idProspect)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Client WHERE ID_Prospect=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idProspect);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void MettreAJourTypeProspect(int idProspect, int nouveauType)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "UPDATE Client SET type = @nouveauType WHERE ID_Prospect = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nouveauType", nouveauType);
                    cmd.Parameters.AddWithValue("@id", idProspect);
                    cmd.ExecuteNonQuery();
                }
            }
        }




        public bool VerifierSiProspectExiste(int idProspect)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM Prospect WHERE ID_Prospect = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idProspect);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        #endregion

        #region Produits
        // Pour récupérer la liste des produits
        public List<Produit> GetProduits()
        {
            List<Produit> listeProduits = new List<Produit>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Product", conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Produit produit = new Produit(
                        Convert.ToInt32(reader["ID_Product"]),
                        Convert.ToString(reader["Product_Name"]),
                        Convert.ToString(reader["Description"]),
                        Convert.ToDecimal(reader["Price"]),
                        Convert.ToInt32(reader["Stock_Quantity"])
                    );
                    listeProduits.Add(produit);
                }
            }

            return listeProduits;
        }
        public Produit GetProduitByID(int idProduit)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Product WHERE ID_Product = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idProduit);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Produit(
                                reader.GetInt32(reader.GetOrdinal("ID_Product")),
                                reader.GetString(reader.GetOrdinal("Product_Name")),
                                reader.GetString(reader.GetOrdinal("Description")),
                                reader.GetDecimal(reader.GetOrdinal("Price")),
                                reader.GetInt32(reader.GetOrdinal("Stock_Quantity"))
                            );
                        }
                    }
                }
            }
            return null; // ou gestion d'erreur si l'ID n'est pas trouvé
        }


        public int GetNextProductID()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT IFNULL(MAX(ID_Product), 0) + 1 FROM Product";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }




        public void AjouterProduit(Produit produit)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Product (ID_Product, Product_Name, Description, Price, Stock_Quantity) VALUES (@id, @name, @desc, @price, @stock)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", produit.ID_Product);
                    cmd.Parameters.AddWithValue("@name", produit.Product_Name);
                    cmd.Parameters.AddWithValue("@desc", produit.Description);
                    cmd.Parameters.AddWithValue("@price", produit.Price);
                    cmd.Parameters.AddWithValue("@stock", produit.Stock_Quantity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void MettreAJourProduit(Produit produit)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "UPDATE Product SET Product_Name=@name, Description=@desc, Price=@price, Stock_Quantity=@stock WHERE ID_Product=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", produit.ID_Product);
                    cmd.Parameters.AddWithValue("@name", produit.Product_Name);
                    cmd.Parameters.AddWithValue("@desc", produit.Description);
                    cmd.Parameters.AddWithValue("@price", produit.Price);
                    cmd.Parameters.AddWithValue("@stock", produit.Stock_Quantity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SupprimerProduit(int idProduit)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Product WHERE ID_Product=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idProduit);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region RDV

        public List<Rendezvous> GetRendezvous()
        {
            List<Rendezvous> lesRendezvous = new List<Rendezvous>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Appointment", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Rendezvous rendezvous = new Rendezvous(
                            Convert.ToInt32(reader["ID_Rdv"]),
                            reader["ID_Prospect"] as int?,
                            reader["ID_Client"] as int?,
                            Convert.ToInt32(reader["ID_Commercial"]),
                            Convert.ToDateTime(reader["Date_Rdv"]),
                            TimeSpan.Parse(reader["Time_Rdv"].ToString()),
                            reader["Location"].ToString()
                        );
                        lesRendezvous.Add(rendezvous);
                    }
                }
            }
            return lesRendezvous;
        }

        public void AjouterRendezvous(Rendezvous rendezvous)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Appointment (ID_Rdv, ID_Prospect, ID_Client, ID_Commercial, Date_Rdv, Time_Rdv, Location) VALUES (@idRdv, @idProspect, @idClient, @idCommercial, @dateRdv, @timeRdv, @location)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idRdv", rendezvous.ID_Rdv);
                    cmd.Parameters.AddWithValue("@idProspect", rendezvous.ID_Prospect ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@idClient", rendezvous.ID_Client ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@idCommercial", rendezvous.ID_Commercial);
                    cmd.Parameters.AddWithValue("@dateRdv", rendezvous.Date_Rdv);
                    cmd.Parameters.AddWithValue("@timeRdv", rendezvous.Time_Rdv.ToString());
                    cmd.Parameters.AddWithValue("@location", rendezvous.Location);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void MettreAJourRendezvous(Rendezvous rendezvous)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "UPDATE Appointment SET ID_Prospect=@idProspect, ID_Client=@idClient, ID_Commercial=@idCommercial, Date_Rdv=@dateRdv, Time_Rdv=@timeRdv, Location=@location WHERE ID_Rdv=@idRdv";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idRdv", rendezvous.ID_Rdv);
                    cmd.Parameters.AddWithValue("@idProspect", rendezvous.ID_Prospect ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@idClient", rendezvous.ID_Client ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@idCommercial", rendezvous.ID_Commercial);
                    cmd.Parameters.AddWithValue("@dateRdv", rendezvous.Date_Rdv);
                    cmd.Parameters.AddWithValue("@timeRdv", rendezvous.Time_Rdv.ToString());
                    cmd.Parameters.AddWithValue("@location", rendezvous.Location);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public void SupprimerRendezvous(int idRendezvous)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Appointment WHERE ID_Rdv=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idRendezvous);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public int GetNextRendezvousID()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT IFNULL(MAX(ID_Rdv), 0) + 1 FROM Appointment";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        #endregion


        #region facturation
        // Pour récupérer la liste des factures
        public List<Facturation> GetFacturations()
        {
            List<Facturation> listeFacturations = new List<Facturation>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Invoice", conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Facturation facturation = new Facturation(
                        Convert.ToInt32(reader["ID_Invoice"]),
                        Convert.ToInt32(reader["ID_Client"]),
                        Convert.ToDateTime(reader["Date_Invoice"]),
                        Convert.ToDecimal(reader["Total"]),
                        Convert.ToString(reader["Payment_Status"])
                    );
                    listeFacturations.Add(facturation);
                }
            }

            return listeFacturations;
        }
        public int GetNextInvoiceID()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT IFNULL(MAX(ID_Invoice), 0) + 1 FROM Invoice";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }



        public void AjouterFacturation(Facturation facturation)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Invoice (ID_Invoice, ID_Client, Date_Invoice, Total, Payment_Status) VALUES (@id, @idClient, @date, @total, @status)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", facturation.ID_Invoice);
                    cmd.Parameters.AddWithValue("@idClient", facturation.ID_Client);
                    cmd.Parameters.AddWithValue("@date", facturation.Date_Invoice);
                    cmd.Parameters.AddWithValue("@total", facturation.Total);
                    cmd.Parameters.AddWithValue("@status", facturation.Payment_Status);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void MettreAJourFacturation(Facturation facturation)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "UPDATE Invoice SET ID_Client=@idClient, Date_Invoice=@date, Total=@total, Payment_Status=@status WHERE ID_Invoice=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", facturation.ID_Invoice);
                    cmd.Parameters.AddWithValue("@idClient", facturation.ID_Client);
                    cmd.Parameters.AddWithValue("@date", facturation.Date_Invoice);
                    cmd.Parameters.AddWithValue("@total", facturation.Total);
                    cmd.Parameters.AddWithValue("@status", facturation.Payment_Status);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SupprimerFacturation(int idFacturation)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Invoice WHERE ID_Invoice=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idFacturation);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region ligne facturation

        public int GetNextInvoiceLineID()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT IFNULL(MAX(ID_Invoice_Line), 0) + 1 FROM invoice_line";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public List<FacturationLigne> GetLignesFacturation()
        {
            List<FacturationLigne> listeLignesFacturation = new List<FacturationLigne>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Invoice_Line", conn); 
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    FacturationLigne ligneFacturation = new FacturationLigne(
                        Convert.ToInt32(reader["ID_Invoice_Line"]),
                        Convert.ToInt32(reader["ID_Invoice"]),
                        Convert.ToInt32(reader["ID_Product"]),
                        Convert.ToInt32(reader["Quantity"]),
                        Convert.ToDecimal(reader["Subtotal"])
                    );
                    listeLignesFacturation.Add(ligneFacturation);
                }
            }

            return listeLignesFacturation;
        }



        public List<FacturationLigne> GetLignesFacturationParFacture(int idInvoice)
        {
            List<FacturationLigne> lignesFacturation = new List<FacturationLigne>();

            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Invoice_Line WHERE ID_Invoice = @idInvoice";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idInvoice", idInvoice);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idInvoiceLine = Convert.ToInt32(reader["ID_Invoice_Line"]);
                            int idProduct = Convert.ToInt32(reader["ID_Product"]);
                            int quantity = Convert.ToInt32(reader["Quantity"]);
                            decimal subtotal = Convert.ToDecimal(reader["Subtotal"]);

                            // Utiliser le constructeur pour créer l'objet
                            FacturationLigne ligne = new FacturationLigne(idInvoiceLine, idInvoice, idProduct, quantity, subtotal);
                            lignesFacturation.Add(ligne);
                        }
                    }
                }
            }

            return lignesFacturation;
        }

        public void AjouterLigneFacturation(FacturationLigne facturationLigne)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Invoice_Line (ID_Invoice_Line, ID_Invoice, ID_Product, Quantity, Subtotal) VALUES (@idLine, @idInvoice, @idProduct, @quantity, @subtotal)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idLine", facturationLigne.ID_Invoice_Line);
                    cmd.Parameters.AddWithValue("@idInvoice", facturationLigne.ID_Invoice);
                    cmd.Parameters.AddWithValue("@idProduct", facturationLigne.ID_Product);
                    cmd.Parameters.AddWithValue("@quantity", facturationLigne.Quantity);
                    cmd.Parameters.AddWithValue("@subtotal", facturationLigne.Subtotal);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public bool AjouterLigneFacturationALaFacture(int idInvoice, int idProduct, int quantity)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    decimal price = ObtenirPrixProduit(idProduct);
                    decimal subtotal = price * quantity;

                    string query = "INSERT INTO Invoice_Line (ID_Invoice, ID_Product, Quantity, Subtotal) VALUES (@idInvoice, @idProduct, @quantity, @subtotal)";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@idInvoice", idInvoice);
                        cmd.Parameters.AddWithValue("@idProduct", idProduct);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@subtotal", subtotal);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Gérer l'exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public bool SupprimerLigneFacturation(int idInvoiceLine)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM Invoice_Line WHERE ID_Invoice_Line = @idLine";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@idLine", idInvoiceLine);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Gérer l'exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public decimal ObtenirPrixProduit(int idProduct)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT Price FROM Product WHERE ID_Product = @idProduct";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idProduct", idProduct);
                    return (decimal)cmd.ExecuteScalar();
                }
            }
        }







        #endregion

        #region Commerciaux
        // Pour récupérer la liste des commerciaux
        public List<Commercial> GetCommerciaux()
        {
            List<Commercial> listeCommerciaux = new List<Commercial>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Commercial", conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Commercial commercial = new Commercial(
                        Convert.ToInt32(reader["ID_Commercial"]),
                        Convert.ToString(reader["Lastname"]),
                        Convert.ToString(reader["Firstname"]),
                        Convert.ToString(reader["Email"]),
                        Convert.ToString(reader["Password"])
                    );
                    listeCommerciaux.Add(commercial);
                }
            }

            return listeCommerciaux;
        }

        public int GetNextCommercialID()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT IFNULL(MAX(ID_Commercial), 0) + 1 FROM Commercial";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void AjouterCommercial(Commercial commercial)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Commercial (ID_Commercial, Lastname, Firstname, Email, Password) VALUES (@id, @last, @first, @email, @password)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", commercial.ID_Commercial);
                    cmd.Parameters.AddWithValue("@last", commercial.Lastname);
                    cmd.Parameters.AddWithValue("@first", commercial.Firstname);
                    cmd.Parameters.AddWithValue("@email", commercial.Email);
                    cmd.Parameters.AddWithValue("@password", commercial.Password); // Assurez-vous que le mot de passe est traité de manière sécurisée.
                    cmd.ExecuteNonQuery();
                }
                string queryUser = "INSERT INTO Users (ID, Lastname, Firstname, Email, Password) VALUES (@id, @last, @first, @email, @password)";
                using (var cmdUser = new MySqlCommand(queryUser, connection))
                {
                    cmdUser.Parameters.AddWithValue("@id", commercial.ID_Commercial);
                    cmdUser.Parameters.AddWithValue("@last", commercial.Lastname);
                    cmdUser.Parameters.AddWithValue("@first", commercial.Firstname);
                    cmdUser.Parameters.AddWithValue("@email", commercial.Email);
                    cmdUser.Parameters.AddWithValue("@password", commercial.Password);
                    cmdUser.ExecuteNonQuery();
                }
            }
        }

        public void MettreAJourCommercial(Commercial commercial)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "UPDATE Commercial SET Lastname=@last, Firstname=@first, Email=@email, Password=@password WHERE ID_Commercial=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", commercial.ID_Commercial);
                    cmd.Parameters.AddWithValue("@last", commercial.Lastname);
                    cmd.Parameters.AddWithValue("@first", commercial.Firstname);
                    cmd.Parameters.AddWithValue("@email", commercial.Email);
                    cmd.Parameters.AddWithValue("@password", commercial.Password); // Assurez-vous que le mot de passe est traité de manière sécurisée.
                    cmd.ExecuteNonQuery();
                }
                string queryUser = "UPDATE Users SET Lastname=@last, Firstname=@first, Email=@email, Password=@password WHERE ID=@id";
                using (var cmdUser = new MySqlCommand(queryUser, connection))
                {
                    cmdUser.Parameters.AddWithValue("@id", commercial.ID_Commercial);
                    cmdUser.Parameters.AddWithValue("@last", commercial.Lastname);
                    cmdUser.Parameters.AddWithValue("@first", commercial.Firstname);
                    cmdUser.Parameters.AddWithValue("@email", commercial.Email);
                    cmdUser.Parameters.AddWithValue("@password", commercial.Password);
                    cmdUser.ExecuteNonQuery();
                }
            }
        }


        public void SupprimerCommercial(int idCommercial)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Commercial WHERE ID_Commercial=@id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idCommercial);
                    cmd.ExecuteNonQuery();
                }
                string queryUser = "DELETE FROM Users WHERE ID=@id";
                using (var cmdUser = new MySqlCommand(queryUser, connection))
                {
                    cmdUser.Parameters.AddWithValue("@id", idCommercial);
                    cmdUser.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region stat
        public int NombreTotalClients()
        {
            using (var connection = GetConnection())
            {
                 connection.Open();
                string query = "SELECT COUNT(*) FROM Client WHERE Type = 1";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public Dictionary<string, int> RepartitionClientsParVille()
        {
            var repartition = new Dictionary<string, int>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT City, COUNT(*) AS ClientCount FROM Client WHERE Type =1 GROUP BY City ORDER BY ClientCount DESC LIMIT 10";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            repartition.Add(reader.GetString(0), reader.GetInt32(1));
                        }
                    }
                }
            }
            return repartition;
        }


        public Dictionary<string, int> RepartitionClientsParPays()
        {
            var repartition = new Dictionary<string, int>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Country, COUNT(*) AS ClientCount FROM Client WHERE Type =1 GROUP BY Country ORDER BY ClientCount DESC LIMIT 10";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            repartition.Add(reader.GetString(0), reader.GetInt32(1));
                        }
                    }
                }
            }
            return repartition;
        }

        public Dictionary<string, decimal> GetTopClientsByPurchases()
        {
            var topClients = new Dictionary<string, decimal>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT c.Lastname, SUM(i.Total) AS TotalPurchases 
            FROM client c
            JOIN invoice i ON c.ID_Client = i.ID_Client
            GROUP BY c.ID_Client
            ORDER BY TotalPurchases DESC
            LIMIT 5";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            topClients.Add(reader.GetString(0), reader.GetDecimal(1));
                        }
                    }
                }
            }
            return topClients;
        }

        public Dictionary<string, decimal> GetBottomClientsByPurchases()
        {
            var bottomClients = new Dictionary<string, decimal>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT c.Lastname, SUM(i.Total) AS TotalPurchases 
            FROM client c
            JOIN invoice i ON c.ID_Client = i.ID_Client
            GROUP BY c.ID_Client
            ORDER BY TotalPurchases ASC 
            LIMIT 5";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bottomClients.Add(reader.GetString(0), reader.GetDecimal(1));
                        }
                    }
                }
            }
            return bottomClients;
        }




        public int NombreTotalProspects()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Client WHERE Type = 0";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public Dictionary<string, int> RepartitionProspectsParVille()
        {
            var repartition = new Dictionary<string, int>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT City, COUNT(*) AS ProspectCount FROM Client Where Type =0 GROUP BY City ORDER BY ProspectCount DESC LIMIT 10";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            repartition.Add(reader.GetString(0), reader.GetInt32(1));
                        }
                    }
                }
            }
            return repartition;
        }

        public Dictionary<string, int> RepartitionProspectsParPays()
        {
            var repartition = new Dictionary<string, int>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Country, COUNT(*) AS ProspectCount FROM Client Where Type =0 GROUP BY Country ORDER BY ProspectCount DESC LIMIT 10";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            repartition.Add(reader.GetString(0), reader.GetInt32(1));
                        }
                    }
                }
            }
            return repartition;
        }






        public double TauxConversionProspectsClients()
        {
            int totalClients = NombreTotalClients(); // Utilisez la méthode déjà existante
            int totalProspects = NombreTotalProspects();

            return totalProspects > 0 ? (double)totalClients / totalProspects * 100.0 : 0.0; // Converti en pourcentage
        }


        public Dictionary<string, int> NouveauxClientsParMoisAnnee()
        {
            var resultats = new Dictionary<string, int>();

            try
            {
                using (var conn = new MySqlConnection("votre_chaine_de_connexion"))
                {
                    conn.Open();
                    string query = @"
                SELECT DATE_FORMAT(DateAjout, '%Y-%m') AS MoisAnnee, COUNT(*) 
                FROM Client 
                GROUP BY MoisAnnee";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string moisAnnee = reader.GetString(0);
                                int nombre = reader.GetInt32(1);
                                resultats.Add(moisAnnee, nombre);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérer les exceptions ici
                Console.WriteLine("Erreur lors de la récupération des nouveaux clients par mois et année: " + ex.Message);
            }

            return resultats;
        }

        /* public Dictionary<string, int> GetProduitsLesPlusVendus()
         {
             var resultats = new Dictionary<string, int>();
             string query = @"
         SELECT p.Product_Name, COUNT(*) as NombreVentes
         FROM Ventes v
         JOIN Produit p ON v.ID_Product = p.ID_Product
         GROUP BY p.Product_Name
         ORDER BY NombreVentes DESC
         LIMIT 10"; // Limite à 10 pour les 10 produits les plus vendus

             // Exécution de la requête et remplissage du dictionnaire...
             // (Similaire à l'exemple précédent)

             return resultats;
         }*/

        public decimal ChiffreAffairesTotalPour2023()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"
            SELECT SUM(Total) 
            FROM Invoice 
            WHERE YEAR(Date_Invoice) = 2023";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }
        public decimal ChiffreAffairesParStatut(string statut)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT SUM(Total) FROM Invoice WHERE Payment_Status = @statut AND YEAR(Date_Invoice) = 2023";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@statut", statut);
                    var result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }


        public Dictionary<string, decimal> GetChiffreAffairesParMois()
        {
            var resultats = new Dictionary<string, decimal>();

            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"
            SELECT DATE_FORMAT(Date_Invoice, '%Y-%m') AS Mois, 
                   SUM(Total) AS ChiffreAffaires
            FROM Invoice
            WHERE YEAR(Date_Invoice) = YEAR(CURRENT_DATE)
            GROUP BY Mois";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string mois = reader.GetString(0);
                            decimal chiffreAffaires = reader.GetDecimal(1);
                            resultats.Add(mois, chiffreAffaires);
                        }
                    }
                }
            }

            return resultats;
        }

        public Dictionary<string, int> TopCinqProduitsVendus()
        {
            var topProduits = new Dictionary<string, int>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT Product.Product_Name, SUM(Invoice_Line.Quantity) AS TotalSold
            FROM Invoice_Line
            JOIN Product ON Invoice_Line.ID_Product = Product.ID_Product
            GROUP BY Invoice_Line.ID_Product
            ORDER BY TotalSold DESC
            LIMIT 5";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            topProduits.Add(reader.GetString(0), reader.GetInt32(1));
                        }
                    }
                }
            }
            return topProduits;
        }
        public decimal MoyenneMontantsFacturation()
        {
            decimal moyenne = 0.0m;
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT AVG(Total) FROM Invoice";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        moyenne = Convert.ToDecimal(result);
                    }
                }
            }
            return moyenne;
        }

        public Dictionary<string, decimal> ClientsGenerantLePlusDeRevenus()
        {
            var clientsRevenus = new Dictionary<string, decimal>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT Client.Lastname, Client.Firstname, SUM(Invoice.Total) AS TotalRevenue
            FROM Client
            JOIN Invoice ON Client.ID_Client = Invoice.ID_Client
            GROUP BY Client.ID_Client
            ORDER BY TotalRevenue DESC
            LIMIT 3"; 
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nomClient = reader.GetString(0) + " " + reader.GetString(1);
                            decimal revenu = reader.GetDecimal(2);
                            clientsRevenus.Add(nomClient, revenu);
                        }
                    }
                }
            }
            return clientsRevenus;
        }

        public Dictionary<string, int> RendezVousParCommercial()
        {
            var rendezVousCount = new Dictionary<string, int>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT Commercial.Lastname, Commercial.Firstname, COUNT(Appointment.ID_Rdv) AS NbRendezVous
            FROM Commercial
            LEFT JOIN Appointment ON Commercial.ID_Commercial = Appointment.ID_Commercial
            WHERE YEAR(Appointment.Date_Rdv) = 2023
            GROUP BY Commercial.ID_Commercial
            ORDER BY NbRendezVous DESC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nomCommercial = reader.GetString(0) + " " + reader.GetString(1);
                            int nbRendezVous = reader.GetInt32(2);
                            rendezVousCount.Add(nomCommercial, nbRendezVous);
                        }
                    }
                }
            }
            return rendezVousCount;
        }

        public Dictionary<string, Dictionary<int, int>> RendezVousCommerciauxParMois()
        {
            var resultats = new Dictionary<string, Dictionary<int, int>>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT 
                c.Lastname, c.Firstname, 
                MONTH(a.Date_Rdv) AS Mois, 
                COUNT(*) AS NbRendezVous 
            FROM Commercial c
            JOIN Appointment a ON c.ID_Commercial = a.ID_Commercial
            WHERE YEAR(a.Date_Rdv) = YEAR(CURDATE())
            GROUP BY c.ID_Commercial, MONTH(a.Date_Rdv)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nomCommercial = reader.GetString(0) + " " + reader.GetString(1);
                            int mois = reader.GetInt32(2);
                            int nbRendezVous = reader.GetInt32(3);

                            if (!resultats.ContainsKey(nomCommercial))
                            {
                                resultats[nomCommercial] = new Dictionary<int, int>();
                            }

                            resultats[nomCommercial][mois] = nbRendezVous;
                        }
                    }
                }
            }

            return resultats;
        }




    }
    #endregion
}
