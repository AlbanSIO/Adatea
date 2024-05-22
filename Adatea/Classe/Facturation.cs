using System;
using System.Collections.Generic;

namespace Adatea.Classe
{
    public class Facturation
    {
        private int id_Invoice;
        private int id_Client;
        private DateTime date_Invoice;
        private decimal total;
        private string payment_Status;
        private List<FacturationLigne> lignesFacturation;

        public int ID_Invoice
        {
            get { return id_Invoice; }
            set { id_Invoice = value; }
        }

        public int ID_Client
        {
            get { return id_Client; }
            set { id_Client = value; }
        }

        public DateTime Date_Invoice
        {
            get { return date_Invoice; }
            set { date_Invoice = value; }
        }

        public decimal Total
        {
            get { return total; }
            set { total = value; }
        }

        public string Payment_Status
        {
            get { return payment_Status; }
            set { payment_Status = value; }
        }

        public List<FacturationLigne> LignesFacturation
        {
            get { return lignesFacturation; }
            set { lignesFacturation = value; }
        }

        // Constructeur
        public Facturation(int idInvoice, int idClient, DateTime dateInvoice, decimal totalAmount, string paymentStatus)
        {
            this.id_Invoice = idInvoice;
            this.id_Client = idClient;
            this.date_Invoice = dateInvoice;
            this.total = totalAmount;
            this.payment_Status = paymentStatus;
            this.lignesFacturation = new List<FacturationLigne>();
        }

        public void AjouterLigneFacturation(FacturationLigne ligne)
        {
            lignesFacturation.Add(ligne);
            // Mettre à jour le total ici si nécessaire
            // total += ligne.Subtotal;
        }

        public override string ToString()
        {
            return "Invoice " + id_Invoice + " - " + total.ToString("C2"); // Format en tant que monnaie pour la visualisation
        }
    }
}