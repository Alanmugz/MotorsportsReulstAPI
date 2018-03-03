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


        public MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResultResponse Execute(
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


        private MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResultResponse BuildCompetitorResponse(
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.Competitor> competitorResults,
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.PreviousResult> previousStageResult,
			int stageId)
		{
			var _stageResult = this.BuildStageResult(competitorResults, stageId);
			var _overallResult = this.BuildOverallResult(competitorResults, previousStageResult, stageId);

			return new MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResultResponse(_stageResult, _overallResult);
		}


		private IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorStageResult> BuildStageResult(
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.Competitor> competitorResults,
			int stageId)
		{
			var _stageResultOrdered = competitorResults
				.Where(competitorStageResult => competitorStageResult.StageResults.Count() >= stageId)
				.Select(competitor => {
					var _overallTime = this.BuildOverallTime(competitor, stageId);
					var _stageTime = competitor.StageResults.Where(stageResult => stageResult.StageId == stageId).First().StageTime;
					var _penaltyTime = competitor.StageResults.Where(stageResult => stageResult.StageId == stageId).First().PenaltyTime;
					return new {
						CarNumber = competitor.CarNumber,
						Name = competitor.Name,
						Car = competitor.Car,
						Category = competitor.Category,
						OverallTime = _overallTime,
						StageTime = _stageTime,
						PenaltyTime = _penaltyTime };
				})
			.OrderBy(response => response.StageTime).ToList();

			return _stageResultOrdered
				.Select((competitor, position) => {
					var _differenceToLeader = _stageResultOrdered.First().StageTime;
					var _differenceToPrevious = _stageResultOrdered.First().StageTime;
					if (position > 0) { _differenceToPrevious = _stageResultOrdered.ElementAt(position - 1).StageTime; }
					return new MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorStageResult(
						++position,
						competitor.CarNumber,
						competitor.Name,
						competitor.Car,
						competitor.Category,
						competitor.StageTime,
						competitor.StageTime.Subtract(_differenceToLeader),
						competitor.StageTime.Subtract(_differenceToPrevious));
				});
		}


		private IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorOverallResult> BuildOverallResult(
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.Competitor> competitorResults,
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.PreviousResult> previousStageResult,
			int stageId)
		{
			var _overallResultOrdered = competitorResults
				.Where(competitorStageResult => competitorStageResult.StageResults.Count() >= stageId)
				.Select(competitor => {
					var _overallTime = this.BuildOverallTime(competitor, stageId);
					var _stageTime = competitor.StageResults.Where(stageResult => stageResult.StageId == stageId).First().StageTime;
					var _penaltyTime = competitor.StageResults.Where(stageResult => stageResult.StageId == stageId).First().PenaltyTime;
					return new {
						CarNumber = competitor.CarNumber,
						Name = competitor.Name,
						Car = competitor.Car,
						Category = competitor.Category,
						OverallTime = _overallTime,
						StageTime = _stageTime,
						PenaltyTime = _penaltyTime };
				})
			.OrderBy(response => response.OverallTime).ToList();

			return _overallResultOrdered.Select((competitor, position) => {
				var _differenceToLeader = _overallResultOrdered.First().OverallTime;
				var _differenceToPrevious = _overallResultOrdered.First().OverallTime;
				if (position > 0) { _differenceToPrevious = _overallResultOrdered.ElementAt(position - 1).OverallTime; }
				return new MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorOverallResult(
					++position,
					competitor.CarNumber,
					competitor.Name,
					competitor.Car,
					competitor.Category,
					competitor.OverallTime,
					competitor.PenaltyTime,
					competitor.OverallTime.Subtract(_differenceToLeader),
					competitor.OverallTime.Subtract(_differenceToPrevious),
					this.PrevStagePosition(competitor.CarNumber, position, stageId, previousStageResult));
			});
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