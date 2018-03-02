using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public class PutCompetitorUpdate : MotorsportResultAPI.Domain.Workflow.Rally.IPutCompetitorUpdate
    {
        private MotorsportResultAPI.Data.Rally.ICompetitorRepository c_competitorRepository;

        private readonly MotorsportResultAPI.Data.Rally.Mapper c_mapper;
        private readonly MotorsportResultAPI.Data.Helper.Transformer c_transformer;
        

        public PutCompetitorUpdate(
            MotorsportResultAPI.Data.Rally.ICompetitorRepository competitorRepository,
            MotorsportResultAPI.Data.Rally.Mapper mapper,
            MotorsportResultAPI.Data.Helper.Transformer transformer)
        {
            this.c_competitorRepository = competitorRepository;
            this.c_mapper = mapper;
            this.c_transformer = transformer;
        }


        public (MotorsportResultAPI.Types.Domain.v1.Rally.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) Execute(
            string eventId,
            string competitorId,
            MotorsportResultAPI.Types.Domain.v1.Rally.StageResult stageResult)
        {
            var _id = $"{eventId}-{competitorId}";
			var _stageResults = new List<MotorsportResultAPI.Types.Data.v1.Rally.StageResult>();

			if (this.c_transformer.ValidateTimeSpan(stageResult.StageTime) == null || this.c_transformer.ValidateTimeSpan(stageResult.PenaltyTime) == null)
			{ return (null, MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat); }

			var _stageResult = this.c_mapper.MapStageResultToData(stageResult);
			var _competitor = this.c_competitorRepository.FetchById(_id);
			
			if (_competitor != null &&_competitor.StageResults.Exists(result => result.StageId == _stageResult.StageId))
			{
				var _correspondingDatabaseSatgeResult = _competitor.StageResults[stageResult.StageId - 1];
				if (_stageResult.Equals(_correspondingDatabaseSatgeResult)) { return (null, MotorsportResultAPI.Types.Enumeration.Results.MatchingElement); }
				foreach (var result in _competitor.StageResults)
				{
					if (result.StageId == _stageResult.StageId)
					{
						var _updatedStageResult = new MotorsportResultAPI.Types.Data.v1.Rally.StageResult(
							_stageResult.StageId,
							_stageResult.StageTime,
							_stageResult.PenaltyTime);
						_stageResults.Add(_updatedStageResult);
					}
					else
						_stageResults.Add(result);
				}

				var _updatedCompetitor = new MotorsportResultAPI.Types.Data.v1.Rally.Competitor(
					_competitor.Id,
					_competitor.EventId,
					_competitor.CarNumber,
					_competitor.Name,
					_competitor.Car,
					_competitor.Category,
					_stageResults);

				this.c_competitorRepository.Update(
                	_id,
                    _updatedCompetitor);

				return (this.c_mapper.MapCompetitorToDomain(_updatedCompetitor), MotorsportResultAPI.Types.Enumeration.Results.Updated);             
			}

			return (null, MotorsportResultAPI.Types.Enumeration.Results.DoesNotExist);
        }
    }
}