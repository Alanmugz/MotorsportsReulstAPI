using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NLog.Web;

 
namespace MotorsportResultAPI.Domain.Controllers.AutoCross
{
	//[MotorsportResultAPI.Public.Security.BasicAuthentication]
	[Route("autocross/v1")]
	public class CompetitorController : Controller
	{
		private readonly NLog.ILogger c_logger;
		private MotorsportResultAPI.Data.AutoCross.ICompetitorRepository c_competitorRepository;


		public CompetitorController(
			NLog.ILogger logger,
			MotorsportResultAPI.Data.AutoCross.ICompetitorRepository competitorRepository)
		{
			this.c_logger = logger;
			this.c_competitorRepository = competitorRepository;
		}


		[HttpGet]
		[Route("competitor")]
		public IActionResult GetCompetitor()
		{
			var _loggingContext = string.Format("{0}.GetCompetitor", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _carNumber = HttpContext.Request.Query["carNumber"].ToString();
			var _eventId = HttpContext.Request.Query["eventId"].ToString();

			var _competitorId = $"{_eventId}-{_carNumber}";
			var _result = this.c_competitorRepository.FetchById(_competitorId);
			return Ok(_result);
		}


		[HttpGet]
		[Route("event")]
		public IActionResult GetEvent()
		{
			var eventId = HttpContext.Request.Query["id"].ToString();
			var stageId = Convert.ToInt32(HttpContext.Request.Query["stageId"]);

			var _loggingContext = string.Format("{0}.GetEvent", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _competitorResults = this.c_competitorRepository.FetchByEventId(eventId.ToString());
			var _previousStageResult = this.PreviousStageResult(_competitorResults, stageId);
			var _response = this.BuildCompetitorResponse(_competitorResults, _previousStageResult, stageId);
			return Ok(_response);
		}


		[HttpPost]
		[Route("eventid/{eventid}/competitor")]
		public IActionResult PostCompetitor(
			[FromRoute] int eventId,
			[FromBody] MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.Competitor competitor)
		{
			var _loggingContext = string.Format("{0}.PostCompetitor", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _domainCompetitor = this.MapToDomainCompetitor(eventId, competitor);
			var _result = this.c_competitorRepository.Save(_domainCompetitor);

			return this.ParseResult(_result, eventId, competitor.CarNumber);
		}


		[HttpPost]
		[Route("eventid/{eventid}/competitorid/{competitorid}/stageresult/append")]
		public IActionResult Post(
			[FromRoute] int eventId,
			[FromRoute] int competitorid,
			[FromBody] MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.StageResult stageResult)
		{
			var _loggingContext = string.Format("{0}.PostAppend", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _domainStageResult = this.MapToDomainStageResult(stageResult);
			var _result = this.c_competitorRepository.Append(
				eventId,
				competitorid,
				_domainStageResult);

			return this.ParseAppendResult(_result, eventId, competitorid);
		}


		[HttpPut]
		[Route("eventid/{eventid}/competitorid/{competitorid}/stageresult/update")]
		public IActionResult PutUpdate(
			[FromRoute] int eventId,
			[FromRoute] int competitorid,
			[FromBody] MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.StageResult stageResult)
		{
			var _loggingContext = string.Format("{0}.PutUpdate", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _domainStageResult = this.MapToDomainStageResult(stageResult);
			var _result = this.c_competitorRepository.Update(
				eventId,
				competitorid,
				_domainStageResult);

			this.c_logger.Info("{0} Completed", _loggingContext);
			return this.ParseUpdateResult(_result, eventId, competitorid);
		}


		private IEnumerable<MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.CompetitorResponse> BuildCompetitorResponse(
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor> competitorResults,
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.AutoCross.PreviousResult> previousStageResult,
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
				var _competitor = new MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.CompetitorResponse(
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


		private IEnumerable<MotorsportResultAPI.Types.Domain.v1.AutoCross.PreviousResult> PreviousStageResult(
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor> previousStageResult,
			int stageId)
		{
			return previousStageResult
				.Where(competitor => competitor.StageResults.Count() >= (stageId - 1))
				.Select(result =>
				{
					var OverallTime = this.BuildOverallTime(result, (stageId - 1));
					return new MotorsportResultAPI.Types.Domain.v1.AutoCross.PreviousResult(result.CarNumber, OverallTime);
				})
				.OrderBy(_response => _response.OverallTime);
		}


		private string PrevStagePosition(
			string carNumber,
			int position,
			int stageId,
			IEnumerable<Types.Domain.v1.AutoCross.PreviousResult> previousStageResult)
		{
			var _previousStagePosition = previousStageResult
				.Select((competitor, count) => new { carNumber = competitor.CarNumber, index = (count + 1) })
				.First(competitor => competitor.carNumber == carNumber).index;

			if (stageId == 1 || position == _previousStagePosition)	{ return "Equal"; }
			else if (position > _previousStagePosition)	{ return "Down"; }
			return "Up";
		}


		private TimeSpan BuildOverallTime(
			MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor competitor,
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


		private IActionResult ParseResult(
			(MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) result,
			int eventId,
			string carNumber)
		{
			switch (result.Item2)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/Competitor", "Competitor already exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.Created:
				default:
					var _uri = $"{this.Request.Scheme}://{this.Request.Host}/autocross/v1/competitor?carnumber={carNumber}&eventid={eventId}";
					return Created(_uri, result.Item1);
			}
		}


		private IActionResult ParseAppendResult(
			(MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) result,
			int eventId,
			int carNumber)
		{
			switch (result.Item2)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult already exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "invalid time format"));
				case MotorsportResultAPI.Types.Enumeration.Results.PreviousStageResultDoesNotExist:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "previous stage result does not exist"));
				case MotorsportResultAPI.Types.Enumeration.Results.Appended:
				default:
					var _uri = $"{this.Request.Scheme}://{this.Request.Host}/autocross/v1/competitor?carnumber={carNumber}&eventid={eventId}";
					return Created(_uri, result.Item1);
			}
		}


		private IActionResult ParseUpdateResult(
			(MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) result,
			int eventId,
			int carNumber)
		{
			switch (result.Item2)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.DoesNotExist:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult does not exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "invalid time format"));
				case MotorsportResultAPI.Types.Enumeration.Results.MatchingElement:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult to be updated matches existing stageresult"));
				case MotorsportResultAPI.Types.Enumeration.Results.Updated:
				default:
					var _uri = $"{this.Request.Scheme}://{this.Request.Host}/autocross/v1/competitor?carnumber={carNumber}&eventid={eventId}";
					return Created(_uri, result.Item1);
			}
		}


		private MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor MapToDomainCompetitor(
			int eventId,
			MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.Competitor competitor)
		{
			return new MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor(
				$"{eventId}-{competitor.CarNumber}",
				eventId.ToString(),
				competitor.CarNumber,
				competitor.Name,
				competitor.Car,
				competitor.Category,
				competitor.StageResults);
		}


		private MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult MapToDomainStageResult(
			MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.StageResult stageResult)
		{
			return new MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult(
				stageResult.StageId,
				TimeSpan.Parse(stageResult.StageTime),
				TimeSpan.Parse(stageResult.PenaltyTime));
		}
	}
}