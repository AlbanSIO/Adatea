using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adatea.Classe
{
    public class FacturationLigne
    {
        private int id_Invoice_Line;
        private int id_Invoice;
        private int id_Product;
        private int quantity;
        private decimal subtotal;

        public int ID_Invoice_Line
        {
            get { return id_Invoice_Line; }
            set { id_Invoice_Line = value; }
        }

        public int ID_Invoice
        {
            get { return id_Invoice; }
            set { id_Invoice = value; }
        }

        public int ID_Product
        {
            get { return id_Product; }
            set { id_Product = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public decimal Subtotal
        {
            get { return subtotal; }
            set { subtotal = value; }
        }

        // Constructeur
        public FacturationLigne(int idInvoiceLine, int idInvoice, int idProduct, int quantity, decimal subtotal)
        {
            this.id_Invoice_Line = idInvoiceLine;
            this.id_Invoice = idInvoice;
            this.id_Product = idProduct;
            this.quantity = quantity;
            this.subtotal = subtotal;
        }

        public override string ToString()
        {
            return "Ligne Facturation " + id_Invoice_Line + " - " + subtotal.ToString("C2");
        }
    }
}
