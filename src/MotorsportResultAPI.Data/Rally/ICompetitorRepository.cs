using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Data.Rally
{
	public interface ICompetitorRepository
	{
		MotorsportResultAPI.Types.Data.v1.Rally.Competitor FetchById(
			string id);


		IEnumerable<MotorsportResultAPI.Types.Domain.v1.Rally.Competitor> FetchByEventId(
			string eventId);


		void Save(
			MotorsportResultAPI.Types.Data.v1.Rally.Competitor subject);


		void Append(
			string id,
			MotorsportResultAPI.Types.Data.v1.Rally.Competitor competitor);


		void Update(
			string id,
			MotorsportResultAPI.Types.Data.v1.Rally.Competitor competitor);
	}
}
