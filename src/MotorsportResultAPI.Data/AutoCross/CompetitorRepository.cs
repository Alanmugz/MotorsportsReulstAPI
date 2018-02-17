using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MotorsportResultAPI.Data.AutoCross
{
	public class CompetitorRepository : MotorsportResultAPI.Data.Base, MotorsportResultAPI.Data.AutoCross.ICompetitorRepository
	{
		private readonly IMongoCollection<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor> c_repository;
		private readonly MotorsportResultAPI.Data.AutoCross.Mapper c_mapper;
		private readonly MotorsportResultAPI.Data.Helper.Transformer c_transformer;
		//private readonly ILog c_logger;


		public CompetitorRepository(
			string connectionString,
			MotorsportResultAPI.Data.AutoCross.Mapper mapper,
			MotorsportResultAPI.Data.Helper.Transformer transformer)
			//ILog logger)
			: base(connectionString)
		{
			var _database = base.ConnectToDatabase();
			this.c_repository = _database.GetCollection<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>("competitors");
			this.c_mapper = mapper;
			this.c_transformer = transformer;
			//this.c_logger = logger;
		}


		public MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor FetchById(
			string id)
		{
			var _filter = Builders<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>.Filter.Eq("Id", id);
			return this.c_repository.Find(_filter).FirstOrDefault();
		}


		public IEnumerable<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor> FetchByEventId(
			string eventId)
		{
			var _filter = Builders<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>.Filter.Eq("EventId", eventId);

			return this.c_repository.Find(_filter).ToList();
		}


		public MotorsportResultAPI.Types.Enumeration.Results Save(
			MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor subject)
		{
			var _competitor = this.FetchById(subject.Id);

			if (_competitor == null)
			{
				var _dataCompetitor = this.c_mapper.MapCompetitorToData(subject);
				this.c_repository.InsertOne(_dataCompetitor);
				
				return MotorsportResultAPI.Types.Enumeration.Results.Created;
			}

			return MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists;
		}


		public MotorsportResultAPI.Types.Enumeration.Results Append(
			int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult stageResult)
		{
			var _id = $"{eventId}-{competitorId}";

			if (this.c_transformer.ValidateTimeSpan(stageResult.StageTime) == null || this.c_transformer.ValidateTimeSpan(stageResult.PenaltyTime) == null)
			{ return MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat; }

			var _stageResult = this.c_mapper.MapResultToData(stageResult);
			var _competitor = this.FetchById(_id);

			if (_competitor != null && _competitor.StageResults.Count() == stageResult.StageId - 1)
			{
				var filter = Builders<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>.Filter.Eq("Id", _id);
				_competitor.StageResults.Add(_stageResult);
				this.c_repository.FindOneAndReplace(filter, _competitor);

				return MotorsportResultAPI.Types.Enumeration.Results.Appended;
			}

			return MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists;
		}


		public MotorsportResultAPI.Types.Enumeration.Results Update(
			int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult stageResult)
		{
			var _id = $"{eventId}-{competitorId}";
			var newList = new List<MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult>();

			if (this.c_transformer.ValidateTimeSpan(stageResult.StageTime) == null || this.c_transformer.ValidateTimeSpan(stageResult.PenaltyTime) == null)
			{ return MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat; }

			var _stageResult = this.c_mapper.MapResultToData(stageResult);
			var _competitor = this.FetchById(_id);
			
			if (_competitor.StageResults.Exists(result => result.StageId == _stageResult.StageId))
			{
				var _correspondingDatabaseSatgeResult = _competitor.StageResults[stageResult.StageId - 1];
				if (_stageResult.Equals(_correspondingDatabaseSatgeResult)) { return MotorsportResultAPI.Types.Enumeration.Results.MatchingElement; }
				foreach (var result in _competitor.StageResults)
				{
					if (result.StageId == _stageResult.StageId)
					{
						var _newStageResult = new MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult(
							_stageResult.StageId,
							_stageResult.StageTime,
							_stageResult.PenaltyTime);
						newList.Add(_newStageResult);
					}
					else
						newList.Add(result);
				}

				var updatedCompetitor = new MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor(
					_competitor.Id,
					_competitor.EventId,
					_competitor.CarNumber,
					_competitor.Name,
					_competitor.Car,
					_competitor.Category,
					newList);

				var filter = Builders<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor>.Filter.Eq("Id", _id);
				this.c_repository.FindOneAndReplace(filter, updatedCompetitor);

				return MotorsportResultAPI.Types.Enumeration.Results.Updated;
			}

			return MotorsportResultAPI.Types.Enumeration.Results.DoesNotExist;
		}
	}
}
