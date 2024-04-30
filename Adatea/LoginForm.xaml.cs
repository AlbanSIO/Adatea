using Adatea.Classe;
using System;
using System.Windows;
using System.DirectoryServices.AccountManagement;

namespace Adatea
{
    /// <summary>
    /// Logique d'interaction pour LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text;
            string password = TxtPassword.Password;

            // Vérification des identifiants de l'utilisateur contre Active Directory
            if (ValidateCredentials(username, password))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); // Fermez le formulaire de connexion
            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.", "Échec de la connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "yasuo.lan"))
                {
                    // Valide les informations d'identification
                    return pc.ValidateCredentials(username, password);
                }
            }
            catch (Exception ex)
            {
                // Gérer les exceptions
                MessageBox.Show("Erreur lors de la connexion : " + ex.Message);
                return false;
            }
        }

       
    }
}
