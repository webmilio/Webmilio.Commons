namespace Webmilio.Commons.Mathematics.Matrices;

public class MatrixDouble : Matrix<double>
{
    public MatrixDouble(int rows, int columns) : base(rows, columns) { }

    public MatrixDouble(double[,] matrix) : base(matrix) { }

    protected override void Invalidate()
    {
        _determinant = null;
    }

    public override IMatrix<double> Clone()
    {
        MatrixDouble clone = new(CloneInner())
        {
            _determinant = _determinant
        };

        return clone;
    }

    private double? _determinant;
    public double Determinant
    {
        get
        {
            lock (lck)
            {
                if (_determinant.HasValue)
                    return _determinant.Value;

                if (Rows != Columns)
                    return double.NaN;

                if (Rows == 1)
                    return this[0, 0];

                // Calculate and Store.
                double sum = 0;

                int sign = 1;
                int upperBound = Columns - 1;

                for (int c = 0; !(c == Columns && sign < 0); c++)
                {
                    double tmp = sign;

                    for (int i = 0; i < Rows; i++)
                    {
                        int nC = c + i * sign;

                        if (nC >= Columns)
                            nC -= Columns;
                        else if (nC < 0)
                            nC += Columns;

                        tmp *= this[i, nC];
                    }

                    sum += tmp;

                    if (c == upperBound && sign > 0)
                    {
                        c = -1;
                        sign = -1;
                    }
                }

                _determinant = sum;
                return sum;
            }
        }
    }
}
