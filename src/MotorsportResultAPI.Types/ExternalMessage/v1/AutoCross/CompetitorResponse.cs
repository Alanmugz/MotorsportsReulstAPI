using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Types.ExternalMessage.v1.AutoCross
{
	public class CompetitorResponse
	{
		private readonly int? c_position;
		private readonly string c_carNumber;
		private readonly string c_name;
		private readonly string c_car;
		private readonly string c_category;
		private readonly string c_overallTime;
		private readonly string c_stageTime;
		private readonly string c_penaltyTime;
		private readonly string c_differenceToLeader;
		private readonly string c_differenceToPrevious;
		private readonly string c_previouStagePosition;


		public int? Position { get { return this.c_position; } }
		public string CarNumber { get { return this.c_carNumber; } }
		public string Name { get { return this.c_name; } }
		public string Car { get { return this.c_car; } }
		public string Category { get { return this.c_category; } }
		public string OverallTime { get { return this.c_overallTime; } }
		public string StageTime { get { return this.c_stageTime; } }
		public string PenaltyTime { get { return this.c_penaltyTime; } }
		public string DifferenceToLeader { get { return this.c_differenceToLeader; } }
		public string differenceToPrevious { get { return this.c_differenceToPrevious; } }
		public string PreviousStagePosition { get { return this.c_previouStagePosition; } }


		public CompetitorResponse(
			int position,
			string carNumber,
			string name,
			string car,
			string category,
			string overallTime,
			string stageTime,
			string penaltyTime,
			string DifferenceToLeader,
			string differenceToPrevious,
			string previousStagePosition)
		{
			//DBC
			
			this.c_position = position;
			this.c_carNumber = carNumber;
			this.c_name = name;
			this.c_car = car;
			this.c_category = category;
			this.c_overallTime = overallTime;
			this.c_stageTime = stageTime;
			this.c_penaltyTime = penaltyTime;
			this.c_differenceToLeader = DifferenceToLeader;
			this.c_differenceToPrevious = differenceToPrevious;
			this.c_previouStagePosition = previousStagePosition;
		}
	}
}