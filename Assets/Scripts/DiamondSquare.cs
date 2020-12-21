using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = System.Random;
using System.IO;

public class DiamondSquare : MonoBehaviour
{
    private void Start()
    {
        diamondSquare(3, 243);
    }

    Texture2D heightMap;
    Random randomGenerator;
    int MaxRandom = 128;

    // Start is called before the first frame update
    void diamondSquare(int Size, int seed)
    {
        int size = Size * Size + 1;
        heightMap = new Texture2D(size, size);
        randomGenerator = new Random(seed);

        //Preseed Corners with Values
        float height = (float)randomGenerator.Next(MaxRandom)/MaxRandom;
        heightMap.SetPixel(0,0, new Color(height, height, height));
        height = (float)randomGenerator.Next(MaxRandom) / MaxRandom;
        heightMap.SetPixel(0,size-1, new Color(height, height, height));
        height = (float)randomGenerator.Next(MaxRandom) / MaxRandom;
        heightMap.SetPixel(size-1,0, new Color(height, height, height));
        height = (float)randomGenerator.Next(MaxRandom) / MaxRandom;
        heightMap.SetPixel(size-1,size-1, new Color(height, height, height));


        int stepSize = size - 1;
        int reach = stepSize / 2;

        while(reach >= 1)
        {
            Debug.Log("While Reach: "+reach);
            //Do Square Step
            for (int x = reach; x < heightMap.width; x += stepSize)
            {
                for (int y = reach; y < heightMap.height; y += stepSize)
                {
                    Debug.Log("Square:" + x + ": " + y);
                    squareStep(reach, x, y);
                }
            }

            //Do Diamond Step
            for (int x = 0; x < heightMap.width; x+= reach)
            {
                //Calculate Starting Position in Columms
                int startY = 0;
                if((x / reach)%2 == 0)
                {
                    startY = reach;
                }

                for (int y = startY; y < heightMap.height; y+= stepSize)
                {
                    Debug.Log("Diamond:"+ x + ": "+ y);
                    diamondStep(reach, x, y);
                }
            }

            reach /= 2;
            stepSize /= 2;
            MaxRandom = Mathf.Min(MaxRandom / 2, 1);
        }

        //ToDo: Debug Write out Bitmap
        File.WriteAllBytes("D:/Dokumente/Git Repositorys/ComputerGrafik/Assets/Scripts/tmp.png", heightMap.EncodeToPNG());
    }

    void squareStep(int reach, int x, int y)
    {
        /*
         *  A   B
         *    X
         *  C   D
         */
        float avg = 0;
        int counter = 0;

        //Check A
        if(x - reach >= 0 & y + reach < heightMap.height)
        {
            avg += heightMap.GetPixel(x - reach, y + reach).r;
            counter++;
        }
        //Check B
        if (x + reach < heightMap.width & y + reach < heightMap.height)
        {
            avg += heightMap.GetPixel(x + reach, y + reach).r;
            counter++;
        }
        //Check C
        if (x - reach >= 0 & y - reach >= 0)
        {
            avg += heightMap.GetPixel(x - reach, y - reach).r;
            counter++;
        }
        //Check D
        if (x + reach < heightMap.width & y - reach >= 0)
        {
            avg += heightMap.GetPixel(x + reach, y - reach).r;
            counter++;
        }

        avg += (float)randomGenerator.Next(-MaxRandom, MaxRandom) / MaxRandom;
        avg /= counter;
        //Debug.Log("Square Avg: "+avg);
        heightMap.SetPixel(x, y, new Color(avg, avg, avg));
    }

    void diamondStep(int reach, int x, int y)
    {
        /*
         *     B
         *  A  X  C
         *     D
         */
        float avg = 0;
        int counter = 0;

        //Check A
        if (x - reach >= 0)
        {
            avg += heightMap.GetPixel(x - reach, y).r;
            counter++;
        }
        //Check B
        if (y + reach < heightMap.height)
        {
            avg += heightMap.GetPixel(x, y + reach).r;
            counter++;
        }
        //Check C
        if (x + reach < heightMap.width)
        {
            avg += heightMap.GetPixel(x + reach, y ).r;
            counter++;
        }
        //Check D
        if (y - reach >= 0)
        {
            avg += heightMap.GetPixel(x, y - reach).r;
            counter++;
        }

        avg += (float)randomGenerator.Next(-MaxRandom, MaxRandom) / MaxRandom;
        avg /= counter;
        //Debug.Log("Diamond Avg: " + avg);
        heightMap.SetPixel(x, y, new Color(avg, avg, avg));
    }
}
