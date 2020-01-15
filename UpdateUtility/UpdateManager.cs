using DBApplication.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyCsvParser;
using UpdateUtility.Services;

namespace UpdateUtility
{
   public class UpdateManager
    {       
        const string fileNameCity = "GeoLite2-City-Blocks-IPv4.csv";
        const string fileNameLocations = "GeoLite2-City-Locations-ru.csv";
        public UpdateManager()
        {
          
        }

        public void Start()
        {
            Thread thread = new Thread(DoWork);
            thread.Name = "GeoLocService";
            thread.Start();           
        }

        void DoWork()
        {
            try
            { 
              
                Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss")} Сервис запущен");
                DownloadService service = new DownloadService();
                Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Идёт скачивание файла GeoLite2-City-CSV.zip ");

                if (string.IsNullOrEmpty(ConfigurationService.DownloadFileHref))
                {
                    throw new Exception("Настройте ключ DownloadFileHref в конфиг файле App.config");  
                }
                string filePath = service.DownloadFile(ConfigurationService.DownloadFileHref);
               

                // обновляем только в среду, так как данные на сайте обновляются только во вторник или указана AlwaysUpdate=true в конфиге
                if (DateTime.Now.DayOfWeek== DayOfWeek.Wednesday || ConfigurationService.AlwaysUpdate) 
                {
                    CSVService csvServ = new CSVService();
                    GeoRepository repository = new GeoRepository();

                    string filePathLocations = string.Format($"{filePath}\\{fileNameLocations}");

                    if (!File.Exists(filePathLocations))                    
                        throw new Exception($"Загруженный файл {fileNameLocations} не был найден");
                    
                    Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Идёт считывание файла GeoLite2-City-Locations-ru.csv, подождите... ");
                    var itemsLocations = csvServ.GetDataTabletFromCSVFile(filePathLocations); // загружаем файл Locations
                    Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Запись в базу файла GeoLite2-City-Locations-ru.csv, подождите...");
                    repository.DeleteData("Stp_ClearCityLocationRu");
                    repository.InsertDataUsingSQLBulkCopy(itemsLocations, "CityLocationsRu");

                    string filePathCity = string.Format($"{filePath}\\{fileNameCity}");

                    if (!File.Exists(filePathCity))                    
                        throw new Exception($"Загруженный файл {fileNameCity} не был найден");                       
                    
                    Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Идёт считывание файла GeoLite2-City-Blocks-IPv4.csv, подождите... ");
                    var itemsCity = csvServ.GetDataTabletFromCSVFile(filePathCity); // загружаем файл City
                    Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Запись в базу файла GeoLite2-City-Blocks-IPv4.csv, подождите...");
                    repository.DeleteData("Stp_ClearCityBlocksIPv4");
                    repository.InsertDataUsingSQLBulkCopy(itemsCity, "CityBlocksIPv4");

                    Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Данные в базе были обновлены");
                }
                else
                {
                    Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Данные не требуют обновления");
                }

                Console.WriteLine($"{ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Работа сервиса завершена успешно ");
                Thread.Sleep(new TimeSpan(ConfigurationService.AfterDaysToRunApp,0,0,0)); // настраиваем через какое время запускаться, ключ вынесен в конфиг 
                DoWork();  

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex?.Message} {ex?.InnerException}");
                Console.ReadLine();
            }
        }
    }
}
