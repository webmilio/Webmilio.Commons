using System;
using System.Collections.Generic;
using System.Text;

namespace Webmilio.Commons.DependencyInjection;


[Serializable]
public class ServiceNotFoundException : Exception
{
	public ServiceNotFoundException() { }
	public ServiceNotFoundException(string message) : base(message) { }
	public ServiceNotFoundException(string message, Exception inner) : base(message, inner) { }
	protected ServiceNotFoundException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}