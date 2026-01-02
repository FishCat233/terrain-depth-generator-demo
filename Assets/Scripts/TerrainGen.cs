using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    [Header("大小")]
    public int width;
    public int height;
    public int pixelPerUnit;

    [Header("变换")]
    public int seed = 233;
    private List<Vector2> _mainOffsets = new List<Vector2>();
    private Vector2 _altitudeOffset = Vector2.zero;
    public int octaves = 4;
    public float frequency = 4f;
    public float amplitude = 20f;

    public float altitudeFrequency = 4f;
    public AnimationCurve altitudeCurve = new AnimationCurve();

    [Header("杂项")]
    public int seaLevel = 0;
    public int maxHeight = 255;
    public Gradient colorGradient;

    private SpriteRenderer _sp;

    private void Start()
    {
        _sp = GetComponent<SpriteRenderer>();
        GenerateTerrain();
    }

    private void GenerateOffset()
    {
        _mainOffsets.Add(new Vector2(UnityEngine.Random.Range(-100000f, 100000f), UnityEngine.Random.Range(-100000f, 100000f)));
    }

    private void GenerateOffsets()
    {
        UnityEngine.Random.InitState(seed);
        GenerateOffset();
        for (int i = 0; i < octaves; ++i)
        {
            GenerateOffset();
        }

        _altitudeOffset = new Vector2(UnityEngine.Random.Range(-100000f, 100000f), UnityEngine.Random.Range(-100000f, 100000f));
    }

    public void GenerateTerrain()
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] colorMap = new Color[width * height];

        _mainOffsets.Clear();
        GenerateOffsets();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int pixelWeight = GetPixelHeight(i, j);

                if (pixelWeight < seaLevel) pixelWeight = 0;

                colorMap[j * width + i] = colorGradient.Evaluate(Mathf.InverseLerp(seaLevel, maxHeight, pixelWeight));
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        _sp.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), pixelPerUnit);
    }

    private int GetPixelHeight(int x, int y)
    {
        //float pixelHeight = seaLevel + Mathf.PerlinNoise(rx, ry);

        float pixelHeight = GetAltitude(x, y);
        float fq = frequency;
        float am = amplitude;

        // 取噪声
        foreach (var offset in _mainOffsets)
        {
            pixelHeight += Mathf.PerlinNoise(x / fq + offset.x, y / fq + offset.y) * am;
            fq *= 2f;
            am /= 2f;
        }

        //Debug.Log($"{x} {y}: {pixelHeight}");

        return (int)Mathf.Clamp(pixelHeight, 0, maxHeight);
    }

    private float GetAltitude(int x, int y)
    {
        var altitude = Mathf.PerlinNoise(x / altitudeFrequency + _altitudeOffset.x, y / altitudeFrequency + _altitudeOffset.y);

        var ret = Mathf.Clamp(altitudeCurve.Evaluate(altitude), 0, 1) * maxHeight;

        return ret;
    }
}
