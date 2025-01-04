using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Helpers
{
    public class FileService
    {
        private readonly string _serverIp;
        private readonly int _serverPort;

        public FileService()
        {
            _serverIp = "127.0.0.1";
            _serverPort = 5001;
        }

        public List<string> ListFiles()
        {
            var files = new List<string>();

            using (TcpClient client = new TcpClient(_serverIp, _serverPort))
            using (NetworkStream stream = client.GetStream())
            using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
            using (StreamReader reader = new StreamReader(stream))
            {
                writer.WriteLine("LIST");

                string fileName;
                while ((fileName = reader.ReadLine()) != "END")
                {
                    files.Add(fileName);
                }
            }

            return files;
        }

        public bool UploadFile(string filePath, string timestamp = null)
        {
            try
            {
                // Zaman damgası varsa dosya adına ekle
                string originalFileName = Path.GetFileName(filePath);
                string fileName = timestamp != null
                    ? $"{Path.GetFileNameWithoutExtension(originalFileName)}_{timestamp}{Path.GetExtension(originalFileName)}"
                    : originalFileName;

                using (TcpClient client = new TcpClient(_serverIp, _serverPort))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                {
                    writer.WriteLine("UPLOAD");
                    writer.WriteLine(fileName); // Zaman damgalı dosya adı gönderiliyor

                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        fs.CopyTo(stream);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya yükleme hatası: {ex.Message}");
                return false;
            }
        }


        public bool DeleteFile(string fileName)
        {
            try
            {
                using (TcpClient client = new TcpClient(_serverIp, _serverPort))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream))
                {
                    writer.WriteLine("DELETE");
                    writer.WriteLine(fileName);

                    string response = reader.ReadLine();
                    return response == "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya silme hatası: {ex.Message}");
                return false;
            }
        }

        public bool DownloadFile(string fileName, string savePath)
        {
            try
            {
                using (TcpClient client = new TcpClient(_serverIp, _serverPort))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream))
                {
                    // DOWNLOAD komutunu gönder
                    writer.WriteLine("DOWNLOAD");
                    writer.WriteLine(fileName);

                    // Sunucudan yanıt bekle
                    string response = reader.ReadLine();
                    if (response == "OK")
                    {
                        // Gelen dosyayı belirtilen yola kaydet
                        using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                        {
                            stream.CopyTo(fs);
                        }

                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Dosya sunucuda bulunamadı.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya indirme hatası: {ex.Message}");
                return false;
            }
        }
    }
}
