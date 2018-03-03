using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Types.ExternalMessage.v1.Rally
{
	public class CompetitorResultResponse
	{
		private IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorStageResult> c_competitorStageResult;
		private IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorOverallResult> c_competitorOverallResult;


		public IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorStageResult> CompetitorStageResult { get { return this.c_competitorStageResult; } }
		public IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorOverallResult> CompetitorOverallResult { get { return this.c_competitorOverallResult; } }


		public CompetitorResultResponse(
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorStageResult> competitorStageResult,
			IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.CompetitorOverallResult> competitorOverallResult)
		{
			//DBC
			
			this.c_competitorStageResult = competitorStageResult;
			this.c_competitorOverallResult = competitorOverallResult;
		}
	}
}