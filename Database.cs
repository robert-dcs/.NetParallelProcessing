using Npgsql;

namespace DBconnection
{
    class Database
    {
        public NpgsqlConnection getConnection()
        {
            var cs = "Host=localhost;Username=postgres;Password=321;Database=postgres";
            return new NpgsqlConnection(cs);
        }

        public void dropAndCreateTable()
        {
            using var con = getConnection();
            con.Open();
            using var cmdDrop = new NpgsqlCommand();
            cmdDrop.Connection = con;
            cmdDrop.CommandText = "DROP TABLE IF EXISTS persons";
            cmdDrop.ExecuteNonQuery();
            cmdDrop.CommandText = @"CREATE TABLE persons(id SERIAL PRIMARY KEY,name VARCHAR(255) NOT NULL)";
            cmdDrop.ExecuteNonQuery();
            Console.WriteLine("Table droped and created."); 
            con.Close(); 
        }

        public void inserPersonToDB(string person)
        {
            using var con = getConnection();
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