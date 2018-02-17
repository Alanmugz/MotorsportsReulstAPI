using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Types.Data.v1.AutoCross
{
	public class Competitor
	{
		private readonly string c_id;
		private readonly string c_eventId;
		private readonly string c_carNumber;
		private readonly string c_name;
		private readonly string c_car;
		private readonly string c_category;
		private readonly List<MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult> c_stageResults;


		public string Id { get { return this.c_id; } }
		public string EventId { get { return this.c_eventId; } }
		public string CarNumber { get { return this.c_carNumber; } }
		public string Name { get { return this.c_name; } }
		public string Car { get { return this.c_car; } }
		public string Category { get { return this.c_category; } }
		public List<MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult> StageResults { get { return this.c_stageResults; } }


		public Competitor(
			string id,
			string eventId,
			string carNumber,
			string name,
			string car,
			string category,
			List<MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult> StageResults)
		{
			//DBC
			
			this.c_id = id;
			this.c_eventId = eventId;
			this.c_carNumber = carNumber;
			this.c_name = name;
			this.c_car = car;
			this.c_category = category;
			this.c_stageResults = StageResults;
		}
	}
}