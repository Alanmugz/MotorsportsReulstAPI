using System;


namespace MotorsportResultAPI.Types.Domain.v1.AutoCross
{
	public class StageResult
	{
		private readonly int c_stageId;
		private readonly TimeSpan c_stageTime;
		private readonly TimeSpan c_penaltyTime;

		
		public int StageId { get { return this.c_stageId; } }
		public TimeSpan StageTime { get { return this.c_stageTime; } }
		public TimeSpan PenaltyTime { get { return this.c_penaltyTime; } }


		public StageResult(
			int stageId,
			TimeSpan stageTime,
			TimeSpan PenaltyTime)
		{
			//DBC
			
			this.c_stageId = stageId;
			this.c_stageTime = stageTime;
			this.c_penaltyTime = PenaltyTime;
		}
	}
}