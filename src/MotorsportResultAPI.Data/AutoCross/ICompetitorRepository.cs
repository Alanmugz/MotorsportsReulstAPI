using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Data.AutoCross
{
	public interface ICompetitorRepository
	{
		MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor FetchById(
			string id);


		IEnumerable<MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor> FetchByEventId(
			string eventId);


		(MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) Save(
			MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor subject);


		(MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) Append(
			int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult subject);


		(MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor ,MotorsportResultAPI.Types.Enumeration.Results) Update(
			int eventId,
			int competitorId,
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult subject);
	}
}
