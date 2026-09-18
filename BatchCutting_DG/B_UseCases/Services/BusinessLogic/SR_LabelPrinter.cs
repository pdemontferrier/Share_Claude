using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using Net.Codecrete.QrCodeGenerator;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchCutting_DG.B_UseCases.Services.BusinessLogic
{
    public class SR_LabelPrinter : IS_LabelPrinter
    {
        private readonly IS_Notification _notification;
        private readonly IS_Settings _settings;
        private readonly IS_Dictionary _dictionary;
        private readonly IS_Icons _icons;
        private string PrinterName;


        public SR_LabelPrinter(IS_Notification notification, IS_Settings settings,
                                    IS_Dictionary dictionary, IS_Icons icons)
        {
            _notification = notification;
            _settings = settings;
            _dictionary = dictionary;
            _icons = icons;

            PrinterName = _settings.GetLabelPrinter();
        }

        private Bitmap GenerateQrBitmap(string value, int targetSize)
        {
            var qr = QrCode.EncodeText(value, QrCode.Ecc.High);
            byte[] qrBytes = qr.ToBmpBitmap(scale: 2, border: 1);
            using var ms = new MemoryStream(qrBytes);
            using var rawImage = new Bitmap(ms);

            Bitmap qrImage = new Bitmap(targetSize, targetSize);
            using var g = Graphics.FromImage(qrImage);
            g.Clear(Color.White);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.DrawImage(rawImage, new Rectangle(0, 0, targetSize, targetSize));

            return qrImage;
        }

        private Bitmap GetBitmapFromResourceUri(Uri resourceUri)
        {
            var resourceInfo = System.Windows.Application.GetResourceStream(resourceUri);
            if (resourceInfo == null)
                throw new FileNotFoundException($"Ressource not found : {resourceUri}");

            using var stream = resourceInfo.Stream;
            return new Bitmap(stream);
        }

        public bool PrintBarCutLabel(DTO_DecoupeDetailWithCut decoupeDetailWithCut)
        {
            var printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = PrinterName;

            printDoc.PrintPage += (sender, e) =>
            {
                // Police et styles
                var fontName = "Arial";
                var fontSize = 9;
                var fontColor = Brushes.Black;
                var fontTitre = new Font(fontName, fontSize, FontStyle.Regular);
                var fontTexte = new Font(fontName, fontSize, FontStyle.Bold);
                var fontInfo = new Font(fontName, fontSize - 1, FontStyle.Regular);

                // Coordonnées
                float x = 15;           // pixels du bord gauche
                float y = 15;           // pixels du bord haut
                float offsetx = 50;    // Largueur colone
                float offsety = 20;     // Hauteur de ligne

                // Impression
                // 1ère colonne
                var columnTitle = 0;
                var columnData = 1;
                var line = 0;
                e.Graphics.DrawString(decoupeDetailWithCut.DecoupeLotDesignation.ToString(), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));

                line = line + 1;
                e.Graphics.DrawString(decoupeDetailWithCut.NomProjet.ToString(), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));

                line = line + 1;
                e.Graphics.DrawString(decoupeDetailWithCut.NumProjet.ToString() + " - " +
                                    decoupeDetailWithCut.Structure.ToString() + " - " +
                                    decoupeDetailWithCut.Position.ToString(), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));

                line = line + 1;
                e.Graphics.DrawString(_dictionary.GetText("I01_01"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(decoupeDetailWithCut.Reference.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData) + 30, y + (offsety * line)));

                line = line + 1;
                e.Graphics.DrawString(_dictionary.GetText("I01_02"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(decoupeDetailWithCut.Couleur.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData) + 30, y + (offsety * line)));


                line = line + 1;
                columnTitle = 0;
                columnData = 0;
                e.Graphics.DrawString(_dictionary.GetText("I01_07"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(decoupeDetailWithCut.Inclinaison1.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData), y + (offsety * (line + 1))));
                columnTitle = 1;
                columnData = 1;
                e.Graphics.DrawString(_dictionary.GetText("I01_08"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(decoupeDetailWithCut.Pivot1.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData), y + (offsety * (line + 1))));
                columnTitle = 2;
                columnData = 2;
                e.Graphics.DrawString(_dictionary.GetText("I01_03"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(decoupeDetailWithCut.LongueurDecoupe.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData), y + (offsety * (line + 1))));
                columnTitle = 3;
                columnData = 3;
                e.Graphics.DrawString(_dictionary.GetText("I01_09"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle) + 15, y + (offsety * line)));
                e.Graphics.DrawString(decoupeDetailWithCut.Pivot2.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData) + 15, y + (offsety * (line + 1))));
                columnTitle = 4;
                columnData = 4;
                e.Graphics.DrawString(_dictionary.GetText("I01_10"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle) + 15, y + (offsety * line)));
                e.Graphics.DrawString(decoupeDetailWithCut.Inclinaison2.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData) + 15, y + (offsety * (line + 1))));

                // Logo depuis ressources intégrées
                try
                {
                    var logoUri = _icons.GetPrint_Logo_WB_Source();
                    var logoBitmap = GetBitmapFromResourceUri(logoUri);

                    int maxWidth = 110;
                    int maxHeight = 35;

                    float ratioX = (float)maxWidth / logoBitmap.Width;
                    float ratioY = (float)maxHeight / logoBitmap.Height;
                    float scale = Math.Min(ratioX, ratioY);

                    int drawWidth = (int)(logoBitmap.Width * scale);
                    int drawHeight = (int)(logoBitmap.Height * scale);

                    float logoX = x + offsetx * 5 + 11 + (maxWidth - drawWidth) / 2f;
                    float logoY = y;

                    var g = e.Graphics;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    g.DrawImage(logoBitmap, new RectangleF(logoX, logoY, drawWidth, drawHeight));
                }
                catch (Exception ex)
                {
                    _notification.Warning("No_Wa_14", ex.Message);
                }

                // QRCode
                string qrCode = "DD-" + decoupeDetailWithCut.NumLigne?.ToString();
                var qrImage = GenerateQrBitmap(qrCode, 90);
                line = 2;
                columnData = 5;
                e.Graphics.DrawImage(qrImage, new PointF(x + (offsetx * columnData) + 20, y + (offsety * line)));
            };

            try
            {
                printDoc.Print();
                return true;
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_16", ex.Message);
                return false;
            }
        }

        public bool PrintBarDropLabel(DTO_DecoupeBarreWithCut decoupeBarreWithCut)
        {
            var printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = PrinterName;

            printDoc.PrintPage += (sender, e) =>
            {
                // Police et styles
                var fontName = "Arial";
                var fontSize = 12;
                var fontColor = Brushes.Black;
                var fontTitre = new Font(fontName, fontSize, FontStyle.Regular);
                var fontTexte = new Font(fontName, fontSize, FontStyle.Bold);

                // Coordonnées
                float x = 15;           // pixels du bord gauche
                float y = 15;           // pixels du bord haut
                float offsetx = 120;    // Largueur colone
                float offsety = 30;     // Hauteur de ligne

                // Impression
                // 1ère colonne
                var columnTitle = 0;
                var columnData = 1;
                var line = 0;
                e.Graphics.DrawString(_dictionary.GetText("I01_01"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(decoupeBarreWithCut.ReferenceArticle.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData), y + (offsety * line)));

                line = line + 1;
                e.Graphics.DrawString(_dictionary.GetText("I01_02"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(decoupeBarreWithCut.CouleurArticle.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData), y + (offsety * line)));

                line = line + 1;
                e.Graphics.DrawString(_dictionary.GetText("I01_03"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(decoupeBarreWithCut.LongueurChuteFinale + " mm", fontTexte, fontColor, new PointF(x + (offsetx * columnData), y + (offsety * line)));

                line = line + 1;
                e.Graphics.DrawString(_dictionary.GetText("I01_04"), fontTitre, fontColor, new PointF(x + (offsetx * columnTitle), y + (offsety * line)));
                e.Graphics.DrawString(_dictionary.GetText("I01_05") + " - " + decoupeBarreWithCut.EmpSc.ToString(), fontTexte, fontColor, new PointF(x + (offsetx * columnData), y + (offsety * line)));

                // 2ème colonne QRCode
                string qrCode = decoupeBarreWithCut.CodeBarreChute ?? string.Empty;
                var qrImage = GenerateQrBitmap(qrCode, 90);
                columnData = 2;
                line = 0;
                e.Graphics.DrawImage(qrImage, new PointF(x + (offsetx * columnData) + 35, y + (offsety * line)));
            };


            try
            {
                printDoc.Print();
                return true;
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_16", ex.Message);
                return false;
            }
        }

        public bool PrintBarWasteLabel()
        {
            var printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = PrinterName;

            printDoc.PrintPage += (sender, e) =>
            {
                // Police et styles
                var fontName = "Arial";
                var fontSize = 30;
                var fontColor = Brushes.Black;
                var fontTexte = new Font(fontName, fontSize, FontStyle.Bold);

                // Coordonnées
                float x = 50;           // pixels du bord gauche
                float y = 50;           // pixels du bord haut

                // Impression
                e.Graphics.DrawString(_dictionary.GetText("I01_06"), fontTexte, fontColor, new PointF(x, y));

            };

            try
            {
                printDoc.Print();
                return true;
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_16", ex.Message);
                return false;
            }
        }
    }
}