using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adatea.Classe
{
    public class Rendezvous
    {
        private int id_Rdv;
        private int? id_Prospect;
        private int? id_Client;
        private int id_Commercial;
        private DateTime date_Rdv;
        private TimeSpan time_Rdv;
        private string location;

        public int ID_Rdv
        {
            get { return id_Rdv; }
            set { id_Rdv = value; }
        }
        public int? ID_Prospect
        {
            get { return id_Prospect; }
            set { id_Prospect = value; }
        }
        public int? ID_Client
        {
            get { return id_Client; }
            set { id_Client = value; }
        }
        public int ID_Commercial
        {
            get { return id_Commercial; }
            set { id_Commercial = value; }
        }
        public DateTime Date_Rdv
        {
            get { return date_Rdv; }
            set { date_Rdv = value; }
        }
        public TimeSpan Time_Rdv
        {
            get { return time_Rdv; }
            set { time_Rdv = value; }
        }
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        // Constructeur
        public Rendezvous(int idRdv, int? idProspect, int? idClient, int idCommercial, DateTime dateRdv, TimeSpan timeRdv, string location)
        {
            this.id_Rdv = idRdv;
            this.id_Prospect = idProspect;
            this.id_Client = idClient;
            this.id_Commercial = idCommercial;
            this.date_Rdv = dateRdv;
            this.time_Rdv = timeRdv;
            this.location = location;
        }


    }
}
