using System;

namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public interface IPostCompetitorAppend
    {
        (MotorsportResultAPI.Types.Domain.v1.Rally.Competitor, MotorsportResultAPI.Types.Enumeration.Results) Execute(
            int eventId,
			int competitorid,
			MotorsportResultAPI.Types.Domain.v1.Rally.StageResult stageResult);
    }
}