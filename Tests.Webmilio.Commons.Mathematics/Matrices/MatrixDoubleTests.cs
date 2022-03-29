using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webmilio.Commons.Mathematics.Matrices;

namespace Tests.Webmilio.Commons.Mathematics.Matrices;

[TestClass]
public class MatrixDoubleTests
{
    private const int Rows = 4, Columns = 6; // Make sure these are always up-to-date!
    private const double
        Element1_1 = 74.5, Element1_2 = 4, Element1_3 = 13,
        Element2_1 = 66, Element2_2 = 75.1, Element2_5 = 13,
        Element3_4 = 17.7;

    private double[,] _inner;
    private MatrixDouble _smallMatrix, _bigMatrix;

    [TestInitialize]
    public void Setup()
    {
        _smallMatrix = new(new double[,]
        {
            { 3,  7,  8 },
            { 9, 20,  1 },
            { 4,  3, 20 }
        });

        _inner = new[,]
        {
            { 1,     2,          3,          4,      5,       6 }, // Do not change, used in a for-loop for columns vs rows access.
            { 2, Element1_1, Element1_2, Element1_3, 8,      35 },
            { 3, Element2_1, Element2_2,    32,     35, Element2_5 },
            { 4,    79,         51,         12,  Element3_4,  1 }
        };
        _bigMatrix = new(_inner);
    }

    [TestMethod]
    public void ArrayConstructor()
    {
        Assert.AreEqual(Rows, _bigMatrix.Rows);
        Assert.AreEqual(Columns, _bigMatrix.Columns);
    }

    [TestMethod]
    public void SizeConstructor()
    {
        const int sizeRows = 10, sizeColumns = 5;

        var matrix = new MatrixDouble(sizeRows, sizeColumns);

        Assert.AreEqual(sizeRows, matrix.Rows);
        Assert.AreEqual(sizeColumns, matrix.Columns);
    }

    [TestMethod]
    public void Accessor() => Accessor(_bigMatrix);

    private static void Accessor(IMatrix<double> matrix)
    {
        for (int i = 0; i < matrix.Columns; i++)
            Assert.AreEqual(i + 1, matrix[0, i]);

        Assert.AreEqual(Element1_1, matrix[1, 1]);
        Assert.AreEqual(Element1_3, matrix[1, 3]);
        Assert.AreEqual(Element2_1, matrix[2, 1]);
        Assert.AreEqual(Element2_5, matrix[2, 5]);
        Assert.AreEqual(Element3_4, matrix[3, 4]);
    }

    [TestMethod]
    public void Clone()
    {
        var clone = _bigMatrix.Clone() as MatrixDouble;
        Accessor(clone);

        Assert.AreEqual(_bigMatrix.Determinant, clone.Determinant);

        clone[0, 0]++;
        Assert.AreNotEqual(clone[0, 0], _bigMatrix[0, 0]);
    }

    [TestMethod]
    public void Determinant()
    {
        Assert.AreEqual(-465, _smallMatrix.Determinant);
        Assert.AreEqual(double.NaN, _bigMatrix.Determinant);
    }

    [TestMethod]
    public void InnerMatrix()
    {
        for (int r = 0; r < _bigMatrix.Rows; r++)
            for (int c = 0; c < _bigMatrix.Columns; c++)
                Assert.AreEqual(_inner[r, c], _bigMatrix[r, c]);
    }

    [TestMethod]
    public void Invalidate()
    {
        var clone = _smallMatrix.Clone() as MatrixDouble;
        clone[0, 0]++;

        Assert.AreNotEqual(_smallMatrix.Determinant, clone.Determinant);
    }

    [TestMethod]
    public void Resize()
    {
        const int resizeRows = Rows * 2, resizeColumns = Columns / 2;

        var submatrix = _bigMatrix.Clone();
        submatrix.Resize(resizeRows, 0, resizeColumns, 0);

        Assert.AreEqual(resizeRows, submatrix.Rows);
        Assert.AreEqual(resizeColumns, submatrix.Columns);

        int count = 0;
        submatrix.Loop((_, _, _) => count++);

        Assert.AreEqual(resizeRows * resizeColumns, count);
    }

    [TestMethod]
    public void Resize_SmallerWithOffset()
    {
        const int resizeRows = 2, resizeColumns = 3;
        var submatrix = _bigMatrix.Clone();

        submatrix.Resize(resizeRows, 1, resizeColumns, 2);

        Assert.AreEqual(resizeRows, submatrix.Rows);
        Assert.AreEqual(resizeColumns, submatrix.Columns);

        Assert.AreEqual(Element1_2, submatrix[0, 0]);
        Assert.AreEqual(Element1_3, submatrix[0, 1]);
        Assert.AreEqual(Element2_2, submatrix[1, 0]);
    }

    [TestMethod]
    public void Resize_BiggerWithOffset()
    {
        const int resizeRows = 5, resizeColumns = 5;
        var submatrix = _bigMatrix.Clone();

        submatrix.Resize(resizeRows, 2, resizeColumns, 4);

        Assert.AreEqual(resizeRows, submatrix.Rows);
        Assert.AreEqual(resizeColumns, submatrix.Columns);

        Assert.AreEqual(Element2_5, submatrix[0, 1]);
        Assert.AreEqual(Element3_4, submatrix[1, 0]);
    }

    [TestMethod]
    public void Submatrix()
    {
        const int subRows = 2, subColumns = 3;

        var submatrix = _bigMatrix.Submatrix(subRows, 1, subColumns, 1);

        Assert.AreEqual(subRows, submatrix.Rows);
        Assert.AreEqual(subColumns, submatrix.Columns);

        Assert.AreEqual(Element1_1, submatrix[0, 0]);
        Assert.AreEqual(Element1_3, submatrix[0, 2]);
        Assert.AreEqual(Element2_1, submatrix[1, 0]);
    }
}
