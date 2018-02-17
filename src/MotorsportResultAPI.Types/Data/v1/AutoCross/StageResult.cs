using System;


namespace MotorsportResultAPI.Types.Data.v1.AutoCross
{
	public class StageResult : IEquatable<MotorsportResultAPI.Types.Data.v1.AutoCross.StageResult>
	{
		private readonly int c_stageId;
		private readonly string c_stageTime;
		private readonly string c_penaltyTime;

		
		public int StageId { get { return this.c_stageId; } }
		public string StageTime { get { return this.c_stageTime; } }
		public string PenaltyTime { get { return this.c_penaltyTime; } }


		public StageResult(
			int stageId,
			string stageTime,
			string penaltyTime)
		{
			this.c_stageId = stageId;
			this.c_stageTime = stageTime;
			this.c_penaltyTime = penaltyTime;
		}

		public bool Equals(
			StageResult other)
		{
			if (other == null) return false;
			return (this.c_stageTime.Equals(other.StageTime) && this.c_penaltyTime.Equals(other.PenaltyTime));
		}
	}
}