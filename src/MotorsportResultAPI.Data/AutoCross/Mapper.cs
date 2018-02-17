using System;
using System.Collections.Generic;
using System.Linq;


namespace MotorsportResultAPI.Data.AutoCross
{
	public class Mapper
	{
		private readonly MotorsportResultAPI.Data.Helper.Transformer c_transformer;

 
		public Mapper()
		{
			this.c_transformer = new MotorsportResultAPI.Data.Helper.Transformer();
		}


		public MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult MapResultToData(
			MotorsportResultAPI.Types.Domain.v1.AutoCross.StageResult subject)
		{
			return new MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult(
				subject.StageId,
				this.c_transformer.ValidateTimeSpan(subject.StageTime),
				this.c_transformer.ValidateTimeSpan(subject.PenaltyTime));
		}


		public MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor MapCompetitorToData(
			MotorsportResultAPI.Types.Domain.v1.AutoCross.Competitor subject)
		{
			//TODO AM: Remove list
			return new MotorsportResultAPI.Types.Data.v1.AutoCross.Competitor(
				subject.Id,
				subject.EventId,
				subject.CarNumber,
				subject.Name,
				subject.Car,
				subject.Category,
				Enumerable.Empty<MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult>().ToList<MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult>());
		}
	}
}
