using MongoDB.Driver;
using System;


namespace MotorsportResultAPI.Data
{
	public class Base
	{
		private readonly string c_connectionString;

		public Base(
			string connectionString)
		{
			this.c_connectionString = connectionString;
		}


		public IMongoDatabase ConnectToDatabase()
		{
			//var _connectionString = this.c_connectionString.Split('*')[0];
			var _connectionString = "mongodb://localhost";
			//var _database = this.c_connectionString.Split('*')[1];
			var _database = "autocross";
			var client = new MongoClient(_connectionString);
			return client.GetDatabase(_database);
		}
	}
}
