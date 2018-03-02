using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public class PostCompetitorAppend : MotorsportResultAPI.Domain.Workflow.Rally.IPostCompetitorAppend
    {
        private MotorsportResultAPI.Data.Rally.ICompetitorRepository c_competitorRepository;

        private readonly MotorsportResultAPI.Data.Rally.Mapper c_mapper;
        private readonly MotorsportResultAPI.Data.Helper.Transformer c_transformer;
        

        public PostCompetitorAppend(
            MotorsportResultAPI.Data.Rally.ICompetitorRepository competitorRepository,
            MotorsportResultAPI.Data.Rally.Mapper mapper,
			MotorsportResultAPI.Data.Helper.Transformer transformer)
        {
            this.c_competitorRepository = competitorRepository;
            this.c_mapper = mapper;
            this.c_transformer = transformer;
        }


        public (MotorsportResultAPI.Types.Domain.v1.Rally.Competitor, MotorsportResultAPI.Types.Enumeration.Results) Execute(
            int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.Rally.StageResult stageResult)
        {
			var _id = $"{eventId}-{competitorId}";

			if (this.c_transformer.ValidateTimeSpan(stageResult.StageTime) == null || this.c_transformer.ValidateTimeSpan(stageResult.PenaltyTime) == null)
			{ return (null , MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat); }

			var _stageResult = this.c_mapper.MapStageResultToData(stageResult);
			var _competitor = this.c_competitorRepository.FetchById(_id);

			if (_competitor != null && _competitor.StageResults.Count() == stageResult.StageId - 1)
			{
				_competitor.StageResults.Add(_stageResult);
				this.c_competitorRepository.Append(
					_id,
					_competitor);

				return (this.c_mapper.MapCompetitorToDomain(_competitor), MotorsportResultAPI.Types.Enumeration.Results.Appended);
			}
			if(stageResult.StageId > _competitor.StageResults.Count()){ return (null , MotorsportResultAPI.Types.Enumeration.Results.PreviousStageResultDoesNotExist); }
			
			return (null , MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists);
        }
    }
}