using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MotorsportResultAPI.Data.AutoCross
{
    public class CompetitorRepository : MotorsportResultAPI.Data.Base, MotorsportResultAPI.Data.AutoCross.ICompetitorRepository
	{
		private readonly NLog.ILogger c_logger;
		private readonly IMongoCollection<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor> c_repository;
		private readonly MotorsportResultAPI.Data.AutoCross.Mapper c_mapper;
		private readonly MotorsportResultAPI.Data.Helper.Transformer c_transformer;


		public CompetitorRepository(
			NLog.ILogger logger,
			string connectionString,
			MotorsportResultAPI.Data.AutoCross.Mapper mapper,
			MotorsportResultAPI.Data.Helper.Transformer transformer)
			: base(connectionString)
		{
			var _database = base.ConnectToDatabase();
			this.c_repository = _database.GetCollection<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>("competitors");
			this.c_mapper = mapper;
			this.c_transformer = transformer;
			this.c_logger = logger;
		}


		public MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor FetchById(
			string id)
		{
			var _loggingContext = string.Format("{0}.FetchById", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _filter = Builders<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>.Filter.Eq("Id", id);
			var _result = this.c_repository.Find(_filter).FirstOrDefault();
			return _result == null ? null : this.c_mapper.MapCompetitorToDomain(_result);
		}


		public IEnumerable<MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor> FetchByEventId(
			string eventId)
		{
			var _loggingContext = string.Format("{0}.FetchByEventId", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _filter = Builders<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>.Filter.Eq("EventId", eventId);
			var _results = this.c_repository.Find(_filter).ToList();
			return _results.Select(result => this.c_mapper.MapCompetitorToDomain(result));
		}


		public (MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) Save(
			MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor subject)
		{
			var _loggingContext = string.Format("{0}.Save", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _competitor = this.FetchById(subject.Id);

			if (_competitor == null)
			{
				var _dataCompetitor = this.c_mapper.MapCompetitorToData(subject);
				this.c_repository.InsertOne(_dataCompetitor);
				
				return (subject, MotorsportResultAPI.Types.Enumeration.Results.Created);
			}

			return (subject, MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists);
		}


		public (MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) Append(
			int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult stageResult)
		{
			var _loggingContext = string.Format("{0}.Append", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _id = $"{eventId}-{competitorId}";

			if (this.c_transformer.ValidateTimeSpan(stageResult.StageTime) == null || this.c_transformer.ValidateTimeSpan(stageResult.PenaltyTime) == null)
			{ return (null , MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat); }

			var _stageResult = this.c_mapper.MapStageResultToData(stageResult);
			var _competitor = this.c_mapper.MapCompetitorToData(this.FetchById(_id));

			if (_competitor != null && _competitor.StageResults.Count() == stageResult.StageId - 1)
			{
				var filter = Builders<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>.Filter.Eq("Id", _id);
				_competitor.StageResults.Add(_stageResult);
				this.c_repository.FindOneAndReplace(filter, _competitor);
				var _updatedCompetitor = this.FetchById(_id);

				return (_updatedCompetitor , MotorsportResultAPI.Types.Enumeration.Results.Appended);
			}
			if(stageResult.StageId > _competitor.StageResults.Count()){ return (null , MotorsportResultAPI.Types.Enumeration.Results.PreviousStageResultDoesNotExist); }
			return (null , MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists);
		}


		public (MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) Update(
			int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult stageResult)
		{
			var _loggingContext = string.Format("{0}.Update", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _id = $"{eventId}-{competitorId}";
			var _stageResults = new List<MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult>();

			if (this.c_transformer.ValidateTimeSpan(stageResult.StageTime) == null || this.c_transformer.ValidateTimeSpan(stageResult.PenaltyTime) == null)
			{ return (null, MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat); }

			var _stageResult = this.c_mapper.MapStageResultToData(stageResult);
			var _competitor = this.c_mapper.MapCompetitorToData(this.FetchById(_id));
			
			if (_competitor.StageResults.Exists(result => result.StageId == _stageResult.StageId))
			{
				var _correspondingDatabaseSatgeResult = _competitor.StageResults[stageResult.StageId - 1];
				if (_stageResult.Equals(_correspondingDatabaseSatgeResult)) { return (null, MotorsportResultAPI.Types.Enumeration.Results.MatchingElement); }
				foreach (var result in _competitor.StageResults)
				{
					if (result.StageId == _stageResult.StageId)
					{
						var _updatedStageResult = new MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult(
							_stageResult.StageId,
							_stageResult.StageTime,
							_stageResult.PenaltyTime);
						_stageResults.Add(_updatedStageResult);
					}
					else
						_stageResults.Add(result);
				}

				var updatedCompetitor = new MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor(
					_competitor.Id,
					_competitor.EventId,
					_competitor.CarNumber,
					_competitor.Name,
					_competitor.Car,
					_competitor.Category,
					_stageResults);

				var filter = Builders<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>.Filter.Eq("Id", _id);
				this.c_repository.FindOneAndReplace(filter, updatedCompetitor);
				var _updatedCompetitor = this.FetchById(_id);

				return (_updatedCompetitor, MotorsportResultAPI.Types.Enumeration.Results.Updated);             
			}

			return (null, MotorsportResultAPI.Types.Enumeration.Results.DoesNotExist);
		}
	}
}