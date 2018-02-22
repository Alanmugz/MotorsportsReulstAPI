namespace motorsportresultapi_public
{
    public class ConnectionStrings
    {
        private readonly string c_database;


        public string Database { get { return this.c_database; } }


        public ConnectionStrings(
            string database)
        {
            this.c_database = database;
        }
    }
}