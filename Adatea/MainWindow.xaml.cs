
using Adatea.Classe;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;   
using LiveCharts.Wpf;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
 

namespace Adatea
{
    #region début
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BDD bdd = new BDD();

        // Vous avez une liste pour les Pigistes, ajoutez donc une liste pour les Clients
        List<Client> lesClients = new List<Client>();
        List<Prospect> lesProspects = new List<Prospect>();
        List<Rendezvous> lesRendezvous = new List<Rendezvous>();
        List<Produit> lesProduits = new List<Produit>();
        List<Facturation> lesFacturations = new List<Facturation>();
        List<Commercial> lesCommerciaux = new List<Commercial>();
        List<FacturationLigne> lesLignesFacturation = new List<FacturationLigne>();



        // Dans votre constructeur (MainWindow), vous initialisez vos listes, faites la même chose pour les Clients :
        public MainWindow()
        {

            InitializeComponent();
            ChargerStatistiques();
            ChargerGraphiqueChiffreAffaires();
            TxtQuantite.TextChanged += TxtQuantite_TextChanged;



            // Initialisation des identifiants pour les différentes entités
            SetNextClientID();
            SetNextProspectID();
            SetNextRendezvousID();
            SetNextProductID();
            SetNextInvoiceID();
            SetNextInvoiceLineID();
            SetNextCommercialID();

            InitializeRendezvousLocations();    // Initialisation des données pour CbxRendezvousLocation

            // Chargement des données dans les DataGrids
            lesClients = bdd.GetClients().OrderBy(c => c.ID_Client).ToList(); //ordre creoissant
            DtgClients.ItemsSource = lesClients;

            lesProspects = bdd.GetProspects().OrderBy(p => p.ID_Prospect).ToList();//ordre creoissant
            DtgProspects.ItemsSource = lesProspects;

            lesRendezvous = bdd.GetRendezvous().OrderBy(r => r.ID_Rdv).ToList();//ordre creoissant
            DtgRendezvous.ItemsSource = lesRendezvous;

            lesProduits = bdd.GetProduits().OrderBy(p => p.ID_Product).ToList();//ordre creoissant
            DtgProduits.ItemsSource = lesProduits;

            lesFacturations = bdd.GetFacturations().OrderBy(f => f.ID_Invoice).ToList();//ordre creoissant
            DtgFacturations.ItemsSource = lesFacturations;

            lesLignesFacturation = bdd.GetLignesFacturation().OrderBy(lf => lf.ID_Invoice_Line).ToList();
            DtgProduitsFacturation.ItemsSource = null;  // permet de ne pas afficher des le début

            lesCommerciaux = bdd.GetCommerciaux().OrderBy(c => c.ID_Commercial).ToList();//ordre creoissant
            DtgCommerciaux.ItemsSource = lesCommerciaux;

            // Charger les ID  dans les ComboBoxs
            LoadProspectIDs();
            LoadClientIDs();
            LoadCommercialIDs();
            LoadProductIDs();

            // Gestionnaires d'événements
            DtgClients.Loaded += DtgClients_Loaded;


        }

        // Initialisation des emplacements de rendez-vous disponibles
        private void InitializeRendezvousLocations()
        {
            var locations = new List<string> {
            "Teams",
            "Restaurant Chez Toto",
            "Restaurant La Table Parisienne",
            "Restaurant Le Bistro du Coin ",
            "Restaurant La fourchette Dorée",
            "Bureau principal",

             };

            // Attribution de la liste à la source de données de la ComboBox
            CbxRendezvousLocation.ItemsSource = locations;
        }

        // Réinitialisation des champs  lors du clic sur une zone vide du DataGrid
        private void DtgClients_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                TxtClientID.Text = "";
                TxtClientLastname.Text = "";
                TxtClientFirstname.Text = "";
                TxtClientEmail.Text = "";
                TxtClientStreet.Text = "";
                TxtClientPostalCode.Text = "";
                TxtClientCity.Text = "";
                TxtClientCountry.Text = "";
                TxtClientPhoneNumber.Text = "";
                CbxClientProspectID.SelectedItem = null;
                DtgClients.SelectedItem = null;
            }
        }
        // Réinitialisation des champs  lors du clic sur une zone vide du DataGrid

        private void DtgProspects_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                TxtProspectID.Text = "";
                TxtProspectLastname.Text = "";
                TxtProspectFirstname.Text = "";
                TxtProspectEmail.Text = "";
                TxtProspectStreet.Text = "";
                TxtProspectPostalCode.Text = "";
                TxtProspectCity.Text = "";
                TxtProspectCountry.Text = "";
                TxtProspectPhoneNumber.Text = "";
                DtgProspects.SelectedItem = null;
            }
        }

        // Chargement des identifiants  dans les ComboBox
        private void LoadProspectIDs()
        {
            var prospects = bdd.GetProspects(); // Cette méthode doit retourner une liste de tous les prospects
            var comboBoxItems = prospects.Select(p => $"{p.ID_Prospect} - {p.Firstname} - {p.Lastname}").ToList();

            CbxClientProspectID.ItemsSource = comboBoxItems;
            CbxRendezvousProspectID.ItemsSource = comboBoxItems;


        }

        // Chargement des identifiants  dans les ComboBox

        private void LoadClientIDs()
        {
            var clients = bdd.GetClients(); // Récupère tous les clients
            // Trier les clients par ID en ordre croissant
            var sortedClients = clients.OrderBy(c => c.ID_Client).ToList();

            // Utilisez la liste triée pour créer comboBoxItems
            var comboBoxItems = sortedClients.Select(c => $"{c.ID_Client} - {c.Firstname} - {c.Lastname}").ToList();

            CboFacturationClientID.ItemsSource = comboBoxItems;
            CbxRendezvousClientID.ItemsSource = comboBoxItems;
        }



        // Chargement des identifiants  dans les ComboBox

        private void LoadCommercialIDs()
        {
            var commerciaux = bdd.GetCommerciaux(); // Cette méthode doit retourner une liste de tous les commerciaux
            var comboBoxItems = commerciaux.Select(c => $"{c.ID_Commercial} - {c.Firstname} - {c.Lastname}").ToList();

            CbxRendezvousCommercialID.ItemsSource = comboBoxItems;
        }

        // Chargement des identifiants  dans les ComboBox

        private void LoadProductIDs()
        {
            var produits = bdd.GetProduits(); // Récupère tous les produits
            var comboBoxItems = produits.Select(p => $"{p.ID_Product} - {p.Product_Name}").ToList();
            CboProduits.ItemsSource = comboBoxItems;
        }




        private void TxtQuantite_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CboProduits.SelectedItem != null && int.TryParse(TxtQuantite.Text, out int quantite) && quantite > 0)
            {
                try
                {
                    string selectedText = CboProduits.SelectedItem.ToString();
                    int selectedID = int.Parse(selectedText.Split('-')[0].Trim());
                    Produit selectedProduct = bdd.GetProduitByID(selectedID);
                    if (selectedProduct != null)
                    {
                        decimal sousTotal = selectedProduct.Price * quantite;
                        TxtPrix.Text = sousTotal.ToString("F2");
                    }
                }
                catch
                {
                    // Gestion d'erreur si nécessaire
                }
            }
            else
            {
                TxtPrix.Text = "";
            }
        }










        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash;
            }
        }
        #endregion

        #region Clients

        private void DtgClients_Loaded(object sender, RoutedEventArgs e)
        {
            DtgClients.SelectedIndex = -1;
        }

        private void SetNextClientID()
        {
            int nextID = bdd.GetNextClientID();
            TxtClientID.Text = nextID.ToString();
        }

        // Réinitialisation des champs client
        private void ClearClientFields()
        {
            TxtClientID.Text = "";
            TxtClientLastname.Text = "";
            TxtClientFirstname.Text = "";
            TxtClientEmail.Text = "";
            TxtClientStreet.Text = "";
            TxtClientPostalCode.Text = "";
            TxtClientCity.Text = "";
            TxtClientCountry.Text = "";
            TxtClientPhoneNumber.Text = "";
            CbxClientProspectID.SelectedItem = null;
        }

        private void ClearCommercialFields()
        {
            TxtCommercialID.Text = "";
            TxtCommercialLastname.Text = "";
            TxtCommercialFirstname.Text = "";
            TxtCommercialEmail.Text = "";
            PbxCommercialPassword.Password = null;
        }

        private void ClearRendezvousFields()
        {
            // Réinitialisez les champs texte et les sélections des ComboBox pour les détails du rendez-vous
            TxtRendezvousID.Text = "";
            CbxRendezvousProspectID.SelectedIndex = -1; // Réinitialiser la sélection du prospect
            CbxRendezvousClientID.SelectedIndex = -1; // Réinitialiser la sélection du client
            CbxRendezvousCommercialID.SelectedIndex = -1; // Réinitialiser la sélection du commercial
            DtpRendezvousDate.SelectedDate = null; // Réinitialiser la date du rendez-vous
            TxtRendezvousTime.Text = ""; // Réinitialiser l'heure du rendez-vous
            CbxRendezvousLocation.SelectedIndex = -1; // Réinitialiser la sélection du lieu
        }
        private void ClearProductFields()
        {
            TxtIdLigneFacturation.Text = "";
            TxtIdFacturation.Text = "";
            CboProduits.SelectedIndex = -1;
            TxtNomProduit.Text = "";
            TxtDescription.Text = "";
            TxtQuantite.Text = "";
            TxtPrix.Text = "";
        }



        // Remplissage des détails du prospect dans les champs client
        private void FillProspectDetails(Prospect prospect)
        {
            if (prospect != null)
            {
                TxtProspectLastname.Text = prospect.Lastname;
                TxtProspectFirstname.Text = prospect.Firstname;
                TxtProspectEmail.Text = prospect.Email;
                TxtProspectStreet.Text = prospect.Street;
                TxtProspectPostalCode.Text = prospect.PostalCode;
                TxtProspectCity.Text = prospect.City;
                TxtProspectCountry.Text = prospect.Country;
                TxtProspectPhoneNumber.Text = prospect.Phone_Number.ToString();
            }
        }

        // Gestion de la sélection d'un prospect dans la ComboBox
        private void CbxClientProspectID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbxClientProspectID.SelectedItem != null)
            {
                string selectedText = CbxClientProspectID.SelectedItem.ToString();
                int selectedID = int.Parse(selectedText.Split('-')[0].Trim());
                Prospect selectedProspect = bdd.GetProspectByID(selectedID);
                FillProspectDetails(selectedProspect);

                TxtClientLastname.Text = selectedProspect.Lastname;
                TxtClientFirstname.Text = selectedProspect.Firstname;
                TxtClientEmail.Text = selectedProspect.Email;
                TxtClientStreet.Text = selectedProspect.Street;
                TxtClientPostalCode.Text = selectedProspect.PostalCode;
                TxtClientCity.Text = selectedProspect.City;
                TxtClientCountry.Text = selectedProspect.Country;
                TxtClientPhoneNumber.Text = selectedProspect.Phone_Number.ToString();
            }
        }


        // Ajout d'un nouveau client
        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtClientLastname.Text) &&
                !string.IsNullOrWhiteSpace(TxtClientFirstname.Text) &&
                !string.IsNullOrWhiteSpace(TxtClientEmail.Text) &&
                !string.IsNullOrWhiteSpace(TxtClientStreet.Text) && // Mise à jour pour la rue
                !string.IsNullOrWhiteSpace(TxtClientPostalCode.Text) && // Mise à jour pour le code postal
                !string.IsNullOrWhiteSpace(TxtClientCity.Text) && // Mise à jour pour la ville
                !string.IsNullOrWhiteSpace(TxtClientCountry.Text) && // Mise à jour pour le pays
                !string.IsNullOrWhiteSpace(TxtClientPhoneNumber.Text) &&
                CbxClientProspectID.SelectedItem != null)
            {
                string selectedProspect = CbxClientProspectID.SelectedItem.ToString();
                int prospectID = int.Parse(selectedProspect.Split('-')[0].Trim()); // Extrait l'ID
                string prospectName = selectedProspect.Split('-')[1].Trim();

                MessageBoxResult result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir transférer le prospect {prospectID} - {prospectName} en tant que client?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    int clientId = bdd.GetNextClientID();

                    Client aClient = new Client(
                        clientId,
                        TxtClientLastname.Text,
                        TxtClientFirstname.Text,
                        TxtClientEmail.Text,
                        TxtClientStreet.Text, // Nouveau champ pour la rue
                        TxtClientPostalCode.Text, // Nouveau champ pour le code postal
                        TxtClientCity.Text, // Nouveau champ pour la ville
                        TxtClientCountry.Text, // Nouveau champ pour le pays
                        Convert.ToInt32(TxtClientPhoneNumber.Text),
                        prospectID
                    );

                    // Met à jour l'état du prospect pour le marquer comme client
                    bdd.ConvertirProspectEnClient(prospectID);

                    //bdd.AjouterClient(aClient);
                    lesClients.Add(aClient);
                    DtgClients.Items.Refresh();


                    // Supprimer le prospect de la liste en mémoire
                    Prospect prospectASupprimer = lesProspects.FirstOrDefault(p => p.ID_Prospect == prospectID);
                    if (prospectASupprimer != null)
                    {
                        lesProspects.Remove(prospectASupprimer);
                        DtgProspects.ItemsSource = null;
                        DtgProspects.ItemsSource = lesProspects;
                        DtgProspects.Items.Refresh();
                    }

                    ClearClientFields();
                    ClearProspectFields();
                    SetNextClientID(); // Mettre à jour le prochain ID disponible dans l'interface utilisateur
                    ChargerStatistiques(); // Rafraîchir les statistiques
                    LoadProspectIDs();
                    LoadClientIDs();
                }
            }
            else
            {
                MessageBox.Show("Merci de remplir tous les champs avant d'ajouter un client", "Information manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Mise à jour des informations d'un client
        private void BtnUpdateClient_Click(object sender, RoutedEventArgs e)
        {
            if (DtgClients.SelectedItem is Client selectedClient)
            {
                int newId = Convert.ToInt32(TxtClientID.Text);
                // Vérifiez si l'ID a été modifié et si le nouvel ID existe déjà dans la base de données
                if (selectedClient.ID_Client != newId && bdd.VerifierSiClientExiste(newId))
                {
                    MessageBox.Show("L'ID de client existe déjà. Veuillez en choisir un autre.", "ID Existant", MessageBoxButton.OK, MessageBoxImage.Error);
                    TxtClientID.Text = selectedClient.ID_Client.ToString(); // Réinitialisez le champ ID avec l'ancien ID
                }
                else
                {
                    int indice = lesClients.IndexOf(selectedClient);

                    selectedClient.ID_Client = newId;
                    selectedClient.Lastname = TxtClientLastname.Text;
                    selectedClient.Firstname = TxtClientFirstname.Text;
                    selectedClient.Email = TxtClientEmail.Text;
                    selectedClient.Street = TxtClientStreet.Text;
                    selectedClient.PostalCode = TxtClientPostalCode.Text;
                    selectedClient.City = TxtClientCity.Text;
                    selectedClient.Country = TxtClientCountry.Text;
                    selectedClient.Phone_Number = Convert.ToInt32(TxtClientPhoneNumber.Text);

                    // Assurez-vous de gérer correctement l'ID_Prospect
                    ClearClientFields();
                    bdd.MettreAJourClient(lesClients.ElementAt(indice));
                    DtgClients.Items.Refresh();
                    ChargerStatistiques();// Rafraîchir les statistiques
                    LoadClientIDs();

                }
            }
        }
        // Suppression d'un client
        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (DtgClients.SelectedItem is Client selectedClient)
            {
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le client ID: {selectedClient.ID_Client}?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bdd.SupprimerClient(selectedClient.ID_Client);
                    lesClients.Remove(selectedClient);
                    DtgClients.Items.Refresh();
                    ChargerStatistiques(); // Rafraîchir les statistiques
                    LoadClientIDs();
                }
            }
            else
            {
                MessageBox.Show("Merci de sélectionner une ligne avant de cliquer sur Supprimer", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        // Gestion de la sélection d'un client dans le DataGrid
        private void DtgClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DtgClients.SelectedItem is Client selectedClient)
            {
                TxtClientID.Text = selectedClient.ID_Client.ToString();
                CbxClientProspectID.SelectedValue = selectedClient.ID_Prospect.ToString();
                TxtClientLastname.Text = selectedClient.Lastname;
                TxtClientFirstname.Text = selectedClient.Firstname;
                TxtClientEmail.Text = selectedClient.Email;
                TxtClientStreet.Text = selectedClient.Street;
                TxtClientPostalCode.Text = selectedClient.PostalCode;
                TxtClientCity.Text = selectedClient.City;
                TxtClientCountry.Text = selectedClient.Country;
                TxtClientPhoneNumber.Text = selectedClient.Phone_Number.ToString();

                // Mettre à jour la ComboBox avec l'ID_Prospect du client sélectionné
                CbxClientProspectID.SelectedValue = selectedClient.ID_Prospect;
            }
            else
            {
                // Réinitialiser les champs et la ComboBox
                ClearClientFields();
            }
        }



        #endregion

        #region Prospects 



        private void SetNextProspectID()
        {
            int nextID = bdd.GetNextProspectID();
            TxtProspectID.Text = nextID.ToString();
        }

        // Réinitialisation des champs prospect
        private void ClearProspectFields()
        {
            TxtProspectID.Text = "";
            TxtProspectLastname.Text = "";
            TxtProspectFirstname.Text = "";
            TxtProspectEmail.Text = "";
            TxtProspectStreet.Text = "";
            TxtProspectPostalCode.Text = "";
            TxtProspectCity.Text = "";
            TxtProspectCountry.Text = "";
            TxtProspectPhoneNumber.Text = "";
        }



        // Ajout d'un nouveau prospect
        private void BtnAddProspect_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtProspectLastname.Text) &&
                !string.IsNullOrWhiteSpace(TxtProspectFirstname.Text) &&
                !string.IsNullOrWhiteSpace(TxtProspectEmail.Text) &&
                !string.IsNullOrWhiteSpace(TxtProspectStreet.Text) &&
                !string.IsNullOrWhiteSpace(TxtProspectPostalCode.Text) &&
                !string.IsNullOrWhiteSpace(TxtProspectCity.Text) &&
                !string.IsNullOrWhiteSpace(TxtProspectCountry.Text) &&
                !string.IsNullOrWhiteSpace(TxtProspectPhoneNumber.Text))
            {
                if (DtgProspects.SelectedItem == null)
                {
                    int prospectId = bdd.GetNextProspectID();

                    Prospect aProspect = new Prospect(
                        prospectId,
                        TxtProspectLastname.Text,
                        TxtProspectFirstname.Text,
                        TxtProspectEmail.Text,
                        TxtProspectStreet.Text,
                        TxtProspectPostalCode.Text,
                        TxtProspectCity.Text,
                        TxtProspectCountry.Text,
                        Convert.ToInt32(TxtProspectPhoneNumber.Text)
                    );

                    bdd.AjouterProspect(aProspect);
                    lesProspects.Add(aProspect);
                    DtgProspects.Items.Refresh();

                    ClearProspectFields();
                    SetNextProspectID();
                    LoadProspectIDs();
                    LoadClientIDs();
                    ChargerStatistiques();
                }
                else
                {
                    MessageBox.Show("Un prospect est déjà sélectionné. Veuillez le mettre à jour ou en sélectionner un autre.", "Opération invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Merci de remplir tous les champs avant d'ajouter un prospect", "Information manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Mise à jour des informations d'un prospect
        private void BtnUpdateProspect_Click(object sender, RoutedEventArgs e)
        {
            if (DtgProspects.SelectedItem is Prospect selectedProspect)
            {
                int newId = Convert.ToInt32(TxtProspectID.Text);
                // Vérifiez si l'ID a été modifié et si le nouvel ID existe déjà dans la base de données
                if (selectedProspect.ID_Prospect != newId && bdd.VerifierSiProspectExiste(newId))
                {
                    MessageBox.Show("L'ID de prospect existe déjà. Veuillez en choisir un autre.", "ID Existant", MessageBoxButton.OK, MessageBoxImage.Error);
                    TxtProspectID.Text = selectedProspect.ID_Prospect.ToString(); // Réinitialisez le champ ID avec l'ancien ID
                }
                else
                {
                    int indice = lesProspects.IndexOf(selectedProspect);

                    lesProspects.ElementAt(indice).ID_Prospect = newId;
                    lesProspects.ElementAt(indice).Lastname = TxtProspectLastname.Text;
                    lesProspects.ElementAt(indice).Firstname = TxtProspectFirstname.Text;
                    lesProspects.ElementAt(indice).Email = TxtProspectEmail.Text;
                    lesProspects.ElementAt(indice).Street = TxtProspectStreet.Text;
                    lesProspects.ElementAt(indice).PostalCode = TxtProspectPostalCode.Text;
                    lesProspects.ElementAt(indice).City = TxtProspectCity.Text;
                    lesProspects.ElementAt(indice).Country = TxtProspectCountry.Text;
                    lesProspects.ElementAt(indice).Phone_Number = Convert.ToInt32(TxtProspectPhoneNumber.Text);

                    bdd.MettreAJourProspect(lesProspects.ElementAt(indice));
                    DtgProspects.Items.Refresh();
                    ClearProspectFields();
                    LoadProspectIDs();
                    ChargerStatistiques();

                }
            }
        }

        // Suppression d'un prospect
        private void BtnDeleteProspect_Click(object sender, RoutedEventArgs e)
        {
            Prospect prospectASupprimer = (Prospect)DtgProspects.SelectedItem;

            if (DtgProspects.SelectedItem is Prospect selectedProspect)
            {
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le prospect ID: {selectedProspect.ID_Prospect}?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bdd.SupprimerProspect(selectedProspect.ID_Prospect);
                    lesProspects.Remove(selectedProspect);
                    DtgProspects.Items.Refresh();
                    LoadProspectIDs();
                    ChargerStatistiques();

                }
            }
            else
            {
                MessageBox.Show("Merci de sélectionner une ligne avant de cliquer sur Supprimer", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Gestion de la sélection d'un prospect dans le DataGrid
        private void DtgProspects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DtgProspects.SelectedItem is Prospect selectedProspect)
            {
                TxtProspectID.Text = selectedProspect.ID_Prospect.ToString();
                TxtProspectLastname.Text = selectedProspect.Lastname;
                TxtProspectFirstname.Text = selectedProspect.Firstname;
                TxtProspectEmail.Text = selectedProspect.Email;
                TxtProspectStreet.Text = selectedProspect.Street;
                TxtProspectPostalCode.Text = selectedProspect.PostalCode;
                TxtProspectCity.Text = selectedProspect.City;
                TxtProspectCountry.Text = selectedProspect.Country;
                TxtProspectPhoneNumber.Text = selectedProspect.Phone_Number.ToString();
            }
            else
            {
                // Videz les TextBox si aucun prospect n'est sélectionné
                SetNextProspectID();
                TxtProspectLastname.Text = "";
                TxtProspectFirstname.Text = "";
                TxtProspectEmail.Text = "";
                TxtProspectStreet.Text = "";
                TxtProspectPostalCode.Text = "";
                TxtProspectCity.Text = "";
                TxtProspectCountry.Text = "";
                TxtProspectPhoneNumber.Text = "";
            }
        }




        #endregion

        #region Rendezvous


        private void SetNextRendezvousID()
        {
            int nextID = bdd.GetNextRendezvousID();
            TxtRendezvousID.Text = nextID.ToString();
        }

        private void DtgRendezvous_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                TxtRendezvousID.Text = "";
                CbxRendezvousProspectID.SelectedIndex = -1;
                CbxRendezvousClientID.SelectedIndex = -1;
                CbxRendezvousCommercialID.SelectedIndex = -1;
                DtpRendezvousDate.SelectedDate = null;
                TxtRendezvousTime.Text = "";
                CbxRendezvousLocation.Text = "";
                DtgRendezvous.SelectedItem = null;
            }
        }

        private void BtnAddRendezvous_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtRendezvousID.Text) &&
                (CbxRendezvousProspectID.SelectedItem != null || CbxRendezvousClientID.SelectedItem != null) &&
                CbxRendezvousCommercialID.SelectedItem != null &&
                DtpRendezvousDate.SelectedDate.HasValue &&
                !string.IsNullOrWhiteSpace(TxtRendezvousTime.Text) &&
                !string.IsNullOrWhiteSpace(CbxRendezvousLocation.Text))
            {
                int? prospectID = CbxRendezvousProspectID.SelectedItem != null ? Convert.ToInt32(CbxRendezvousProspectID.SelectedValue.ToString().Split('-')[0].Trim()) : (int?)null;
                int? clientID = CbxRendezvousClientID.SelectedItem != null ? Convert.ToInt32(CbxRendezvousClientID.SelectedValue.ToString().Split('-')[0].Trim()) : (int?)null;
                int commercialID = Convert.ToInt32(CbxRendezvousCommercialID.SelectedValue.ToString().Split('-')[0].Trim());

                if (DtgRendezvous.SelectedItem == null)
                {
                    int rendezvousID = bdd.GetNextRendezvousID();

                    Rendezvous aRendezvous = new Rendezvous(rendezvousID, prospectID, clientID, commercialID, DtpRendezvousDate.SelectedDate.Value, TimeSpan.Parse(TxtRendezvousTime.Text), CbxRendezvousLocation.Text);

                    bdd.AjouterRendezvous(aRendezvous);
                    lesRendezvous.Add(aRendezvous);
                    DtgRendezvous.Items.Refresh();

                    SetNextRendezvousID();
                    ClearRendezvousFields();
                }
                else
                {
                    MessageBox.Show("Un rendez-vous est déjà sélectionné.", "Opération invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Veuillez remplir tous les champs nécessaires.", "Information manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void BtnUpdateRendezvous_Click(object sender, RoutedEventArgs e)
        {
            if (DtgRendezvous.SelectedItem is Rendezvous selectedRendezvous)
            {
                // Vérifiez qu'un seul des deux est sélectionné
                if ((CbxRendezvousProspectID.SelectedItem != null) != (CbxRendezvousClientID.SelectedItem != null))
                {
                    int index = lesRendezvous.IndexOf(selectedRendezvous);

                    selectedRendezvous.ID_Rdv = Convert.ToInt32(TxtRendezvousID.Text);
                    selectedRendezvous.ID_Prospect = CbxRendezvousProspectID.SelectedItem != null
                        ? Convert.ToInt32(CbxRendezvousProspectID.SelectedValue.ToString().Split('-')[0].Trim())
                        : (int?)null;
                    selectedRendezvous.ID_Client = CbxRendezvousClientID.SelectedItem != null
                        ? Convert.ToInt32(CbxRendezvousClientID.SelectedValue.ToString().Split('-')[0].Trim())
                        : (int?)null;
                    selectedRendezvous.ID_Commercial = Convert.ToInt32(CbxRendezvousCommercialID.SelectedValue.ToString().Split('-')[0].Trim());
                    selectedRendezvous.Date_Rdv = DtpRendezvousDate.SelectedDate.Value;
                    selectedRendezvous.Time_Rdv = TimeSpan.Parse(TxtRendezvousTime.Text);
                    selectedRendezvous.Location = CbxRendezvousLocation.Text;

                    bdd.MettreAJourRendezvous(lesRendezvous.ElementAt(index));
                    DtgRendezvous.Items.Refresh();
                    ClearRendezvousFields();

                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner soit un prospect, soit un client.", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private void BtnDeleteRendezvous_Click(object sender, RoutedEventArgs e)
        {
            if (DtgRendezvous.SelectedItem is Rendezvous selectedRendezvous)
            {
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le rendez-vous ID: {selectedRendezvous.ID_Rdv}?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bdd.SupprimerRendezvous(selectedRendezvous.ID_Rdv);
                    lesRendezvous.Remove(selectedRendezvous);
                    DtgRendezvous.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Merci de sélectionner une ligne avant de cliquer sur Supprimer", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DtgRendezvous_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DtgRendezvous.SelectedItem is Rendezvous selectedRendezvous)
            {
                TxtRendezvousID.Text = selectedRendezvous.ID_Rdv.ToString();
                CbxRendezvousProspectID.SelectedValue = selectedRendezvous.ID_Prospect.HasValue ? $"{selectedRendezvous.ID_Prospect} - ..." : null;
                CbxRendezvousClientID.SelectedValue = selectedRendezvous.ID_Client.HasValue ? $"{selectedRendezvous.ID_Client} - ..." : null;
                CbxRendezvousCommercialID.SelectedValue = $"{selectedRendezvous.ID_Commercial} - ...";
                DtpRendezvousDate.SelectedDate = selectedRendezvous.Date_Rdv;
                TxtRendezvousTime.Text = selectedRendezvous.Time_Rdv.ToString(@"hh\:mm");
                CbxRendezvousLocation.Text = selectedRendezvous.Location;
            }
            else
            {
                // Réinitialiser les champs si aucun élément n'est sélectionné
                SetNextRendezvousID();
                CbxRendezvousProspectID.SelectedValue = null;
                CbxRendezvousClientID.SelectedValue = null;
                CbxRendezvousCommercialID.SelectedValue = null;
                DtpRendezvousDate.SelectedDate = null;
                TxtRendezvousTime.Text = "";
                CbxRendezvousLocation.Text = "";
            }
        }


        #endregion

        #region Produits

        private void SetNextProductID()
        {
            int nextID = bdd.GetNextProductID();
            TxtProduitID.Text = nextID.ToString();
        }

        private void DtgProduits_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                TxtProduitID.Text = "";
                TxtProduitName.Text = "";
                TxtProduitDescription.Text = "";
                TxtProduitPrice.Text = "";
                TxtProduitStockQuantity.Text = "";
                DtgProduits.SelectedItem = null;
            }
        }


        private void BtnAddProduit_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtProduitName.Text) &&
                !string.IsNullOrWhiteSpace(TxtProduitDescription.Text) &&
                !string.IsNullOrWhiteSpace(TxtProduitPrice.Text) &&
                !string.IsNullOrWhiteSpace(TxtProduitStockQuantity.Text))
            {
                if (DtgProduits.SelectedItem == null)
                {
                    int productId = bdd.GetNextProductID();

                    Produit unProduit = new Produit(
                    productId,
                    TxtProduitName.Text,
                    TxtProduitDescription.Text,
                    Convert.ToDecimal(TxtProduitPrice.Text),
                    Convert.ToInt32(TxtProduitStockQuantity.Text)
                );

                    bdd.AjouterProduit(unProduit);
                    lesProduits.Add(unProduit);
                    DtgProduits.Items.Refresh();
                    LoadProductIDs();
                    SetNextProductID();
                }
                else
                {
                    MessageBox.Show("Un produit est déjà sélectionné. Veuillez le mettre à jour ou en sélectionner un autre.", "Opération invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Merci de remplir tous les champs avant d'ajouter un produit", "Information manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void BtnUpdateProduit_Click(object sender, RoutedEventArgs e)
        {
            if (DtgProduits.SelectedItem is Produit selectedProduit)
            {
                int index = lesProduits.IndexOf(selectedProduit);

                lesProduits.ElementAt(index).ID_Product = Convert.ToInt32(TxtProduitID.Text);
                lesProduits.ElementAt(index).Product_Name = TxtProduitName.Text;
                lesProduits.ElementAt(index).Description = TxtProduitDescription.Text;
                lesProduits.ElementAt(index).Price = Convert.ToDecimal(TxtProduitPrice.Text);
                lesProduits.ElementAt(index).Stock_Quantity = Convert.ToInt32(TxtProduitStockQuantity.Text);

                bdd.MettreAJourProduit(selectedProduit); // Ajoutez cette ligne pour mettre à jour dans la base de données

                DtgProduits.Items.Refresh();
            }
        }


        private void BtnDeleteProduit_Click(object sender, RoutedEventArgs e)
        {
            if (DtgProduits.SelectedItem is Produit selectedProduit)
            {
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le produit ID: {selectedProduit.ID_Product}?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bdd.SupprimerProduit(selectedProduit.ID_Product);
                    lesProduits.Remove(selectedProduit);
                    DtgProduits.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Merci de sélectionner une ligne avant de cliquer sur Supprimer", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DtgProduits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DtgProduits.SelectedItem is Produit selectedProduit)
            {
                TxtProduitID.Text = selectedProduit.ID_Product.ToString();
                TxtProduitName.Text = selectedProduit.Product_Name;
                TxtProduitDescription.Text = selectedProduit.Description;
                TxtProduitPrice.Text = selectedProduit.Price.ToString("F2"); // Affiche le prix avec deux décimales
                TxtProduitStockQuantity.Text = selectedProduit.Stock_Quantity.ToString();
            }
            else
            {
                // Reset des TextBox si aucun produit n'est sélectionné
                SetNextProductID();
                TxtProduitName.Text = "";
                TxtProduitDescription.Text = "";
                TxtProduitPrice.Text = "";
                TxtProduitStockQuantity.Text = "";
            }
        }
        #endregion

        #region Facturation




        private void SetNextInvoiceID()
        {
            int nextID = bdd.GetNextInvoiceID();
            TxtFacturationID.Text = nextID.ToString();
        }
        private void DtgFacturations_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                TxtFacturationID.Text = "";
                CboFacturationClientID.SelectedIndex = -1;
                DtpFacturationDate.SelectedDate = null;
                TxtFacturationTotal.Text = "";
                CboFacturationPaymentStatus.SelectedIndex = -1;
                DtgFacturations.SelectedItem = null;
            }
        }







        private void BtnAddFacturation_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtFacturationTotal.Text) &&
                CboFacturationClientID.SelectedItem != null &&
                DtpFacturationDate.SelectedDate.HasValue &&
                !string.IsNullOrWhiteSpace(CboFacturationPaymentStatus.Text))
            {
                if (DtgFacturations.SelectedItem == null)
                {
                    int invoiceId = bdd.GetNextInvoiceID();

                    Facturation uneFacturation = new Facturation(
                        invoiceId,
                        Convert.ToInt32(CboFacturationClientID.SelectedValue.ToString().Split('-')[0].Trim()),
                        DtpFacturationDate.SelectedDate.Value,
                        Convert.ToDecimal(TxtFacturationTotal.Text),
                        CboFacturationPaymentStatus.Text
                    );

                    bdd.AjouterFacturation(uneFacturation);
                    lesFacturations.Add(uneFacturation);
                    DtgFacturations.Items.Refresh();
                    ChargerStatistiques(); // Rafraîchir les statistiques

                    SetNextInvoiceID();
                }
                else
                {
                    MessageBox.Show("Une facturation est déjà sélectionnée. Veuillez la mettre à jour ou en sélectionner une autre.", "Opération invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Merci de remplir tous les champs avant d'ajouter une facturation", "Information manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void BtnUpdateFacturation_Click(object sender, RoutedEventArgs e)
        {
            if (DtgFacturations.SelectedItem is Facturation selectedFacturation)
            {
                int index = lesFacturations.IndexOf(selectedFacturation);

                lesFacturations.ElementAt(index).ID_Invoice = Convert.ToInt32(TxtFacturationID.Text);
                lesFacturations.ElementAt(index).ID_Client = Convert.ToInt32(CboFacturationClientID.Text);
                lesFacturations.ElementAt(index).Date_Invoice = Convert.ToDateTime(DtpFacturationDate.SelectedDate);
                lesFacturations.ElementAt(index).Total = Convert.ToDecimal(TxtFacturationTotal.Text);
                lesFacturations.ElementAt(index).Payment_Status = CboFacturationPaymentStatus.Text;

                bdd.MettreAJourFacturation(selectedFacturation);
                DtgFacturations.Items.Refresh();
                ChargerStatistiques(); // Rafraîchir les statistiques

            }
        }

        private void BtnDeleteFacturation_Click(object sender, RoutedEventArgs e)
        {
            if (DtgFacturations.SelectedItem is Facturation selectedFacturation)
            {
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la facturation ID: {selectedFacturation.ID_Invoice}?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    // Supprimer toutes les lignes de facturation associées à cette facture
                    var lignesASupprimer = lesLignesFacturation.Where(ligne => ligne.ID_Invoice == selectedFacturation.ID_Invoice).ToList();
                    foreach (var ligne in lignesASupprimer)
                    {
                        bdd.SupprimerLigneFacturation(ligne.ID_Invoice_Line); // Assurez-vous que cette méthode existe et fonctionne correctement
                        lesLignesFacturation.Remove(ligne);
                    }

                    // Supprimer la facture elle-même
                    bdd.SupprimerFacturation(selectedFacturation.ID_Invoice);
                    lesFacturations.Remove(selectedFacturation);

                    DtgFacturations.Items.Refresh();
                    DtgProduitsFacturation.ItemsSource = null;
                    DtgProduitsFacturation.Items.Refresh();

                    ChargerStatistiques(); // Rafraîchir les statistiques si nécessaire
                }
            }
            else
            {
                MessageBox.Show("Merci de sélectionner une ligne avant de cliquer sur Supprimer", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }





        private void DtgFacturations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DtgFacturations.SelectedItem is Facturation selectedFacturation)
            {
                // Mise à jour des champs de détails de la facturation
                TxtFacturationID.Text = selectedFacturation.ID_Invoice.ToString();
                TxtIdFacturation.Text = selectedFacturation.ID_Invoice.ToString();
                CboFacturationClientID.Text = selectedFacturation.ID_Client.ToString();
                DtpFacturationDate.SelectedDate = selectedFacturation.Date_Invoice;
                TxtFacturationTotal.Text = selectedFacturation.Total.ToString();
                CboFacturationPaymentStatus.Text = selectedFacturation.Payment_Status;

                // Mise à jour des lignes de produits de la facturation
                var lignesFacturationFiltrees = lesLignesFacturation
                    .Where(ligne => ligne.ID_Invoice == selectedFacturation.ID_Invoice)
                    .ToList();
                DtgProduitsFacturation.ItemsSource = lignesFacturationFiltrees;
                DtgProduitsFacturation.Items.Refresh();
            }
            else
            {
                // Réinitialisation des champs de détails de la facturation
                SetNextInvoiceID();
                TxtIdFacturation.Text = "";
                CboFacturationClientID.Text = "";
                DtpFacturationDate.SelectedDate = null;
                TxtFacturationTotal.Text = "";
                CboFacturationPaymentStatus.Text = "";

                // Réinitialisation du DataGrid des produits de la facturation
                DtgProduitsFacturation.ItemsSource = null;
            }
        }

        private void DtgProduitsFacturation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DtgProduitsFacturation.SelectedItem is FacturationLigne selectedLigneFacturation)
            {
                // Mise à jour des champs avec les informations de la ligne de facturation sélectionnée
                TxtIdLigneFacturation.Text = selectedLigneFacturation.ID_Invoice_Line.ToString();
                TxtIdFacturation.Text = selectedLigneFacturation.ID_Invoice.ToString();

                // Trouver le produit correspondant à l'ID du produit dans la ligne de facturation
                Produit produitSelectionne = lesProduits.FirstOrDefault(p => p.ID_Product == selectedLigneFacturation.ID_Product);
                if (produitSelectionne != null)
                {
                    CboProduits.SelectedValue = produitSelectionne.ID_Product;
                    TxtNomProduit.Text = produitSelectionne.Product_Name;
                    TxtDescription.Text = produitSelectionne.Description;
                    TxtQuantite.Text = selectedLigneFacturation.Quantity.ToString();
                    // Calcul du sous-total basé sur la quantité et le prix unitaire
                    decimal sousTotal = produitSelectionne.Price * selectedLigneFacturation.Quantity;
                    TxtPrix.Text = sousTotal.ToString("F2");
                }
            }
            else
            {
                // Réinitialisation des champs si aucune ligne n'est sélectionnée
                TxtIdLigneFacturation.Text = "";
                TxtIdFacturation.Text = "";
                CboProduits.SelectedIndex = -1;
                TxtNomProduit.Text = "";
                TxtDescription.Text = "";
                TxtQuantite.Text = "";
                TxtPrix.Text = "";
            }
        }




        private void BtnAjouterProduitFacture_Click(object sender, RoutedEventArgs e)
        {
            if (CboProduits.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un produit.");
                return;
            }

            if (!int.TryParse(TxtQuantite.Text, out int quantite) || quantite <= 0)
            {
                MessageBox.Show("Veuillez entrer une quantité valide.");
                return;
            }

            string selectedItem = CboProduits.SelectedItem.ToString();
            if (int.TryParse(selectedItem.Split('-')[0].Trim(), out int idProduit))
            {
                Produit produitSelectionne = bdd.GetProduitByID(idProduit);
                if (produitSelectionne != null)
                {
                    decimal sousTotal = produitSelectionne.Price * quantite;
                    FacturationLigne nouvelleLigne = new FacturationLigne(
                        bdd.GetNextInvoiceLineID(),
                        Convert.ToInt32(TxtIdFacturation.Text),
                        idProduit,
                        quantite,
                        sousTotal);

                    bdd.AjouterLigneFacturation(nouvelleLigne);
                    lesLignesFacturation.Add(nouvelleLigne);

                    // Mettre à jour le total de la facture
                    if (DtgFacturations.SelectedItem is Facturation selectedFacturation)
                    {
                        selectedFacturation.Total += sousTotal;
                        TxtFacturationTotal.Text = selectedFacturation.Total.ToString("F2");
                        bdd.MettreAJourFacturation(selectedFacturation); // Mettre à jour la facturation dans la base de données
                    }

                    // Réaffecter et rafraîchir les DataGrids
                    DtgProduitsFacturation.ItemsSource = lesLignesFacturation
                        .Where(ligne => ligne.ID_Invoice == Convert.ToInt32(TxtIdFacturation.Text))
                        .ToList();
                    DtgProduitsFacturation.Items.Refresh();
                    DtgFacturations.Items.Refresh();
                    ChargerStatistiques();
                    ClearProductFields();
                }
                else
                {
                    MessageBox.Show("Produit non trouvé.");
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de la récupération de l'ID du produit.");
            }
        }




        private void BtnSupprimerProduitFacture_Click(object sender, RoutedEventArgs e)
        {
            if (DtgProduitsFacturation.SelectedItem is FacturationLigne ligneSelectionnee)
            {
                MessageBoxResult result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la ligne de facturation ID: {ligneSelectionnee.ID_Invoice_Line}?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    // Diminuer le total de la facture
                    if (DtgFacturations.SelectedItem is Facturation selectedFacturation)
                    {
                        // Soustraire le sous-total de la ligne sélectionnée du total de la facture
                        selectedFacturation.Total -= ligneSelectionnee.Subtotal;
                        TxtFacturationTotal.Text = selectedFacturation.Total.ToString("F2");

                        // Mettre à jour la facturation dans la base de données
                        bdd.MettreAJourFacturation(selectedFacturation);
                    }

                    // Supprimer la ligne de facturation et mettre à jour les collections et les affichages
                    bdd.SupprimerLigneFacturation(ligneSelectionnee.ID_Invoice_Line);
                    lesLignesFacturation.Remove(ligneSelectionnee);

                    // Réaffecter et rafraîchir les DataGrids
                    DtgProduitsFacturation.ItemsSource = lesLignesFacturation
                        .Where(ligne => ligne.ID_Invoice == Convert.ToInt32(TxtIdFacturation.Text))
                        .ToList();
                    DtgProduitsFacturation.Items.Refresh();
                    ChargerStatistiques();
                    ClearProductFields();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ligne de produit à supprimer.", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }








        private void SetNextInvoiceLineID()
        {
            int nextID = bdd.GetNextInvoiceLineID();
            TxtIdLigneFacturation.Text = nextID.ToString();
        }
        private void ClearInvoiceLineFields()
        {
            TxtIdLigneFacturation.Text = "";
            TxtIdFacturation.Text = "";
            CboProduits.SelectedIndex = -1; // Réinitialiser la ComboBox des produits
            TxtNomProduit.Text = "";
            TxtDescription.Text = "";
            TxtQuantite.Text = "";
            TxtPrix.Text = "";

            // Mettre à jour le prochain ID de ligne de facturation, si nécessaire
            SetNextInvoiceLineID();
        }



        /*  private void BtnAjouterLigneFacturation_Click(object sender, RoutedEventArgs e)
          {
              if (!string.IsNullOrWhiteSpace(TxtQuantite.Text) &&
                  CboProduits.SelectedItem != null &&
                  !string.IsNullOrWhiteSpace(TxtPrix.Text)) // Assurez-vous que ce champ existe dans votre UI
              {
                  int idLigneFacturation = bdd.GetNextInvoiceLineID();
                  int idProduit = Convert.ToInt32(CboProduits.SelectedValue); // Assurez-vous que cela correspond à votre implémentation
                  int quantite = Convert.ToInt32(TxtQuantite.Text);

                  // Assurez-vous que vous pouvez convertir TxtPrix.Text en un decimal
                  if (decimal.TryParse(TxtPrix.Text, out decimal prixUnitaire))
                  {
                      decimal subtotal = prixUnitaire * quantite; // Calculez le sous-total


                      // Mise à jour de l'interface utilisateur
                      ClearInvoiceLineFields();
                  }
                  else
                  {
                      MessageBox.Show("Le prix doit être un nombre valide.");
                  }
              }
              else
              {
                  MessageBox.Show("Veuillez remplir tous les champs nécessaires.");
              }
          }*/







        private void CboProduits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboProduits.SelectedItem != null)
            {
                try
                {
                    string selectedText = CboProduits.SelectedItem.ToString();
                    int selectedID = int.Parse(selectedText.Split('-')[0].Trim());
                    Produit selectedProduct = bdd.GetProduitByID(selectedID);

                    if (selectedProduct != null)
                    {
                        TxtNomProduit.Text = selectedProduct.Product_Name;
                        TxtDescription.Text = selectedProduct.Description;
                        TxtQuantite.Text = "1";
                        TxtPrix.Text = selectedProduct.Price.ToString("F2");
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Erreur de format : " + ex.Message);
                }
            }
        }


        // Ajoute un produit à la facture



        /*private void BtnAjouterProduitFacturation_Click(object sender, RoutedEventArgs e)
        {
            if (DtgFacturations.SelectedItem is Facturation facturationSelectionnee
                && CboProduits.SelectedItem is Produit produitSelectionne
                && int.TryParse(TxtQuantite.Text, out int quantite))
            {
                decimal subtotal = produitSelectionne.Price * quantite;
                FacturationLigne nouvelleLigne = new FacturationLigne(
                    bdd.GetNextInvoiceLineID(), // Assurez-vous que cette méthode existe dans votre classe BDD
                    facturationSelectionnee.ID_Invoice,
                    produitSelectionne.ID_Product,
                    quantite,
                    subtotal);

                // Ajouter la ligne à la facturation et mettre à jour la base de données
                facturationSelectionnee.AjouterLigneFacturation(nouvelleLigne);
                bdd.AjouterLigneFacturationDB(nouvelleLigne); // Cette méthode doit exister dans votre classe BDD

                // Mettre à jour les DataGrids et autres éléments de l'UI
                // ...
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un produit et entrer une quantité valide.");
            }
        }
        private void BtnSupprimerProduit_Click(object sender, RoutedEventArgs e)
        {
            if (DtgProduitsFacturation.SelectedItem is FacturationLigne ligneSelectionnee
                && DtgFacturations.SelectedItem is Facturation facturationSelectionnee)
            {
                facturationSelectionnee.SupprimerLigneFacturation(ligneSelectionnee.ID_Invoice_Line);
                bdd.SupprimerLigneFacturationDB(ligneSelectionnee.ID_Invoice_Line); // Cette méthode doit exister dans votre classe BDD

                // Mettre à jour les DataGrids et autres éléments de l'UI
                // ...
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ligne de produit à supprimer.");
            }
        }
        */

        #endregion

        #region Commerciaux

        private void SetNextCommercialID()
        {
            int nextID = bdd.GetNextCommercialID();
            TxtCommercialID.Text = nextID.ToString();
        }
        private bool IsPasswordComplex(string password)
        {
            return password.Length >= 12 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }


        private void DtgCommerciaux_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                TxtCommercialID.Text = "";
                TxtCommercialLastname.Text = "";
                TxtCommercialFirstname.Text = "";
                TxtCommercialEmail.Text = "";
                PbxCommercialPassword.Password = "";
                DtgCommerciaux.SelectedItem = null;
            }
        }

        private void BtnAddCommercial_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtCommercialID.Text) &&
                !string.IsNullOrWhiteSpace(TxtCommercialLastname.Text) &&
                !string.IsNullOrWhiteSpace(TxtCommercialFirstname.Text) &&
                !string.IsNullOrWhiteSpace(TxtCommercialEmail.Text) &&
                !string.IsNullOrWhiteSpace(PbxCommercialPassword.Password))
            {
                if (DtgCommerciaux.SelectedItem == null) // Vérifiez si un commercial est sélectionné
                {
                    if (!IsPasswordComplex(PbxCommercialPassword.Password))
                    {
                        MessageBox.Show("Le mot de passe doit contenir au moins 12 caractères, dont une majuscule, un chiffre et un caractère spécial.", "Mot de passe non conforme", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Commercial unCommercial = new Commercial(
                        Convert.ToInt32(TxtCommercialID.Text),
                        TxtCommercialLastname.Text,
                        TxtCommercialFirstname.Text,
                        TxtCommercialEmail.Text,
                        HashPassword(PbxCommercialPassword.Password) // Hacher le mot de passe
                    );

                    bdd.AjouterCommercial(unCommercial); // Ajoutez cette ligne pour enregistrer dans la base de données
                    lesCommerciaux.Add(unCommercial);
                    DtgCommerciaux.Items.Refresh();

                    ClearCommercialFields(); // Efface les champs après l'ajout
                    SetNextCommercialID(); // Mettez à jour le prochain ID commercial disponible
                }
                else
                {
                    MessageBox.Show("Un commercial est déjà sélectionné. Veuillez le mettre à jour ou en sélectionner un autre.", "Opération invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Merci de remplir tous les champs avant d'ajouter un commercial", "Information manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnUpdateCommercial_Click(object sender, RoutedEventArgs e)
        {
            if (DtgCommerciaux.SelectedItem is Commercial selectedCommercial)
            {
                if (!IsPasswordComplex(PbxCommercialPassword.Password))
                {
                    MessageBox.Show("Le mot de passe doit contenir au moins 12 caractères, dont une majuscule, un chiffre et un caractère spécial.", "Mot de passe non conforme", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int index = lesCommerciaux.IndexOf(selectedCommercial);

                selectedCommercial.Lastname = TxtCommercialLastname.Text;
                selectedCommercial.Firstname = TxtCommercialFirstname.Text;
                selectedCommercial.Email = TxtCommercialEmail.Text;
                selectedCommercial.Password = HashPassword(PbxCommercialPassword.Password); // Hacher le mot de passe
                bdd.MettreAJourCommercial(selectedCommercial); // Ajoutez cette ligne pour mettre à jour dans la base de données
                DtgCommerciaux.Items.Refresh();
                ClearCommercialFields();
            }
        }



        private void BtnDeleteCommercial_Click(object sender, RoutedEventArgs e)
        {
            if (DtgCommerciaux.SelectedItem is Commercial selectedCommercial)
            {
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le commercial ID: {selectedCommercial.ID_Commercial}?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bdd.SupprimerCommercial(selectedCommercial.ID_Commercial);
                    lesCommerciaux.Remove(selectedCommercial);
                    DtgCommerciaux.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Merci de sélectionner une ligne avant de cliquer sur Supprimer", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DtgCommerciaux_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DtgCommerciaux.SelectedItem is Commercial selectedCommercial)
            {
                TxtCommercialID.Text = selectedCommercial.ID_Commercial.ToString();
                TxtCommercialLastname.Text = selectedCommercial.Lastname;
                TxtCommercialFirstname.Text = selectedCommercial.Firstname;
                TxtCommercialEmail.Text = selectedCommercial.Email;
                PbxCommercialPassword.Password = selectedCommercial.Password;
            }
            else
            {
                SetNextCommercialID();
                TxtCommercialLastname.Text = "";
                TxtCommercialFirstname.Text = "";
                TxtCommercialEmail.Text = "";
                PbxCommercialPassword.Password = "";
            }
        }
        #endregion

        #region stat
        private void ChargerStatistiques()
        {
            ChargerNombreTotalClients();
            ChargerNombreTotalProspects();
            ChargerTopCinqProduitsVendus();
            ChargerMoyenneMontantsFacturation();
            ChargerClientsGenerantLePlusDeRevenus();
            ChargerRendezVousParCommercial();
            ChargerGraphiqueRepartitionClientsParVille();
            ChargerGraphiqueRepartitionClientsParPays();
            ChargerGraphiqueChiffreAffaires();
            ChargerRepartitionProspectsParVille();
            ChargerRepartitionProspectsParPays();
            ChargerGraphiqueTopClients();
            ChargerGraphiqueBottomClients();
        }

        private void ChargerNombreTotalClients()
        {
            int totalClients = bdd.NombreTotalClients();
            TxtNombreTotalClients.Text = totalClients.ToString();
        }

        private void ChargerGraphiqueChiffreAffaires()
        {
            decimal totalPaye = bdd.ChiffreAffairesParStatut("Payé");
            decimal totalEnAttente = bdd.ChiffreAffairesParStatut("En attente de paiement");

            // Création de la série pour le graphique en camembert
            SeriesCollection series = new SeriesCollection
    {
        new PieSeries
        {
            Title = "Payé",
            Values = new ChartValues<decimal> { totalPaye },
            DataLabels = true,
            LabelPoint = chartPoint => string.Format("{0:C2}", chartPoint.Y)
        },
        new PieSeries
        {
            Title = "En attente de paiement",
            Values = new ChartValues<decimal> { totalEnAttente },
            DataLabels = true,
            LabelPoint = chartPoint => string.Format("{0:C2}", chartPoint.Y)
        }
    };

            PieChartChiffreAffaires2023.Series = series;
        }


        private void ChargerGraphiqueRepartitionClientsParVille()
        {
            var repartition = bdd.RepartitionClientsParVille();

            var seriesCollection = new SeriesCollection();

            foreach (var ville in repartition)
            {
                seriesCollection.Add(new ColumnSeries
                {
                    Title = ville.Key, // Nom de la ville
                    Values = new ChartValues<int> { ville.Value } // Nombre de clients dans cette ville
                });
            }

            ChartRepartitionClientsParVille.Series = seriesCollection;

            // Configurer les axes, si nécessaire
            ChartRepartitionClientsParVille.AxisX.Clear();
            ChartRepartitionClientsParVille.AxisX.Add(new Axis { Title = "Villes" });
            ChartRepartitionClientsParVille.AxisY.Clear();
            ChartRepartitionClientsParVille.AxisY.Add(new Axis { Title = "Nombre de Clients", LabelFormatter = value => value.ToString("N0") });
        }

        private void ChargerGraphiqueRepartitionClientsParPays()
        {
            var repartitionPays = bdd.RepartitionClientsParPays();

            var seriesCollection = new SeriesCollection();

            foreach (var pays in repartitionPays)
            {
                seriesCollection.Add(new ColumnSeries
                {
                    Title = pays.Key, // Nom du pays
                    Values = new ChartValues<int> { pays.Value } // Nombre de clients dans ce pays
                });
            }

            ChartRepartitionClientsParPays.Series = seriesCollection;

            // Configurer les axes, si nécessaire
            ChartRepartitionClientsParPays.AxisX.Clear();
            ChartRepartitionClientsParPays.AxisX.Add(new Axis { Title = "Pays" });
            ChartRepartitionClientsParPays.AxisY.Clear();
            ChartRepartitionClientsParPays.AxisY.Add(new Axis { Title = "Nombre de Clients", LabelFormatter = value => value.ToString("N0") });
        }

        private void ChargerGraphiqueTopClients()
        {
            var topClients = bdd.GetTopClientsByPurchases();

            var seriesCollection = new SeriesCollection();

            foreach (var client in topClients)
            {
                seriesCollection.Add(new ColumnSeries
                {
                    Title = client.Key, // Nom du client
                    Values = new ChartValues<decimal> { client.Value } // Total des achats
                });
            }

            ChartTopClients.Series = seriesCollection;

            // Configurer les axes, si nécessaire
            ChartTopClients.AxisX.Clear();
            ChartTopClients.AxisX.Add(new Axis { Title = "Clients" });
            ChartTopClients.AxisY.Clear();
            ChartTopClients.AxisY.Add(new Axis { Title = "Total des Achats", LabelFormatter = value => value.ToString("C") });
        }

        private void ChargerGraphiqueBottomClients()
        {
            var bottomClients = bdd.GetBottomClientsByPurchases();  // Assurez-vous que cette méthode est correctement définie pour récupérer les 5 plus bas totaux

            var seriesCollection = new SeriesCollection();

            foreach (var client in bottomClients)
            {
                seriesCollection.Add(new ColumnSeries
                {
                    Title = client.Key,  // Nom du client
                    Values = new ChartValues<decimal> { client.Value }  // Total des achats
                });
            }

            ChartBottomClients.Series = seriesCollection;  // Assurez-vous que le nom du graphique correspond

            // Configurer les axes, si nécessaire
            ChartBottomClients.AxisX.Clear();
            ChartBottomClients.AxisX.Add(new Axis { Title = "Clients" });
            ChartBottomClients.AxisY.Clear();
            ChartBottomClients.AxisY.Add(new Axis { Title = "Total des Achats", LabelFormatter = value => value.ToString("C") });
        }




        private void ChargerNombreTotalProspects()
        {
            int totalProspects = bdd.NombreTotalProspects();
            // Remplacez TxtNombreTotalProspects par le nom de votre contrôle dans l'interface utilisateur
            TxtNombreTotalProspects.Text = totalProspects.ToString();
        }

        private void ChargerRepartitionProspectsParVille()
        {
            var repartition = bdd.RepartitionProspectsParVille();
            var seriesCollection = new SeriesCollection();

            foreach (var item in repartition)
            {
                seriesCollection.Add(new ColumnSeries
                {
                    Title = item.Key,
                    Values = new ChartValues<int> { item.Value }
                });
            }

            ChartRepartitionProspectsParVille.Series = seriesCollection;
        }

        private void ChargerRepartitionProspectsParPays()
        {
            var stats = bdd.RepartitionProspectsParPays();
            var seriesCollection = new SeriesCollection();

            foreach (var stat in stats)
            {
                seriesCollection.Add(new ColumnSeries
                {
                    Title = stat.Key,
                    Values = new ChartValues<int> { stat.Value }
                });
            }

            ChartRepartitionProspectsParPays.Series = seriesCollection;
        }

        private void ChargerTopCinqProduitsVendus()
        {
            var topProduits = bdd.TopCinqProduitsVendus();
            LstTopCinqProduitsVendus.Items.Clear();
            foreach (var produit in topProduits)
            {
                LstTopCinqProduitsVendus.Items.Add($"{produit.Key} : {produit.Value}");
            }
        }
        private void ChargerMoyenneMontantsFacturation()
        {
            decimal moyenne = bdd.MoyenneMontantsFacturation();
            TxtMoyenneMontantsFacturation.Text = moyenne.ToString("C2");
        }

        private void ChargerClientsGenerantLePlusDeRevenus()
        {
            var clientsRevenus = bdd.ClientsGenerantLePlusDeRevenus();
            LstClientsRevenus.Items.Clear();
            foreach (var client in clientsRevenus)
            {
                LstClientsRevenus.Items.Add($"{client.Key} : {client.Value.ToString("C2")}");
            }
        }
        private void ChargerRendezVousParCommercial()
        {
            var rendezVousCount = bdd.RendezVousParCommercial();
            LstRendezVousParCommercial2023.Items.Clear();
            foreach (var item in rendezVousCount)
            {
                LstRendezVousParCommercial2023.Items.Add($"{item.Key} : {item.Value} rendez-vous");
            }
        }


        private void ExporterStatistiquesEnPDF(object sender, RoutedEventArgs e)
        {
            // Chemin 
            string filePath = @"C:\Users\Alban\Downloads\Statistiques.pdf";

            // Création du document PDF
            Document doc = new Document();
            try
            {
                // Créer un écrivain PDF qui écoute le document
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

                // Ouvrir le document pour l'écriture
                doc.Open();

                // Ajouter des informations de l'onglet statistiques au document PDF
                doc.Add(new iTextSharp.text.Paragraph("Statistiques CRM"));
                doc.Add(new iTextSharp.text.Paragraph(" ")); // Ligne vide pour l'espacement

                // Informations générales
                doc.Add(new iTextSharp.text.Paragraph("Nombre total de clients : " + TxtNombreTotalClients.Text));
                doc.Add(new iTextSharp.text.Paragraph("Nombre total de prospects : " + TxtNombreTotalProspects.Text));
                doc.Add(new iTextSharp.text.Paragraph("Moyenne des montants de facturation : " + TxtMoyenneMontantsFacturation.Text));
                doc.Add(new iTextSharp.text.Paragraph(" "));


                // Top 5 des produits les plus vendus
                doc.Add(new iTextSharp.text.Paragraph("Top 5 des produits les plus vendus :"));
                foreach (var item in LstTopCinqProduitsVendus.Items)
                {
                    doc.Add(new iTextSharp.text.Paragraph("- " + item.ToString()));
                }
                doc.Add(new iTextSharp.text.Paragraph(" "));

                // Top 3 des clients générant le plus de revenus
                doc.Add(new iTextSharp.text.Paragraph("Top 3 des clients générant le plus de revenus :"));
                foreach (var item in LstClientsRevenus.Items)
                {
                    doc.Add(new iTextSharp.text.Paragraph("- " + item.ToString()));
                }
                doc.Add(new iTextSharp.text.Paragraph(" "));

                // Rendez-vous par Commercial 2023
                doc.Add(new iTextSharp.text.Paragraph("Rendez-vous par Commercial 2023 :"));
                foreach (var item in LstRendezVousParCommercial2023.Items)
                {
                    doc.Add(new iTextSharp.text.Paragraph("- " + item.ToString()));
                }
                doc.Add(new iTextSharp.text.Paragraph(" "));

                // Fermer le document
                doc.Close();

                MessageBox.Show("Statistiques exportées en PDF avec succès dans votre dossier téléchargement.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'exportation en PDF : " + ex.Message);
            }
        }



                


        #endregion


 }
}
