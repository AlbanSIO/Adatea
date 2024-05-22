using Adatea.Classe;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Adatea
{
    /// <summary>
    /// Logique d'interaction pour ExportPDF.xaml
    /// </summary>
    public partial class ExportPDF : Window
    {
        BDD bdd = new BDD();


        public ExportPDF()
        {
            InitializeComponent();
            ChargerStatistiques();
        }

        public MemoryStream RenderChartToMemoryStream(Chart chart)
        {
            chart.Measure(chart.RenderSize);
            chart.Arrange(new Rect(new System.Windows.Point(0, 0), chart.RenderSize));
            chart.Update(true, true); //force chart redraw
            chart.UpdateLayout();

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)chart.ActualWidth,
                (int)chart.ActualHeight,
                96d,
                96d,
                System.Windows.Media.PixelFormats.Pbgra32);
            renderBitmap.Render(chart);

            MemoryStream ms = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save(ms);
            ms.Position = 0;
            return ms;
        }

        public void ExportChartToPdf(Chart chart, Document document, string title = "")
        {
            MemoryStream chartStream = RenderChartToMemoryStream(chart);

            iTextSharp.text.Image chartImage = iTextSharp.text.Image.GetInstance(chartStream.ToArray());
            chartImage.ScaleToFit(document.PageSize.Width - 20, document.PageSize.Height - 20);
            chartImage.Alignment = Element.ALIGN_CENTER;

            if (!string.IsNullOrEmpty(title))
            {
                document.Add(new Paragraph(title));
            }
            document.Add(chartImage);
        }

        private void ExporterStatistiquesEnPDF(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Statistiques.pdf");

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();

                doc.Add(new Paragraph("Statistiques CRM"));
                doc.Add(new Paragraph(" "));

                ExportChartToPdf(ChartRepartitionClientsParVille, doc, "Répartition des Clients par Ville");
                ExportChartToPdf(ChartRepartitionClientsParPays, doc, "Répartition des Clients par Pays");
                ExportChartToPdf(ChartTopClients, doc, "Top 5 Clients par Total d'Achats");
                ExportChartToPdf(ChartBottomClients, doc, "5 Clients par Total d'Achats les Plus Bas");

                ExportChartToPdf(ChartRepartitionProspectsParVille, doc, "Répartition des Prospects par Ville");
                ExportChartToPdf(ChartRepartitionProspectsParPays, doc, "Répartition des Prospects par Pays");
                ExportChartToPdf(PieChartChiffreAffaires2023, doc, "Répartition du Chiffre d'Affaires 2023");

                doc.Close();
                // Forcer la mise à jour de la disposition
                Application.Current.Dispatcher.Invoke(() => { this.UpdateLayout(); }, System.Windows.Threading.DispatcherPriority.Render);

                MessageBox.Show("Statistiques exportées en PDF avec succès dans votre dossier téléchargement.");
            }
            catch (Exception ex)
            {
                doc.Close();
                MessageBox.Show("Erreur lors de l'exportation en PDF : " + ex.Message);
            }
        }

        private void ChargerStatistiques()
        {
            ChargerGraphiqueRepartitionClientsParVille();
            ChargerGraphiqueRepartitionClientsParPays();
            ChargerGraphiqueChiffreAffaires();
            ChargerRepartitionProspectsParVille();
            ChargerRepartitionProspectsParPays();
            ChargerGraphiqueTopClients();
            ChargerGraphiqueBottomClients();
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
            ChartRepartitionClientsParVille.AxisX.Add(new LiveCharts.Wpf.Axis { Title = "Villes" });
            ChartRepartitionClientsParVille.AxisY.Clear();
            ChartRepartitionClientsParVille.AxisY.Add(new LiveCharts.Wpf.Axis { Title = "Nombre de Clients", LabelFormatter = value => value.ToString("N0") });
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
            ChartRepartitionClientsParPays.AxisX.Add(new LiveCharts.Wpf.Axis { Title = "Pays" });
            ChartRepartitionClientsParPays.AxisY.Clear();
            ChartRepartitionClientsParPays.AxisY.Add(new LiveCharts.Wpf.Axis { Title = "Nombre de Clients", LabelFormatter = value => value.ToString("N0") });
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
            ChartTopClients.AxisX.Add(new LiveCharts.Wpf.Axis { Title = "Clients" });
            ChartTopClients.AxisY.Clear();
            ChartTopClients.AxisY.Add(new LiveCharts.Wpf.Axis { Title = "Total des Achats", LabelFormatter = value => value.ToString("C") });
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
            ChartBottomClients.AxisX.Add(new LiveCharts.Wpf.Axis { Title = "Clients" });
            ChartBottomClients.AxisY.Clear();
            ChartBottomClients.AxisY.Add(new LiveCharts.Wpf.Axis { Title = "Total des Achats", LabelFormatter = value => value.ToString("C") });
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
    }
}
