using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adatea.Classe
{
    public class Prospect
    {
        private int id_Prospect;
        private string lastname;
        private string firstname;
        private string email;
        private string street;
        private string postalCode;
        private string city;
        private string country;
        private int phone_Number;

        public int ID_Prospect
        {
            get { return id_Prospect; }
            set { id_Prospect = value; }
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
        public string Street
        {
            get { return street; }
            set { street = value; }
        }
        public string PostalCode
        {
            get { return postalCode; }
            set { postalCode = value; }
        }
        public string City
        {
            get { return city; }
            set { city = value; }
        }
        public string Country
        {
            get { return country; }
            set { country = value; }
        }
        public int Phone_Number
        {
            get { return phone_Number; }
            set { phone_Number = value; }
        }

        // Constructeur
        public Prospect(int idProspect, string lastname, string firstname, string email, string street, string postalCode, string city, string country, int phoneNumber)
        {
            this.id_Prospect = idProspect;
            this.lastname = lastname;
            this.firstname = firstname;
            this.email = email;
            this.street = street;
            this.postalCode = postalCode;
            this.city = city;
            this.country = country;
            this.phone_Number = phoneNumber;
        }

        public override string ToString()
        {
            return Lastname + " " + Firstname;
        }
    }
}
