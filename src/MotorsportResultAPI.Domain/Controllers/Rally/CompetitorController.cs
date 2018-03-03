using Microsoft.AspNetCore.Mvc;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 
namespace MotorsportResultAPI.Domain.Controllers.Rally
{
	//[MotorsportResultAPI.Public.Security.BasicAuthentication]
	[Route("rally/v1")]
	public class CompetitorController : Controller
	{
		private readonly NLog.ILogger c_logger;
		private MotorsportResultAPI.Data.Rally.ICompetitorRepository c_competitorRepository;
		private MotorsportResultAPI.Domain.Workflow.Rally.IGetCompetitor c_getCompetitorWorkflow;
		private MotorsportResultAPI.Domain.Workflow.Rally.IGetEvent c_getEventWorkflow;
		private MotorsportResultAPI.Domain.Workflow.Rally.IPostCompetitor c_postCompetitorWorkflow;
		private MotorsportResultAPI.Domain.Workflow.Rally.IPostCompetitorAppend c_postCompetitorAppendWorkflow;
		private MotorsportResultAPI.Domain.Workflow.Rally.IPutCompetitorUpdate c_putCompetitorUpdateWorkflow;


		public CompetitorController(
			NLog.ILogger logger,
			MotorsportResultAPI.Data.Rally.ICompetitorRepository competitorRepository,
			MotorsportResultAPI.Domain.Workflow.Rally.IGetCompetitor getCompetitorWorkflow,
			MotorsportResultAPI.Domain.Workflow.Rally.IGetEvent getEventWorkflow,
			MotorsportResultAPI.Domain.Workflow.Rally.IPostCompetitor postCompetitorWorkflow,
			MotorsportResultAPI.Domain.Workflow.Rally.IPostCompetitorAppend postCompetitorAppendWorkflow,
			MotorsportResultAPI.Domain.Workflow.Rally.IPutCompetitorUpdate putCompetitorUpdateWorkflow)
		{
			this.c_logger = logger;
			this.c_competitorRepository = competitorRepository;
			this.c_getCompetitorWorkflow = getCompetitorWorkflow;
			this.c_getEventWorkflow = getEventWorkflow;
			this.c_postCompetitorWorkflow = postCompetitorWorkflow;
			this.c_postCompetitorAppendWorkflow = postCompetitorAppendWorkflow;
			this.c_putCompetitorUpdateWorkflow = putCompetitorUpdateWorkflow;
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
			var _result = this.c_getCompetitorWorkflow.Execute(_competitorId);

			return this.ParseGetCompetitorResult(_result);
		}


		[HttpGet]
		[Route("event")]
		public IActionResult GetEvent()
		{
			var eventId = HttpContext.Request.Query["id"].ToString();
			var stageId = Convert.ToInt32(HttpContext.Request.Query["stageId"]);

			var _loggingContext = string.Format("{0}.GetEvent", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _result = this.c_getEventWorkflow.Execute(eventId.ToString(), stageId);

			return this.ParseGetEventResult(_result);
		}


		[HttpPost]
		[Route("eventid/{eventid}/competitor")]
		public IActionResult PostCompetitor(
			[FromRoute] int eventId,
			[FromBody] MotorsportResultAPI.Types.ExternalMessage.v1.Rally.Competitor competitor)
		{
			var _loggingContext = string.Format("{0}.PostCompetitor", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _domainCompetitor = this.MapToDomainCompetitor(eventId, competitor);
			var _result = this.c_postCompetitorWorkflow.Execute(_domainCompetitor);

			return this.ParseResult(_result, eventId, competitor.CarNumber);
		}


		[HttpPost]
		[Route("eventid/{eventid}/competitorid/{competitorid}/stageresult/append")]
		public IActionResult PostAppend(
			[FromRoute] int eventId,
			[FromRoute] int competitorid,
			[FromBody] MotorsportResultAPI.Types.ExternalMessage.v1.Rally.StageResult stageResult)
		{
			var _loggingContext = string.Format("{0}.PostAppend", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _domainStageResult = this.MapToDomainStageResult(stageResult);
			var _result = this.c_postCompetitorAppendWorkflow.Execute(eventId, competitorid, _domainStageResult);

			return this.ParseAppendResult(_result, eventId, competitorid);
		}


		[HttpPut]
		[Route("eventid/{eventid}/competitorid/{competitorid}/stageresult/update")]
		public IActionResult PutUpdate(
			[FromRoute] int eventId,
			[FromRoute] int competitorid,
			[FromBody] MotorsportResultAPI.Types.ExternalMessage.v1.Rally.StageResult stageResult)
		{
			var _loggingContext = string.Format("{0}.PutUpdate", this.GetType().FullName);
			this.c_logger.Info("{0} Commencing", _loggingContext);

			var _domainStageResult = this.MapToDomainStageResult(stageResult);
			var _result = this.c_putCompetitorUpdateWorkflow.Execute(eventId.ToString(), competitorid.ToString(), _domainStageResult);
			
			return this.ParseUpdateResult(_result, eventId, competitorid);
		}


		private IActionResult ParseGetCompetitorResult(
			MotorsportResultAPI.Types.Domain.v1.Rally.Competitor subject)
		{
			switch (subject)
			{
				case null:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/Competitor", "Competitor doesn't exists"));
				default:
					return Ok(subject);
			}
		}


		private IActionResult ParseGetEventResult(
			MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResultResponse subject)
		{
			switch (subject)
			{
				case null:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/Competitor", "Event doesn't exists"));
				default:
					return Ok(subject);
			}
		}


		private IActionResult ParseResult(
			(MotorsportResultAPI.Types.Domain.v1.Rally.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) result,
			int eventId,
			string carNumber)
		{
			switch (result.Item2)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/Competitor", "Competitor already exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.Created:
				default:
					var _uri = $"{this.Request.Scheme}://{this.Request.Host}/Rally/v1/competitor?carnumber={carNumber}&eventid={eventId}";
					return Created(_uri, result.Item1);
			}
		}


		private IActionResult ParseAppendResult(
			(MotorsportResultAPI.Types.Domain.v1.Rally.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) result,
			int eventId,
			int carNumber)
		{
			switch (result.Item2)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/stageresult", "stageresult already exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/stageresult", "invalid time format"));
				case MotorsportResultAPI.Types.Enumeration.Results.PreviousStageResultDoesNotExist:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/stageresult", "previous stage result does not exist"));
				case MotorsportResultAPI.Types.Enumeration.Results.Appended:
				default:
					var _uri = $"{this.Request.Scheme}://{this.Request.Host}/Rally/v1/competitor?carnumber={carNumber}&eventid={eventId}";
					return Created(_uri, result.Item1);
			}
		}


		private IActionResult ParseUpdateResult(
			(MotorsportResultAPI.Types.Domain.v1.Rally.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) result,
			int eventId,
			int carNumber)
		{
			switch (result.Item2)
			{
				case MotorsportResultAPI.Types.Enumeration.Results.DoesNotExist:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/stageresult", "stageresult does not exists"));
				case MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/stageresult", "invalid time format"));
				case MotorsportResultAPI.Types.Enumeration.Results.MatchingElement:
					return BadRequest(new MotorsportResultAPI.Types.Domain.v1.Error("400", "Rally/v1/stageresult", "stageresult to be updated matches existing stageresult"));
				case MotorsportResultAPI.Types.Enumeration.Results.Updated:
				default:
					var _uri = $"{this.Request.Scheme}://{this.Request.Host}/Rally/v1/competitor?carnumber={carNumber}&eventid={eventId}";
					return Created(_uri, result.Item1);
			}
		}


		private MotorsportResultAPI.Types.Domain.v1.Rally.Competitor MapToDomainCompetitor(
			int eventId,
			MotorsportResultAPI.Types.ExternalMessage.v1.Rally.Competitor competitor)
		{
			return new MotorsportResultAPI.Types.Domain.v1.Rally.Competitor(
				$"{eventId}-{competitor.CarNumber}",
				eventId.ToString(),
				competitor.CarNumber,
				competitor.Name,
				competitor.Car,
				competitor.Category,
				competitor.StageResults);
		}


		private MotorsportResultAPI.Types.Domain.v1.Rally.StageResult MapToDomainStageResult(
			MotorsportResultAPI.Types.ExternalMessage.v1.Rally.StageResult stageResult)
		{
			return new MotorsportResultAPI.Types.Domain.v1.Rally.StageResult(
				stageResult.StageId,
				TimeSpan.Parse(stageResult.StageTime),
				TimeSpan.Parse(stageResult.PenaltyTime));
		}
	}
}