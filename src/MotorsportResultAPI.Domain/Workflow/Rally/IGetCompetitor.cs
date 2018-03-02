using System;

namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public interface IGetCompetitor
    {
        MotorsportResultAPI.Types.Domain.v1.Rally.Competitor Execute(
            string competitorId);
    }
}