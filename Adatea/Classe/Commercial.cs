using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adatea.Classe
{
    public class Commercial
    {
        private int id_Commercial;
        private string lastname;
        private string firstname;
        private string email;
        private string password;  

        public int ID_Commercial
        {
            get { return id_Commercial; }
            set { id_Commercial = value; }
        }

        public string Lastname
        {
            get { return lastname; }
            set { lastname = value; }
        }

        public string Firstname
        {
            get { return firstname; }
            set { firstname = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        // Constructeur
        public Commercial(int idCommercial, string lastname, string firstname, string email, string password)
        {
            this.id_Commercial = idCommercial;
            this.lastname = lastname;
            this.firstname = firstname;
            this.email = email;
            this.password = password; // Encore une fois, ceci est à traiter avec prudence dans une vraie application.
        }

        public override string ToString()
        {
            return Lastname + " " + Firstname;
        }
    }
}
