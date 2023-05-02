using Npgsql;

namespace DBconnection
{
    class Database
    {
        public NpgsqlConnection getConnection()
        {
            
            var cs = "Host=localhost;Username=postgres;Password=321;Database=postgres";
            using var con = new NpgsqlConnection(cs);
            return con;
            
        }

        public void printDbVersion()
        {
            

            var cs = "Host=localhost;Username=postgres;Password=321;Database=postgres";

            using var con = new NpgsqlConnection(cs);
            con.Open();

            var sql = "SELECT version()";

            using var cmd = new NpgsqlCommand(sql, con);

            var version = cmd.ExecuteScalar().ToString();
            Console.WriteLine($"PostgreSQL version: {version}");
            
        }
        public void inserPersonToDB(string person)
        {
            var cs = "Host=localhost;Username=postgres;Password=321;Database=postgres";
            using var con = new NpgsqlConnection(cs);
            con.Open();
            var sql = "INSERT INTO persons(name) VALUES(@name)";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("name", person);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}