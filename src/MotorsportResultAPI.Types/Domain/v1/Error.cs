using System;


namespace MotorsportResultAPI.Types.Domain.v1
{
	public class Error
	{
		private readonly string c_resultCode;
		private readonly string c_source;
		private readonly string c_message;


		public string ResultCode { get { return this.c_resultCode; } }
		public string Source { get { return this.c_source; } }
		public string Message { get { return this.c_message; } }


		public Error(
			string resultCode,
			string source,
			string message)
		{
			//DBC

			this.c_resultCode = resultCode;
			this.c_source = source;
			this.c_message = message;
		}
	}
}
