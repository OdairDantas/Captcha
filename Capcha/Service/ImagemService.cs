using AForge.Imaging.Filters;
using Capcha.Service.Interface;
using System;
using System.Drawing;
using System.IO;
using Tesseract;

namespace Capcha.Service
{
    public class ImagemService : IImagemServie
    {
        public string ExtrairTexto(string base64)
        {
            try
            {
                var imagem = LimparImagem(Base64ToImage(base64));
                var imagemName = String.Concat(Guid.NewGuid().ToString(), ".bmp");
                var path = Path.Combine("App_Data", imagemName);
                imagem.Save(path);
                var reconhecido = OCR(path);
                File.Delete(imagemName);
                return reconhecido;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private Bitmap LimparImagem(Image img)
        {
            var imagem = new Bitmap(img);

            imagem = imagem.Clone(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var inverter = new Invert();

            var cor = new ColorFiltering
            {
                Blue = new AForge.IntRange(200, 255),
                Red = new AForge.IntRange(200, 255),
                Green = new AForge.IntRange(200, 255)
            };

            var open = new Opening();
            var bc = new BlobsFiltering();
            var gs = new GaussianSharpen();
            var cc = new ContrastCorrection();
            bc.MinHeight = 10;

            var seq = new FiltersSequence(gs, inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter);
            imagem = seq.Apply(imagem);
            return imagem;
        }

        private Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
        private string OCR(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
                {
                    engine.SetVariable("tessedit_char_whitelist", "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
                    engine.SetVariable("tessedit_unrej_any_wd", true);

                    using var img = Pix.LoadFromFile(imagePath);
                    using var page = engine.Process(img);
                    return page.GetText().Trim().Replace(@"\r\n", "");

                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
