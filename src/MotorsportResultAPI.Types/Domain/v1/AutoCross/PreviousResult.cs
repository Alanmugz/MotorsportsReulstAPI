using System;


namespace MotorsportResultAPI.Types.Domain.v1.AutoCross
{
	public class PreviousResult
	{
		private readonly string c_carNumber;
		private readonly string c_overallTime;

		
		public string CarNumber { get { return this.c_carNumber; } }
		public string OverallTime { get { return this.c_overallTime; } }


		public PreviousResult(
			string carNumber,
			string overallTime)
		{
			//DBC
			
			this.c_carNumber = carNumber;
			this.c_overallTime = overallTime;
		}
	}
}