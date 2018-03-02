using System;

namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public interface IPutCompetitorUpdate
    {
        (MotorsportResultAPI.Types.Domain.v1.Rally.Competitor, MotorsportResultAPI.Types.Enumeration.Results) Execute(
            string eventId,
            string competitorId,
            MotorsportResultAPI.Types.Domain.v1.Rally.StageResult stageResult);
    }
}