using System;


namespace MotorsportResultAPI.Types.Domain.v1.Rally
{
	public class PreviousResult
	{
		private readonly string c_carNumber;
		private readonly TimeSpan c_overallTime;

		
		public string CarNumber { get { return this.c_carNumber; } }
		public TimeSpan OverallTime { get { return this.c_overallTime; } }


		public PreviousResult(
			string carNumber,
			TimeSpan overallTime)
		{
			//DBC
			
			this.c_carNumber = carNumber;
			this.c_overallTime = overallTime;
		}
	}
}