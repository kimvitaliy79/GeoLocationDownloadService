using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace UpdateUtility.Services
{
  public class DownloadService
    {       
        const string  zipExt = ".zip";
        public DownloadService()
        {
           
        }       

        public string DownloadFile(string url)
        {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            string filename = "";

            string destinationpath = string.IsNullOrEmpty(ConfigurationService.PathForDownload) ? Environment.CurrentDirectory: ConfigurationService.PathForDownload;
            if (!Directory.Exists(destinationpath))
            {
                Directory.CreateDirectory(destinationpath);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result)
            {
                string path = response.Headers["Content-Disposition"];
                if (string.IsNullOrWhiteSpace(path))
                {
                    var uri = new Uri(url);
                    filename = Path.GetFileName(uri.LocalPath);
                }
                else
                {
                    ContentDisposition contentDisposition = new ContentDisposition(path);
                    filename = contentDisposition.FileName;

                }

                var responseStream = response.GetResponseStream();
                using (var fileStream = File.Create(System.IO.Path.Combine(destinationpath, filename)))
                {
                    responseStream.CopyTo(fileStream);
                }
            }

            string zipPath= Path.Combine(destinationpath, filename);
            string unzipedPath = string.Empty;

            if (filename.Contains(zipExt))
            {
                unzipedPath = filename.Replace(zipExt, string.Empty).Trim();
                unzipedPath= Path.Combine(destinationpath, unzipedPath);
            }

            if (Directory.Exists(unzipedPath))
                Directory.Delete(unzipedPath, true);

            ZipFile.ExtractToDirectory(zipPath, destinationpath); 

            return unzipedPath;
        }
    }
}
