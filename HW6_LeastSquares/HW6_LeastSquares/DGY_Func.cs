using System;
using System.Runtime.InteropServices;
using Emgu.CV;

#region for static, extension class
public static class MatExtension
{
    public static double GetValue(this Mat mat, int row, int col)
    {
        double[] value = new double[1];
        //Marshal.Copy(value, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
        Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
        return value[0];
    }
    public static void SetValue(this Mat mat, int row, int col, double value)
    {
        var target = new[] { value };
        Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
    }
}
#endregion