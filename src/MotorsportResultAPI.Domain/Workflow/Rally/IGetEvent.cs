using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public interface IGetEvent
    {
        MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResultResponse Execute(
            string competitorId,
            int stageId);
    }
}