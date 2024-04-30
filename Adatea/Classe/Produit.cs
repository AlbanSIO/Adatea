using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adatea.Classe
{
    public class Produit
    {
        private int id_Product;
        private string product_Name;
        private string description;
        private decimal price;
        private int stock_Quantity;

        public int ID_Product
        {
            get { return id_Product; }
            set { id_Product = value; }
        }

        public string Product_Name
        {
            get { return product_Name; }
            set { product_Name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        public int Stock_Quantity
        {
            get { return stock_Quantity; }
            set { stock_Quantity = value; }
        }

        // Constructeur
        public Produit(int idProduct, string productName, string description, decimal price, int stockQuantity)
        {
            this.id_Product = idProduct;
            this.product_Name = productName;
            this.description = description;
            this.price = price;
            this.stock_Quantity = stockQuantity;
        }

        public override string ToString()
        {
            return product_Name + " - " + price.ToString("C2"); // Format en tant que monnaie pour la visualisation
        }
    }
}


