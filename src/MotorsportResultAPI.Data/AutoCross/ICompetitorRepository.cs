using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Data.AutoCross
{
	public interface ICompetitorRepository
	{
		MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor FetchById(
			string id);


		IEnumerable<MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor> FetchByEventId(
			string eventId);


		MotorsportResultAPI.Types.Enumeration.Results Save(
			MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor subject);


		MotorsportResultAPI.Types.Enumeration.Results Append(
			int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult subject);


		MotorsportResultAPI.Types.Enumeration.Results Update(
			int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult subject);
	}
}
