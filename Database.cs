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
            cmdDrop.CommandText = "DROP TABLE IF EXISTS person";
            cmdDrop.ExecuteNonQuery();
            cmdDrop.CommandText = @"CREATE TABLE person(id SERIAL PRIMARY KEY,name VARCHAR(255) NOT NULL)";
            cmdDrop.ExecuteNonQuery();
            Console.WriteLine("DB initialized"); 
            con.Close(); 
        }

        public void inserPersonToDB(string person)
        {
            using var con = getConnection();
            con.Open();
            var sql = "INSERT INTO person(name) VALUES(@name)";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("name", person);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        
        public void countRows()
        {
            using var con = getConnection();
            con.Open();
            var sql = "SELECT count(*) FROM person";
            using var cmd = new NpgsqlCommand(sql, con);
            Int64 count = (Int64) cmd.ExecuteScalar();

            Console.WriteLine("DB rows: " + count);
 
            con.Close();
        }

        
        public void getFirstRecord()
        {
            using var con = getConnection();
            con.Open();
            var sql = "SELECT name FROM person ORDER BY id ASC LIMIT 1";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, con))
            {
                string name;
                NpgsqlDataReader reader = command.ExecuteReader();
                while(reader.Read()){
                    name = reader[0].ToString();
                    Console.WriteLine("First row: " + name); 
                }

                con.Close();
            }
        }

        
        public void getLastRecord()
        {
            using var con = getConnection();
            con.Open();
            var sql = "SELECT name FROM person ORDER BY id DESC LIMIT 1";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, con))
            {
                string name;
                NpgsqlDataReader reader = command.ExecuteReader();
                while(reader.Read()){
                    name = reader[0].ToString();
                    Console.WriteLine("Last row: " + name); 
                }
                con.Close();
            }
        }
    }
}