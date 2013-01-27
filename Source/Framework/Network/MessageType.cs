using System;

namespace Pegasus.Framework.Network
{
	/// <summary>
	///   Describes the type of a message sent over the network.
	/// </summary>
	internal enum MessageType : byte
	{
		/// <summary>
		///   Indicates that the client wanted to use a different service or a different version of the service.
		/// </summary>
		ServiceIdentifierMismatch,

		/// <summary>
		///   Indicates that the client sent a message of an invalid or unexpected type.
		/// </summary>
		InvalidMessageType,

		/// <summary>
		///   Indicates that the message is an operation invocation requested by the client.
		/// </summary>
		OperationCall,

		/// <summary>
		///   Indicates that the message contains the result of an operation invocation.
		/// </summary>
		OperationResult,

		/// <summary>
		///   Indicates that the message contains the exception that was raised by an operation.
		/// </summary>
		OperationException,

		/// <summary>
		///   Indicates that the message is a callback invocation requested by the server.
		/// </summary>
		CallbackCall,

		/// <summary>
		///   Indicates that the message contains the result of a callback invocation.
		/// </summary>
		CallbackResult,

		/// <summary>
		///   Indicates that the message contains the exception that was raised by a callback.
		/// </summary>
		CallbackException
	}
}