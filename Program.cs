using Npgsql;
using ClosedXML.Excel;

namespace LerPlanilhaExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            var xls = new XLWorkbook(@"C:\Users\Robertson\Documents\projetos\rt_iot\dotNeParalelismProcessor\sample\data1000.xlsx");
            var file = xls.Worksheets.First(w => w.Name == "Sheet1");
            var totalRows = file.Rows().Count();
            var listOfPeople = new List<string>();
            for (int l = 1; l <= totalRows; l++)
            {
                var name = file.Cell($"A{l}").Value.ToString();
                listOfPeople.Add(name);
            }
            Console.WriteLine("First record from sample: " + listOfPeople[0]);
            Console.WriteLine("Las record from sample: " + listOfPeople[listOfPeople.Count() - 1]);
            synchronousProcessing(listOfPeople);
            parallelProcessing(listOfPeople);
        }


        static void parallelProcessing(List<string> listOfPeople) {
            var dataBaseCon = new DBconnection.Database();
            using var con = dataBaseCon.getConnection();
            dataBaseCon.dropAndCreateTable();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Parallel.ForEach(listOfPeople, person =>
            {
                dataBaseCon.inserPersonToDB(person);
            });
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("[C#] Parallel implementation took " + elapsedMs + " milliseconds and processed " + listOfPeople.Count() + " records.");
        }

        static void synchronousProcessing(List<string> listOfPeople) {
            var dataBaseCon = new DBconnection.Database();
            using var con = dataBaseCon.getConnection();
            dataBaseCon.dropAndCreateTable();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (string person in listOfPeople)
            {
                dataBaseCon.inserPersonToDB(person);
            };
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("[C#] Synchronous implementation took " + elapsedMs + " milliseconds and processed " + listOfPeople.Count() + " records.");
        }
    }
}

