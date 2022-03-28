using System;

namespace Webmilio.Commons.Mathematics.Matrices;

public interface IMatrix
{
    public void Resize(int rows, int originRow, int columns, int originColumn);

    public int Rows { get; }
    public int Columns { get; }
}

public interface IMatrix<T> : IMatrix, ICloneable<IMatrix<T>>
{
    public delegate void LoopDelegate(IMatrix<T> matrix, int row, int column);
    public delegate bool LoopConditionDelegate(IMatrix<T> matrix, int row, int column);

    public void Loop(LoopDelegate action);

    public IMatrix<T> Submatrix(int rows, int sourceRow, int columns, int sourceColumn);
    public void Apply(IMatrix<T> matrix, int sourceRow, int destinationRow, int sourceColumn, int destinationColumn);

    public IMatrix<T> Inverse();

    public T[,] InnerMatrix { get; }

    public T this[int row, int column] { get; set; }
}
