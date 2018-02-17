using System;


namespace MotorsportResultAPI.Data.Helper
{
	public class Transformer
	{
		public Transformer () {}

		
		public string ValidateTimeSpan(
			string str)
		{
			TimeSpan _timeSpan;
			if (!TimeSpan.TryParse(str, out _timeSpan))
			{
				return null;
			}

			return _timeSpan.ToString();
		}
	}
}
