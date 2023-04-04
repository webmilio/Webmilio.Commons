namespace Webmilio.Commons.Serialization;

public class FunctionSerializer<TInput, TOutput> : ISerializer<TInput, TOutput>
{
    public delegate TInput DeserializingMethod(TOutput data);
    public delegate TOutput SerializingMethod(TInput data);

    private readonly DeserializingMethod _deserializingMethod;
    private readonly SerializingMethod _serializingMethod;

    public FunctionSerializer(DeserializingMethod deserializingMethod, SerializingMethod serializingMethod)
    {
        _deserializingMethod = deserializingMethod;
        _serializingMethod = serializingMethod;
    }

    public TInput Deserialize(TOutput data)
    {
        return _deserializingMethod(data);
    }

    public TOutput Serialize(TInput data)
    {
        return _serializingMethod(data);
    }
}
