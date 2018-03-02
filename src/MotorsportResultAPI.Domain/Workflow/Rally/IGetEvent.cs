using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public interface IGetEvent
    {
        IEnumerable<MotorsportResultAPI.Types.ExternalMessage.v1.Rally.CompetitorResponse> Execute(
            string competitorId,
            int stageId);
    }
}