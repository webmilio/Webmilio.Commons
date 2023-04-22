using System;

namespace Webmilio.Commons.Serialization;

public interface ISerializer<TInput, TOutput>
{
    public TInput Deserialize(TOutput input);

    public TOutput Serialize(TInput input);
}