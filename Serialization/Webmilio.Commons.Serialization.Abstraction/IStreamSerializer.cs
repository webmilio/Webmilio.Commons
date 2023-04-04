using System;

namespace Webmilio.Commons.Serialization;

public interface IStreamSerializer<TStreamReader, TStreamWriter, TObject>
{
    public TObject Deserialize(TStreamReader stream);
    public void Serialize(TStreamWriter stream, TObject data);
}