using System;

namespace Webmilio.Commons.Mathematics.Matrices;

public abstract class Matrix : IMatrix
{
    protected volatile object lck = new();

    protected Matrix(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
    }

    public void Resize(int rows, int columns) => Resize(rows, 0, columns, 0);
    public abstract void Resize(int rows, int originRow, int columns, int originColumn);

    protected virtual void Invalidate() { }

    public int Rows { get; protected set; }
    public int Columns { get; protected set; }
}

public class Matrix<T> : Matrix, IMatrix<T>
{
    public Matrix(int rows, int columns) : base(rows, columns)
    {
        InnerMatrix = new T[rows, columns];
    }

    public Matrix(T[,] matrix) : base(matrix.GetLength(0), matrix.GetLength(1))
    {
        InnerMatrix = matrix;
    }

    public override void Resize(int rows, int originRow, int columns, int originColumn)
    {
        if (originRow + rows <= 0 || originColumn + columns <= 0)
            throw new ArgumentOutOfRangeException("The combination of rows/columns and origin points must be bigger than 0.");

        lock (lck)
        {
            var previousRows = Rows;
            var previousColumns = Columns;

            Rows = rows;
            Columns = columns;

            var previous = InnerMatrix;
            InnerMatrix = new T[rows, columns];

            Loop(delegate (IMatrix<T> matrix, int row, int column)
            {
                if (row + originRow >= previousRows |
                    column + originColumn >= previousColumns)
                    return;

                matrix[row, column] = previous[row + originRow, column + originColumn];
            });
        }
    }

    public IMatrix<T> Submatrix(int rows, int columns) => Submatrix(rows, 0, columns, 0);

    public IMatrix<T> Submatrix(int rows, int sourceRow, int columns, int sourceColumn)
    {
        const string ErrorTemplate = "The addition of {0} and {1} must be a value between 0 and the upper corresponding dimension of the matrix.";

        if (sourceRow + rows < 0 || sourceRow + rows > Rows)
            throw new ArgumentOutOfRangeException(string.Format(ErrorTemplate, nameof(sourceColumn), nameof(columns)));

        if (sourceColumn + columns < 0 || sourceColumn + columns > Columns)
            throw new ArgumentOutOfRangeException(string.Format(ErrorTemplate, nameof(sourceRow), nameof(rows)));

        Matrix<T> submatrix = new(rows, columns);
        var source = this;

        submatrix.Loop(delegate (IMatrix<T> m, int r, int c)
        {
            m[r, c] = source[r + sourceRow, c + sourceColumn];
        });

        return submatrix;
    }

    public void Loop(IMatrix<T>.LoopDelegate action)
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Columns; c++)
                action(this, r, c);
    }

    public bool All(IMatrix<T>.LoopConditionDelegate condition)
    {
        for (int r = 0; r < Rows; r++)
        for (int c = 0; c < Columns; c++)
            if (!condition(this, r, c))
                return false;

        return true;
    }

    public bool Any(IMatrix<T>.LoopConditionDelegate condition)
    {
        for (int r = 0; r < Rows; r++)
        for (int c = 0; c < Columns; c++)
            if (condition(this, r, c))
                return true;

        return false;
    }

    public void Apply(IMatrix<T> matrix) => Apply(matrix, 0, 0);
    public void Apply(IMatrix<T> matrix, int destinationRow, int destinationColumn) => Apply(matrix, 0, destinationRow, 0, destinationColumn);

    public void Apply(IMatrix<T> matrix, int sourceRow, int destinationRow, int sourceColumn, int destinationColumn)
    {
        const string ErrorTemplate = "Parameter {0} or {1} is out of range; must be a positive integer within the boundaries of {2}. ";

        if (destinationColumn < 0 || destinationColumn >= Columns ||
            destinationRow < 0 || destinationRow >= Rows)
            throw new ArgumentOutOfRangeException(string.Format(ErrorTemplate, nameof(destinationColumn), nameof(destinationRow), "the target matrix"));

        if (sourceColumn < 0 || sourceColumn >= matrix.Columns ||
            sourceRow < 0 || sourceRow >= matrix.Rows)
            throw new ArgumentOutOfRangeException(string.Format(ErrorTemplate, nameof(sourceColumn), nameof(sourceRow), $"the provided {nameof(matrix)}"));

        if (matrix.Columns - sourceColumn + destinationColumn > Columns ||
            matrix.Rows - sourceRow + destinationRow > Rows)
            throw new ArgumentOutOfRangeException("The addition of destination and source arguments to the length of the supplied matrix " +
                                                  "must not result in a row index or column index outside the bounds of the target matrix.");
        Invalidate();

        Loop(delegate (IMatrix<T> m, int r, int c)
        {
            m[r + destinationRow, c + destinationColumn] = matrix.InnerMatrix[r + sourceRow, c + sourceColumn];
        });
    }

    public IMatrix<T> Inverse()
    {
        Matrix<T> inverse;

        lock (lck)
        {
            inverse = new(Rows, Columns);
            Array.Copy(InnerMatrix, inverse.InnerMatrix, InnerMatrix.Length);
        }

        return inverse;
    }

    public virtual IMatrix<T> Clone()
    {
        return new Matrix<T>(CloneInner());
    }

    protected T[,] CloneInner()
    {
        var inner = new T[Rows, Columns];
        Array.Copy(InnerMatrix, inner, InnerMatrix.Length);

        return inner;
    }

    public virtual T[,] InnerMatrix { get; private set; }

    public T this[int row, int column]
    {
        get => InnerMatrix[row, column];
        set
        {
            Invalidate();

            InnerMatrix[row, column] = value;
        }
    }
}