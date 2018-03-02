using MongoDB.Driver;
using MotorsportResultAPI.Types.Data.v1.Rally;
using MotorsportResultAPI.Types.Domain.v1.Rally;
using MotorsportResultAPI.Types.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MotorsportResultAPI.Data.Rally
{
    public class CompetitorRepository : MotorsportResultAPI.Data.Base, MotorsportResultAPI.Data.Rally.ICompetitorRepository
	{
		private readonly NLog.ILogger c_logger;
		private readonly IMongoCollection<MotorsportResultAPI.Types.Data.v1.Rally.Competitor> c_repository;
		private readonly MotorsportResultAPI.Data.Rally.Mapper c_mapper;
		private readonly MotorsportResultAPI.Data.Helper.Transformer c_transformer;


		public CompetitorRepository(
			NLog.ILogger logger,
			string connectionString,
			MotorsportResultAPI.Data.Rally.Mapper mapper,
			MotorsportResultAPI.Data.Helper.Transformer transformer)
			: base(connectionString)
		{
			var _database = base.ConnectToDatabase();
			this.c_repository = _database.GetCollection<MotorsportResultAPI.Types.Data.v1.Rally.Competitor>("competitors");
			this.c_mapper = mapper;
			this.c_transformer = transformer;
			this.c_logger = logger;
		}


		public MotorsportResultAPI.Types.Data.v1.Rally.Competitor FetchById(
			string id)
		{
			var _loggingContext = string.Format("{0}.FetchById", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _filter = Builders<MotorsportResultAPI.Types.Data.v1.Rally.Competitor>.Filter.Eq("Id", id);

			return this.c_repository.Find(_filter).FirstOrDefault();
		}


		public IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.Competitor> FetchByEventId(
			string eventId)
		{
			var _loggingContext = string.Format("{0}.FetchByEventId", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _filter = Builders<MotorsportResultAPI.Types.Data.v1.Rally.Competitor>.Filter.Eq("EventId", eventId);
			var _results = this.c_repository.Find(_filter).ToList();
			return _results.Select(result => this.c_mapper.MapCompetitorToDomain(result));
		}


		public void Save(
			MotorsportResultAPI.Types.Data.v1.Rally.Competitor subject)
		{
			var _loggingContext = string.Format("{0}.Save", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			this.c_repository.InsertOne(subject);
		}


		public void Append(
			string id,
			MotorsportResultAPI.Types.Data.v1.Rally.Competitor competitor)
		{
			var _loggingContext = string.Format("{0}.Append", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var filter = Builders<MotorsportResultAPI.Types.Data.v1.Rally.Competitor>.Filter.Eq("Id", id);
			this.c_repository.FindOneAndReplace(filter, competitor);
		}


		public void Update(
			string id,
			MotorsportResultAPI.Types.Data.v1.Rally.Competitor competitor)
		{
			var _loggingContext = string.Format("{0}.Update", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var filter = Builders<MotorsportResultAPI.Types.Data.v1.Rally.Competitor>.Filter.Eq("Id", id);
			this.c_repository.FindOneAndReplace(filter, competitor);
		}
    }
}