//using log4net;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
//using System.Web.Http;
//using Check = CCS.Common.DBC.Check;


namespace MotorsportResultAPI.Public.Controllers.AutoCross
{
	//[MotorsportResultAPI.Public.Security.BasicAuthentication]
	[Route("autocross/v1")]
	public class StageResultController : Controller
	{
		//private readonly ILog c_logger;
		//private readonly MotorsportResultAPI.Data.AutoCross.ICompetitorRepository c_competitorRepository;


		//public StageResultController(
		//	ILog logger,
		//	MotorsportResultAPI.Data.AutoCross.ICompetitorRepository resultRepository)
		//{
		//	//Check.RequireArgumentNotNull("logger", logger);

		//	this.c_logger = logger;
		//	this.c_competitorRepository = resultRepository;
		//}


		//[HttpPost]
		//[Route("eventid/{eventid}/competitorid/{competitorid}/stageresult/append")]
		//public HttpResponseMessage Post(
		//	int eventId,
		//	int competitorid,
		//	MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult stageResult)
		//{
		//	var _loggingContext = string.Format("{0}.Post", this.GetType().FullName);
		//	this.c_logger.InfoFormat("{0} Commencing", _loggingContext);

		//	var _result = this.c_competitorRepository.Append(
		//		eventId,
		//		competitorid,
		//		stageResult);
		//	this.c_logger.InfoFormat("{0} Completed", _loggingContext);

		//	return this.ParseAppendResult(_result);
		//}


		//[HttpPut]
		//[Route("eventid/{eventid}/competitorid/{competitorid}/stageresult/update")]
		//public HttpResponseMessage Put(
		//	int eventId,
		//	int competitorid,
		//	MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult stageResult)
		//{
		//	var _loggingContext = string.Format("{0}.Post", this.GetType().FullName);
		//	this.c_logger.InfoFormat("{0} Commencing", _loggingContext);

		//	var _result = this.c_competitorRepository.Update(
		//		eventId,
		//		competitorid,
		//		stageResult);
		//	this.c_logger.InfoFormat("{0} Completed", _loggingContext);

		//	return this.ParseUpdateResult(_result);
		//}


		//private HttpResponseMessage ParseAppendResult(
		//	MotorsportResultAPI.Types.Enumeration.Results subject)
		//{
		//	switch (subject)
		//	{
		//		case MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists:
		//			return base.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
		//				new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult already exists"));
		//		case MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat:
		//			return base.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
		//				new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "invalid time format"));
		//		case MotorsportResultAPI.Types.Enumeration.Results.Appended:
		//		default:
		//			return base.Request.CreateResponse(System.Net.HttpStatusCode.Created);
		//	}
		//}


		//private HttpResponseMessage ParseUpdateResult(
		//	MotorsportResultAPI.Types.Enumeration.Results subject)
		//{
		//	switch (subject)
		//	{
		//		case MotorsportResultAPI.Types.Enumeration.Results.DoesNotExist:
		//			return base.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
		//				new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult does not exists"));
		//		case MotorsportResultAPI.Types.Enumeration.Results.InvalidTimeFormat:
		//			return base.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
		//				new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "invalid time format"));
		//		case MotorsportResultAPI.Types.Enumeration.Results.MatchingElement:
		//			return base.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
		//				new MotorsportResultAPI.Types.Domain.v1.Error("400", "autocross/v1/stageresult", "stageresult to be updated matches existing stageresult"));
		//		case MotorsportResultAPI.Types.Enumeration.Results.Updated:
		//		default:
		//			return base.Request.CreateResponse(System.Net.HttpStatusCode.Created);
		//	}
		//}
	}
}