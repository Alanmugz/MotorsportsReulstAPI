using System;


namespace MotorsportResultAPI.Data.Helper
{
	public class Transformer
	{
		public Transformer () {}

		
		public string ValidateTimeSpan(
			TimeSpan subject)
		{
			TimeSpan _timeSpan;
			if (!TimeSpan.TryParse(subject.ToString(), out _timeSpan))
			{
				return null;
			}

			return _timeSpan.ToString();
		}
	}
}
