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
		//private readonly MotorsportResultAPI.Data.Helper.Transformer c_transformer;
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
			var _loggingContext = string.Format("{0}.Get", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _carNumber = HttpContext.Request.Query["carNumber"].ToString();
			var _eventId = HttpContext.Request.Query["eventId"].ToString();

			var _competitorId = $"{_eventId}-{_carNumber}";
			var _result = this.c_competitorRepository.FetchById(_competitorId);

			this.c_logger.Info("{0} Completed", _loggingContext);
			return Ok(_result);
		}


		[HttpGet]
		[Route("event")]
		public IActionResult GetEvent()
		{
			var eventId = HttpContext.Request.Query["id"].ToString();
			var stageId = Convert.ToInt32(HttpContext.Request.Query["stageId"]);

			var _loggingContext = string.Format("{0}.Get", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _competitorResults = this.c_competitorRepository.FetchByEventId(eventId.ToString());
			var _previousStageResult = this.PreviousStageResult(_competitorResults, stageId);
			var _response = this.BuildompetitorResponse(_competitorResults, _previousStageResult, stageId);

			this.c_logger.Info("{0} Completed", _loggingContext);
			return Ok(_response);
		}


		[HttpPost]
		[Route("eventid/{eventid}/competitor")]
		public IActionResult Post(
			[FromRoute] int eventid,
			[FromBody] MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.Competitor competitor)
		{
			var _loggingContext = string.Format("{0}.Post", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _domainCompetitor = this.MapToDomainCompetitor(eventid, competitor);
			var _result = this.c_competitorRepository.Save(_domainCompetitor);

			this.c_logger.Info("{0} Completed", _loggingContext);
			return this.ParseResult(_result);
		}


		[HttpPost]
		[Route("eventid/{eventid}/competitorid/{competitorid}/stageresult/append")]
		public IActionResult Post(
			[FromRoute] int eventId,
			[FromRoute] int competitorid,
			[FromBody] MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult stageResult)
		{
			var _loggingContext = string.Format("{0}.Post", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _result = this.c_competitorRepository.Append(
				eventId,
				competitorid,
				stageResult);

			this.c_logger.Info("{0} Completed", _loggingContext);
			return this.ParseAppendResult(_result);
		}


		[HttpPut]
		[Route("eventid/{eventid}/competitorid/{competitorid}/stageresult/update")]
		public IActionResult Put(
			[FromRoute] int eventId,
			[FromRoute] int competitorid,
			[FromBody] MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult stageResult)
		{
			var _loggingContext = string.Format("{0}.Post", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _result = this.c_competitorRepository.Update(
				eventId,
				competitorid,
				stageResult);

			this.c_logger.Info("{0} Completed", _loggingContext);
			return this.ParseUpdateResult(_result);
		}


		private IEnumerable<MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.CompetitorResponse> BuildompetitorResponse(
			IEnumerable<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor> competitorResults,
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.AutoCross.PreviousResult> previousStageResult,
			int stageId)
		{
			var _competitorStageResults = competitorResults
				.Where(competitorStageResult => competitorStageResult.StageResults.Count() >= stageId)
				.Select(competitor => {
					var OverallTime = this.BuildOverallTime(competitor, stageId);
					var StageTime = competitor.StageResults.Where(stageResult => stageResult.StageId == stageId).First().StageTime.ToString();
					var PenaltyTime = competitor.StageResults.Where(stageResult => stageResult.StageId == stageId).First().PenaltyTime.ToString();
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
				var _differenceToPrevious = TimeSpan.Parse(_competitorStageResults.First().OverallTime);;
				if (position == 0) { _differenceToLeader = TimeSpan.Parse(_competitorStageResults.First().OverallTime); }
				if (position > 0) { _differenceToPrevious = TimeSpan.Parse(_competitorStageResults.ElementAt(position - 1).OverallTime); }
				var _competitor = new MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross.CompetitorResponse(
					++position,
					competitor.CarNumber,
					competitor.Name,
					competitor.Car,
					competitor.Category,
					competitor.OverallTime,
					competitor.StageTime,
					competitor.PenaltyTime,
					TimeSpan.Parse(competitor.OverallTime).Subtract(_differenceToLeader).ToString(),
					TimeSpan.Parse(competitor.OverallTime).Subtract(_differenceToPrevious).ToString(),
					this.PrevStagePosition(competitor.CarNumber, position, stageId, previousStageResult));

				return _competitor;
			});

			return _result;
		}


		private IEnumerable<MotorsportResultAPI.Types.Domain.v1.AutoCross.PreviousResult> PreviousStageResult(
			IEnumerable<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor> previousStageResult,
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
			IEnumerable<Types.Domain.v1.AutoCross.PreviousResult> prevResult)
		{
			int prevPosition = prevResult.Select((v, i) => new { carNumber = v.CarNumber, index = (i + 1) }).First(element => element.carNumber == carNumber).index;
			if (stageId == 1 || position == prevPosition)
			{
				return "Equal";
			}
			else if (position > prevPosition)
			{
				return "Down";
			}

			return "Up";
		}


		private string BuildOverallTime(
			MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor competitor,
			int stageId)
		{
			var _stageTotal = competitor.StageResults
					.Where(stageResult => stageResult.StageId <= stageId)
					.Aggregate(
						TimeSpan.Zero, (subStageTotal, stageResult) => subStageTotal.Add(TimeSpan.Parse(stageResult.StageTime))
				);
			var _penaltyTotal = competitor.StageResults
					.Where(stageResult => stageResult.StageId <= stageId)
					.Aggregate(
						TimeSpan.Zero, (subPenaltyTotal, stageResult) => subPenaltyTotal.Add(TimeSpan.Parse(stageResult.PenaltyTime))
				);

			return _stageTotal.Add(_penaltyTotal).ToString();
		}


		private IActionResult ParseResult(
			MotorsportResultAPI.Types.Enumeration.Results subject)
		{
			switch (subject)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/Competitor", "Competitor already exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.Created:
				default:
					return Created("", System.Net.HttpStatusCode.Created);
			}
		}


		private IActionResult ParseAppendResult(
			MotorsportResultAPI.Types.Enumeration.Results subject)
		{
			switch (subject)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult already exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "invalid time format"));
				case MotorsportResultAPI.Types.Enumeration.Results.Appended:
				default:
					return Created("", System.Net.HttpStatusCode.Created);
			}
		}


		private IActionResult ParseUpdateResult(
			MotorsportResultAPI.Types.Enumeration.Results subject)
		{
			switch (subject)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.DoesNotExist:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult does not exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "invalid time format"));
				case MotorsportResultAPI.Types.Enumeration.Results.MatchingElement:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult to be updated matches existing stageresult"));
				case MotorsportResultAPI.Types.Enumeration.Results.Updated:
				default:
					return Created("", System.Net.HttpStatusCode.Created);
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
	}
}