using System;


namespace MotorsportResultAPI.Types.ExternalMessage.v1.Rally
{
	public class StageResult
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
			string PenaltyTime)
		{
			//DBC
			
			this.c_stageId = stageId;
			this.c_stageTime = stageTime;
			this.c_penaltyTime = PenaltyTime;
		}
	}
}