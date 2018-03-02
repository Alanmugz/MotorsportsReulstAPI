using System;

namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public interface IPostCompetitor
    {
        (MotorsportResultAPI.Types.Domain.v1.Rally.Competitor, MotorsportResultAPI.Types.Enumeration.Results) Execute(
            MotorsportResultAPI.Types.Domain.v1.Rally.Competitor subject);
    }
}