namespace Webmilio.Commons.Mathematics;

public interface IScalarOperable<TSource, TScalar>
{
    public TSource Add(TScalar scalar);
    public TSource Subtract(TScalar scalar);
    public TSource Multiply(TScalar scalar);
    public TSource Divide(TScalar scalar);
}