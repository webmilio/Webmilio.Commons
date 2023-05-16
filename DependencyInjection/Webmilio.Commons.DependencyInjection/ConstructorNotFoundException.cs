namespace Webmilio.Commons.DependencyInjection;


[System.Serializable]
public class ConstructorMappingException : System.Exception
{
	public ConstructorMappingException() { }
	public ConstructorMappingException(string message) : base(message) { }
	public ConstructorMappingException(string message, System.Exception inner) : base(message, inner) { }

	protected ConstructorMappingException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}