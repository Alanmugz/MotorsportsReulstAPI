using System;
using System.Collections.Generic;


namespace MotorsportResultAPI.Types.Domain.v1.Rally
{
	public class CompetitorStageResult
	{
		private readonly int? c_position;
		private readonly string c_carNumber;
		private readonly string c_name;
		private readonly string c_car;
		private readonly string c_category;
		private readonly string c_stageTime;
		private readonly string c_differenceToLeader;
		private readonly string c_differenceToPrevious;


		public int? Position { get { return this.c_position; } }
		public string CarNumber { get { return this.c_carNumber; } }
		public string Name { get { return this.c_name; } }
		public string Car { get { return this.c_car; } }
		public string Category { get { return this.c_category; } }
		public string StageTime { get { return this.c_stageTime; } }
		public string DifferenceToLeader { get { return this.c_differenceToLeader; } }
		public string differenceToPrevious { get { return this.c_differenceToPrevious; } }
		


		public CompetitorStageResult(
			int position,
			string carNumber,
			string name,
			string car,
			string category,
			TimeSpan stageTime,
			TimeSpan differenceToLeader,
			TimeSpan differenceToPrevious)
		{
			//DBC
			
			this.c_position = position;
			this.c_carNumber = carNumber;
			this.c_name = name;
			this.c_car = car;
			this.c_category = category;
			this.c_stageTime = new DateTime(stageTime.Ticks).ToString("HH:mm:ss.f");
			this.c_differenceToLeader = new DateTime(differenceToLeader.Ticks).ToString("HH:mm:ss.f");
			this.c_differenceToPrevious = new DateTime(differenceToPrevious.Ticks).ToString("HH:mm:ss.f");
		}
	}
}