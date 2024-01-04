using System;

namespace Webmilio.Commons.DependencyInjection;


[Serializable]
public class ConstructorNotFoundException : Exception
{
	public ConstructorNotFoundException() { }
	public ConstructorNotFoundException(string message) : base(message) { }
	public ConstructorNotFoundException(string message, Exception inner) : base(message, inner) { }
	protected ConstructorNotFoundException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}