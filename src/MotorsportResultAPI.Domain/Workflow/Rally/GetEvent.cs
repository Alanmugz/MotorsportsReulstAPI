using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public class GetEvent : MotorsportResultAPI.Domain.Workflow.Rally.IGetEvent
    {
        private MotorsportResultAPI.Data.Rally.ICompetitorRepository c_competitorRepository;

        private readonly MotorsportResultAPI.Data.Rally.Mapper c_mapper;
        

        public GetEvent(
            MotorsportResultAPI.Data.Rally.ICompetitorRepository competitorRepository,
            MotorsportResultAPI.Data.Rally.Mapper mapper)
        {
            this.c_competitorRepository = competitorRepository;
            this.c_mapper = mapper;
        }


        public IEnumerable<MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResponse> Execute(
            string eventId,
            int stageId)
        {
            var _competitorResults = this.c_competitorRepository.FetchByEventId(eventId);
			var _previousStageResult = this.PreviousStageResult(_competitorResults, stageId);
			var _result = this.BuildCompetitorResponse(_competitorResults, _previousStageResult, stageId);

            return _result == null ? null : _result;
        }


        private IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.PreviousResult> PreviousStageResult(
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.Competitor> previousStageResult,
			int stageId)
		{
			return previousStageResult
				.Where(competitor => competitor.StageResults.Count() >= (stageId - 1))
				.Select(result =>
				{
					var OverallTime = this.BuildOverallTime(result, (stageId - 1));
					return new MotorsportResultAPI.Types.Domain.v1.Rally.PreviousResult(result.CarNumber, OverallTime);
				})
				.OrderBy(_response => _response.OverallTime);
		}


        private TimeSpan BuildOverallTime(
			MotorsportResultAPI.Types.Domain.v1.Rally.Competitor competitor,
			int stageId)
		{
			var _stageTotal = competitor.StageResults
					.Where(stageResult => stageResult.StageId <= stageId)
					.Aggregate(
						TimeSpan.Zero, (subStageTotal, stageResult) => subStageTotal.Add(stageResult.StageTime)
				);
			var _penaltyTotal = competitor.StageResults
					.Where(stageResult => stageResult.StageId <= stageId)
					.Aggregate(
						TimeSpan.Zero, (subPenaltyTotal, stageResult) => subPenaltyTotal.Add(stageResult.PenaltyTime)
				);

			return _stageTotal.Add(_penaltyTotal);
		}


        private IEnumerable<MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResponse> BuildCompetitorResponse(
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.Competitor> competitorResults,
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.PreviousResult> previousStageResult,
			int stageId)
		{
			var _competitorStageResults = competitorResults
				.Where(competitorStageResult => competitorStageResult.StageResults.Count() >= stageId)
				.Select(competitor => {
					var OverallTime = this.BuildOverallTime(competitor, stageId);
					var StageTime = competitor.StageResults.Where(stageResult => stageResult.StageId == stageId).First().StageTime;
					var PenaltyTime = competitor.StageResults.Where(stageResult => stageResult.StageId == stageId).First().PenaltyTime;
					return new {
						competitor.CarNumber,
						competitor.Name,
						competitor.Car,
						competitor.Category,
						OverallTime,
						StageTime,
						PenaltyTime };
				})
			.OrderBy(response => response.OverallTime).ToList();
		
			var _result = _competitorStageResults.Select((competitor, position) => {
				var _differenceToLeader = TimeSpan.Zero;
				var _differenceToPrevious = _competitorStageResults.First().OverallTime;
				if (position == 0) { _differenceToLeader = _competitorStageResults.First().OverallTime; }
				if (position > 0) { _differenceToPrevious = _competitorStageResults.ElementAt(position - 1).OverallTime; }
				var _competitor = new MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResponse(
					++position,
					competitor.CarNumber,
					competitor.Name,
					competitor.Car,
					competitor.Category,
					competitor.OverallTime,
					competitor.StageTime,
					competitor.PenaltyTime,
					competitor.OverallTime.Subtract(_differenceToLeader),
					competitor.OverallTime.Subtract(_differenceToPrevious),
					this.PrevStagePosition(competitor.CarNumber, position, stageId, previousStageResult));

				return _competitor;
			});

			return _result;
		}


        private string PrevStagePosition(
			string carNumber,
			int position,
			int stageId,
			IEnumerable<Types.Domain.v1.Rally.PreviousResult> previousStageResult)
		{
			var _previousStagePosition = previousStageResult
				.Select((competitor, count) => new { carNumber = competitor.CarNumber, index = (count + 1) })
				.First(competitor => competitor.carNumber == carNumber).index;

			if (stageId == 1 || position == _previousStagePosition)	{ return "Equal"; }
			else if (position > _previousStagePosition)	{ return "Down"; }
			return "Up";
		}
    }
}