using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace projeto
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Redimensionador");

            Thread thread = new Thread(Redimensionar);
            thread.Start();
        }

        static void Redimensionar()
        {
            string diretorio_entrada = "arquivo_entrada";
            string diretorio_redimensionado = "arquivo_redimensionado";
            string diretorio_finalizado = "arquivo_finalizado";

            if (!Directory.Exists(diretorio_entrada))
            {
                Directory.CreateDirectory(diretorio_entrada);
            }

            if (!Directory.Exists(diretorio_finalizado))
            {
                Directory.CreateDirectory(diretorio_finalizado);
            }

            if (!Directory.Exists(diretorio_redimensionado))
            {
                Directory.CreateDirectory(diretorio_redimensionado);
            }

            while (true)
            {
                var arquivosEntrada = Directory.EnumerateFiles(diretorio_entrada);

                int novaAltura = 200;

                foreach (var arquivo in arquivosEntrada)
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(arquivo, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            FileInfo fileInfo = new FileInfo(arquivo);

                            string caminhoRedimensionado = Path.Combine(diretorio_redimensionado, $"{DateTime.Now.Ticks}_{fileInfo.Name}");

                            Redimensionador(Image.FromStream(fileStream), novaAltura, caminhoRedimensionado);
                        }

                        string caminhoFinalizado = Path.Combine(diretorio_finalizado, Path.GetFileName(arquivo));

                        File.Move(arquivo, caminhoFinalizado);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar arquivo: {ex.Message}");
                    }
                }

                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
        }

        static void Redimensionador(Image imagem, int altura, string caminho)
        {
            double ratio = (double)altura / imagem.Height;
            int novaLargura = (int)(imagem.Width * ratio);
            int novaAltura = (int)(imagem.Height * ratio);

            using (Bitmap novaImage = new Bitmap(novaLargura, novaAltura))
            {
                using (Graphics g = Graphics.FromImage(novaImage))
                {
                    g.DrawImage(imagem, 0, 0, novaLargura, novaAltura);
                }

                novaImage.Save(caminho);
            }
        }
    }
}
