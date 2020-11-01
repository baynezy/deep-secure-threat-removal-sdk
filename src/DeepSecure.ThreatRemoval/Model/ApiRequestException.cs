using System;
using System.Net.Http;

namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// The Deep Secure Threat Removal API has returned a documented error response
	/// </summary>
	public class ApiRequestException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <c>ApiRequestException</c> class with a specified error message,
		/// the error response from the API and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="apiErrorResponse">The <c>ApiErrorResponse</c> returned from the API</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference
		/// (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ApiRequestException(string message, ApiErrorResponse apiErrorResponse, HttpRequestException innerException) : base(message, innerException)
		{
			ApiErrorResponse = apiErrorResponse;
		}

		public ApiErrorResponse ApiErrorResponse { get; }
	}
}