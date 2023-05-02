using Npgsql;
using ClosedXML.Excel;

namespace LerPlanilhaExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            var xls = new XLWorkbook(@"C:\Users\Robertson\Documents\projetos\rt_iot\dotNeParalelismProcessor\data.xlsx");
            var planilha = xls.Worksheets.First(w => w.Name == "Sheet1");
            var totalLinhas = planilha.Rows().Count();
            // primeira linha é o cabecalho
            var persons = new List<string>();
            for (int l = 1; l <= totalLinhas; l++)
            {
                var name = planilha.Cell($"A{l}").Value.ToString();
                persons.Add(name);
                //Console.WriteLine($"{name}");
            }
            Console.WriteLine(persons.Count);
            synchronousProcessing(persons);
            parallelProcessing(persons);
        }


        static void parallelProcessing(List<string> listOfPerons) {
            var dataBaseCon = new DBconnection.Database();
            var cs = "Host=localhost;Username=postgres;Password=321;Database=postgres";
            using var con = new NpgsqlConnection(cs);
            con.Open();
            using var cmdDrop = new NpgsqlCommand();
            cmdDrop.Connection = con;
            cmdDrop.CommandText = "DROP TABLE IF EXISTS persons";
            cmdDrop.ExecuteNonQuery();
            cmdDrop.CommandText = @"CREATE TABLE persons(id SERIAL PRIMARY KEY,name VARCHAR(255) NOT NULL)";
            cmdDrop.ExecuteNonQuery();
            Console.WriteLine("Table droped and created."); 
            con.Close(); 
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Parallel.ForEach(listOfPerons, person =>
            {
                dataBaseCon.inserPersonToDB(person);
            });
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Parallel people saved. Tooked milliseconds: " + elapsedMs);

        }

        static void synchronousProcessing(List<string> listOfPerons) {
            var dataBaseCon = new DBconnection.Database();
            var cs = "Host=localhost;Username=postgres;Password=321;Database=postgres";
            using var con = new NpgsqlConnection(cs);
            con.Open();
            using var cmdDrop = new NpgsqlCommand();
            cmdDrop.Connection = con;
            cmdDrop.CommandText = "DROP TABLE IF EXISTS persons";
            cmdDrop.ExecuteNonQuery();
            cmdDrop.CommandText = @"CREATE TABLE persons(id SERIAL PRIMARY KEY,name VARCHAR(255) NOT NULL)";
            cmdDrop.ExecuteNonQuery();
            Console.WriteLine("Table droped and created."); 
            con.Close();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (string person in listOfPerons)
            {
                dataBaseCon.inserPersonToDB(person);
            };
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Synchronous people saved. Tooked milliseconds: " + elapsedMs);
        }
    }
}

