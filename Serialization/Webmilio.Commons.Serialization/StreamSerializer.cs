namespace Webmilio.Commons.Serialization;

public class StreamSerializer<TStreamReader, TStreamWriter, TObject> :
    IStreamSerializer<TStreamReader, TStreamWriter, TObject>
{
    public delegate TObject Deserializer(TStreamReader stream);
    public delegate void Serializer(TStreamWriter stream, TObject data);

    private readonly Deserializer _deserializer;
    private readonly Serializer _serializer;

    public StreamSerializer(Deserializer deserializer, Serializer serializer)
    {
        _deserializer = deserializer;
        _serializer = serializer;
    }

    public TObject Deserialize(TStreamReader stream)
    {
        return _deserializer(stream);
    }

    public void Serialize(TStreamWriter stream, TObject data)
    {
        _serializer(stream, data);
    }
}