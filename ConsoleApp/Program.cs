using ConsoleApp.Infrastructure;
using Rwt.Abstractions.Services;
using System;
using System.Threading;
using Unity;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();
            container.AddRwtServices();

            var service = container.Resolve<IDataImportService>();

            var personIds = new[]
            {
                "26013232910",
                "17074125859",
                "06039229186",
                "12032922838",
                "28113844592",
                "20038518462",
                "02041764778",
                "14120038088",
                "20044930243",
                "25108625433",
                "17102342801",
                "15100777069",
                "23026903629",
                "30106434952",
                "02025920336",
                "17080549261",
                "14034504440"
            };

            foreach(var ssn in personIds)
            {
                Guid personDbId;
                try
                {
                    personDbId = service.ImportPerson(ssn);
                    Console.WriteLine($"Person {ssn} is imported/updated into DB object {personDbId}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with updating person {ssn}: {ex.Message}");
                }
            }

            Console.WriteLine("Process complete.");
            Console.ReadKey();
        }
    }
}
