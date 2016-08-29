//using System;
using UnityEngine;

public class cNoise
{
    private int m_Seed;

    public cNoise(int a_Seed)
    {
        m_Seed = a_Seed;
    }

    public cNoise(cNoise a_Noise)
    {
        m_Seed = a_Noise.m_Seed;
    }

    public float IntNoise1D(int a_X)
    {
        int x = ((a_X * m_Seed) << 13) ^ a_X;
        return (1f - (float)((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);
    }

    public float IntNoise2D(int a_X, int a_Y)
    {
        int n = a_X + a_Y * 57 + m_Seed * 57 * 57;
        n = (n << 13) ^ n;
        return (1f - (float)((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);
    }

    public float IntNoise3D(int a_X, int a_Y, int a_Z)
    {
        int n = a_X + a_Y * 57 + a_Z * 57 * 57 + m_Seed * 57 * 57 * 57;
        n = (n << 13) ^ n;
        return (1f - (float)((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);
    }

    public float IntNoise2DInRange(int a_X, int a_Y, float a_Min, float a_Max)
    {
        return a_Min + Mathf.Abs(IntNoise2D(a_X, a_Y)) * (a_Max - a_Min);
    }

    public int IntNoise1DInt(int a_X)
    {
        int x = ((a_X * m_Seed) << 13) ^ a_X;
        return ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff);
    }

    public int IntNoise2DInt(int a_X, int a_Y)
    {
        int n = a_X + a_Y * 57 + m_Seed * 57 * 57;
        n = (n << 13) ^ n;
        return ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff);
    }

    public int IntNoise3DInt(int a_X, int a_Y, int a_Z)
    {
        int n = a_X + a_Y * 57 + a_Z * 57 * 57 + m_Seed * 57 * 57 * 57;
        n = (n << 13) ^ n;
        return ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff);
    }

    public float LinearNoise1D(float a_X)
    {
        int BaseX = a_X < 0 ? (int)a_X - 1 : (int)a_X;
        float FracX = a_X - BaseX;
        return LinearInterpolate(IntNoise1D(BaseX), IntNoise1D(BaseX + 1), FracX);
    }

    public float CosineNoise1D(float a_X)
    {
        int BaseX = a_X < 0 ? (int)a_X - 1 : (int)a_X;
        float FracX = a_X - BaseX;
        return CosineInterpolate(IntNoise1D(BaseX), IntNoise1D(BaseX + 1), FracX);
    }

    public float CubicNoise1D(float a_X)
    {
        int BaseX = a_X < 0 ? (int)a_X - 1 : (int)a_X;
        float FracX = a_X - BaseX;
        return CubicInterpolate(IntNoise1D(BaseX - 1), IntNoise1D(BaseX), IntNoise1D(BaseX + 1), IntNoise1D(BaseX + 2), FracX);
    }

    public float SmoothNoise1D(int a_X)
    {
        return IntNoise1D(a_X) / 2 + IntNoise1D(a_X - 1) / 4 + IntNoise1D(a_X + 1) / 4;
    }

    public float CubicNoise2D(float a_X, float a_Y)
    {
        int BaseX = a_X < 0 ? (int)a_X - 1 : (int)a_X,
            BaseY = a_Y < 0 ? (int)a_Y - 1 : (int)a_Y;

        float[,] points = { { IntNoise2D(BaseX - 1, BaseY - 1), IntNoise2D(BaseX, BaseY - 1), IntNoise2D(BaseX + 1, BaseY - 1), IntNoise2D(BaseX + 2, BaseY - 1) }, { IntNoise2D(BaseX - 1, BaseY), IntNoise2D(BaseX, BaseY), IntNoise2D(BaseX + 1, BaseY), IntNoise2D(BaseX + 2, BaseY) }, { IntNoise2D(BaseX - 1, BaseY + 1), IntNoise2D(BaseX, BaseY + 1), IntNoise2D(BaseX + 1, BaseY + 1), IntNoise2D(BaseX + 2, BaseY + 1) }, { IntNoise2D(BaseX - 1, BaseY + 2), IntNoise2D(BaseX, BaseY + 2), IntNoise2D(BaseX + 1, BaseY + 2), IntNoise2D(BaseX + 2, BaseY + 2) } };

        float FracX = a_X - BaseX,
              interp1 = CubicInterpolate(points[0, 0], points[0, 1], points[0, 2], points[0, 3], FracX),
              interp2 = CubicInterpolate(points[1, 0], points[1, 1], points[1, 2], points[1, 3], FracX),
              interp3 = CubicInterpolate(points[2, 0], points[2, 1], points[2, 2], points[2, 3], FracX),
              interp4 = CubicInterpolate(points[3, 0], points[3, 1], points[3, 2], points[3, 3], FracX),

              FracY = a_Y - BaseY;
        return CubicInterpolate(interp1, interp2, interp3, interp4, FracY);
    }

    public float CubicNoise3D(float a_X, float a_Y, float a_Z)
    {
        int BaseX = a_X < 0 ? (int)a_X - 1 : (int)a_X,
            BaseY = a_Y < 0 ? (int)a_Y - 1 : (int)a_Y,
            BaseZ = a_Z < 0 ? (int)a_Z - 1 : (int)a_Z;

        float[,] points1 = { { IntNoise3D(BaseX - 1, BaseY - 1, BaseZ - 1), IntNoise3D(BaseX, BaseY - 1, BaseZ - 1), IntNoise3D(BaseX + 1, BaseY - 1, BaseZ - 1), IntNoise3D(BaseX + 2, BaseY - 1, BaseZ - 1) }, { IntNoise3D(BaseX - 1, BaseY, BaseZ - 1), IntNoise3D(BaseX, BaseY, BaseZ - 1), IntNoise3D(BaseX + 1, BaseY, BaseZ - 1), IntNoise3D(BaseX + 2, BaseY, BaseZ - 1) }, { IntNoise3D(BaseX - 1, BaseY + 1, BaseZ - 1), IntNoise3D(BaseX, BaseY + 1, BaseZ - 1), IntNoise3D(BaseX + 1, BaseY + 1, BaseZ - 1), IntNoise3D(BaseX + 2, BaseY + 1, BaseZ - 1) }, { IntNoise3D(BaseX - 1, BaseY + 2, BaseZ - 1), IntNoise3D(BaseX, BaseY + 2, BaseZ - 1), IntNoise3D(BaseX + 1, BaseY + 2, BaseZ - 1), IntNoise3D(BaseX + 2, BaseY + 2, BaseZ - 1) } };

        float FracX = a_X - BaseX,
              x1interp1 = CubicInterpolate(points1[0, 0], points1[0, 1], points1[0, 2], points1[0, 3], FracX),
              x1interp2 = CubicInterpolate(points1[1, 0], points1[1, 1], points1[1, 2], points1[1, 3], FracX),
              x1interp3 = CubicInterpolate(points1[2, 0], points1[2, 1], points1[2, 2], points1[2, 3], FracX),
              x1interp4 = CubicInterpolate(points1[3, 0], points1[3, 1], points1[3, 2], points1[3, 3], FracX);

        float[,] points2 = { { IntNoise3D(BaseX - 1, BaseY - 1, BaseZ), IntNoise3D(BaseX, BaseY - 1, BaseZ), IntNoise3D(BaseX + 1, BaseY - 1, BaseZ), IntNoise3D(BaseX + 2, BaseY - 1, BaseZ) }, { IntNoise3D(BaseX - 1, BaseY, BaseZ), IntNoise3D(BaseX, BaseY, BaseZ), IntNoise3D(BaseX + 1, BaseY, BaseZ), IntNoise3D(BaseX + 2, BaseY, BaseZ) }, { IntNoise3D(BaseX - 1, BaseY + 1, BaseZ), IntNoise3D(BaseX, BaseY + 1, BaseZ), IntNoise3D(BaseX + 1, BaseY + 1, BaseZ), IntNoise3D(BaseX + 2, BaseY + 1, BaseZ) }, { IntNoise3D(BaseX - 1, BaseY + 2, BaseZ), IntNoise3D(BaseX, BaseY + 2, BaseZ), IntNoise3D(BaseX + 1, BaseY + 2, BaseZ), IntNoise3D(BaseX + 2, BaseY + 2, BaseZ) } };

        float x2interp1 = CubicInterpolate(points2[0, 0], points2[0, 1], points2[0, 2], points2[0, 3], FracX),
              x2interp2 = CubicInterpolate(points2[1, 0], points2[1, 1], points2[1, 2], points2[1, 3], FracX),
              x2interp3 = CubicInterpolate(points2[2, 0], points2[2, 1], points2[2, 2], points2[2, 3], FracX),
              x2interp4 = CubicInterpolate(points2[3, 0], points2[3, 1], points2[3, 2], points2[3, 3], FracX);

        float[,] points3 = { { IntNoise3D(BaseX - 1, BaseY - 1, BaseZ + 1), IntNoise3D(BaseX, BaseY - 1, BaseZ + 1), IntNoise3D(BaseX + 1, BaseY - 1, BaseZ + 1), IntNoise3D(BaseX + 2, BaseY - 1, BaseZ + 1) }, { IntNoise3D(BaseX - 1, BaseY, BaseZ + 1), IntNoise3D(BaseX, BaseY, BaseZ + 1), IntNoise3D(BaseX + 1, BaseY, BaseZ + 1), IntNoise3D(BaseX + 2, BaseY, BaseZ + 1) }, { IntNoise3D(BaseX - 1, BaseY + 1, BaseZ + 1), IntNoise3D(BaseX, BaseY + 1, BaseZ + 1), IntNoise3D(BaseX + 1, BaseY + 1, BaseZ + 1), IntNoise3D(BaseX + 2, BaseY + 1, BaseZ + 1) }, { IntNoise3D(BaseX - 1, BaseY + 2, BaseZ + 1), IntNoise3D(BaseX, BaseY + 2, BaseZ + 1), IntNoise3D(BaseX + 1, BaseY + 2, BaseZ + 1), IntNoise3D(BaseX + 2, BaseY + 2, BaseZ + 1) } };

        float x3interp1 = CubicInterpolate(points3[0, 0], points3[0, 1], points3[0, 2], points3[0, 3], FracX),
              x3interp2 = CubicInterpolate(points3[1, 0], points3[1, 1], points3[1, 2], points3[1, 3], FracX),
              x3interp3 = CubicInterpolate(points3[2, 0], points3[2, 1], points3[2, 2], points3[2, 3], FracX),
              x3interp4 = CubicInterpolate(points3[3, 0], points3[3, 1], points3[3, 2], points3[3, 3], FracX);

        float[,] points4 = { { IntNoise3D(BaseX - 1, BaseY - 1, BaseZ + 2), IntNoise3D(BaseX, BaseY - 1, BaseZ + 2), IntNoise3D(BaseX + 1, BaseY - 1, BaseZ + 2), IntNoise3D(BaseX + 2, BaseY - 1, BaseZ + 2) }, { IntNoise3D(BaseX - 1, BaseY, BaseZ + 2), IntNoise3D(BaseX, BaseY, BaseZ + 2), IntNoise3D(BaseX + 1, BaseY, BaseZ + 2), IntNoise3D(BaseX + 2, BaseY, BaseZ + 2) }, { IntNoise3D(BaseX - 1, BaseY + 1, BaseZ + 2), IntNoise3D(BaseX, BaseY + 1, BaseZ + 2), IntNoise3D(BaseX + 1, BaseY + 1, BaseZ + 2), IntNoise3D(BaseX + 2, BaseY + 1, BaseZ + 2) }, { IntNoise3D(BaseX - 1, BaseY + 2, BaseZ + 2), IntNoise3D(BaseX, BaseY + 2, BaseZ + 2), IntNoise3D(BaseX + 1, BaseY + 2, BaseZ + 2), IntNoise3D(BaseX + 2, BaseY + 2, BaseZ + 2) } };

        float x4interp1 = CubicInterpolate(points4[0, 0], points4[0, 1], points4[0, 2], points4[0, 3], FracX),
              x4interp2 = CubicInterpolate(points4[1, 0], points4[1, 1], points4[1, 2], points4[1, 3], FracX),
              x4interp3 = CubicInterpolate(points4[2, 0], points4[2, 1], points4[2, 2], points4[2, 3], FracX),
              x4interp4 = CubicInterpolate(points4[3, 0], points4[3, 1], points4[3, 2], points4[3, 3], FracX);

        float FracY = a_Y - BaseY,
              yinterp1 = CubicInterpolate(x1interp1, x1interp2, x1interp3, x1interp4, FracY),
              yinterp2 = CubicInterpolate(x2interp1, x2interp2, x2interp3, x2interp4, FracY),
              yinterp3 = CubicInterpolate(x3interp1, x3interp2, x3interp3, x3interp4, FracY),
              yinterp4 = CubicInterpolate(x4interp1, x4interp2, x4interp3, x4interp4, FracY),

              FracZ = (a_Z) - BaseZ;
        return CubicInterpolate(yinterp1, yinterp2, yinterp3, yinterp4, FracZ);
    }

    public void SetSeed(int a_Seed)
    {
        m_Seed = a_Seed;
    }

    public static float CubicInterpolate(float a_A, float a_B, float a_C, float a_D, float a_Pct)
    {
        float P = (a_D - a_C) - (a_A - a_B),
              Q = (a_A - a_B) - P,
              R = a_C - a_A,
              S = a_B;
        return ((P * a_Pct + Q) * a_Pct + R) * a_Pct + S;
    }

    public static float CosineInterpolate(float a_A, float a_B, float a_Pct)
    {
        float ft = a_Pct * (float)3.1415927,
              f = (1f - Mathf.Cos(ft)) * 0.5f;
        return a_A * (1 - f) + a_B * f;
    }

    public static float LinearInterpolate(float a_A, float a_B, float a_Pct)
    {
        return a_A * (1 - a_Pct) + a_B * a_Pct;
    }
}//208