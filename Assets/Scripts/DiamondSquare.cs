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
        diamondSquare(16, 243);
    }

    Texture2D heightMap;
    Random randomGenerator;
    float randomModifier = 1;

    // Start is called before the first frame update
    void diamondSquare(int Size, int seed)
    {
        int size = Size * Size + 1;
        heightMap = new Texture2D(size, size);
        randomGenerator = new Random(seed);

        //Preseed Corners with Values
        float height = (float)randomGenerator.NextDouble();
        heightMap.SetPixel(0,0, new Color(height, height, height));
        height = (float)randomGenerator.NextDouble();
        heightMap.SetPixel(0,size-1, new Color(height, height, height));
        height = (float)randomGenerator.NextDouble();
        heightMap.SetPixel(size-1,0, new Color(height, height, height));
        height = (float)randomGenerator.NextDouble();
        heightMap.SetPixel(size-1,size-1, new Color(height, height, height));

        int reach = size / 2;
        int stepSize = size;

        while(reach >= 1)
        {
            Debug.Log("While Reach: "+stepSize);
            //Do Square Step
            for (int x = reach; x < heightMap.width; x += stepSize)
            {
                for (int y = reach; y < heightMap.height; y += stepSize)
                {
                    Debug.Log("Square:" + x + ": " + y);
                    squareStep(stepSize, x, y);
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
                    diamondStep(stepSize, x, y);
                }
            }

            reach /= 2;
            stepSize /= 2;
            randomModifier += 10f;
        }

        //ToDo: Debug Write out Bitmap
        File.WriteAllBytes("D:/Dokumente/Git Repositorys/ComputerGrafik/Assets/Scripts/tmp.png", heightMap.EncodeToPNG());
    }

    void squareStep(int stepSize, int x, int y)
    {
        /*
         *  A   B
         *    X
         *  C   D
         */
        float avg = 0;
        int counter = 0;

        //Check A
        if(x - stepSize >= 0 & y + stepSize < heightMap.height)
        {
            avg += heightMap.GetPixel(x - stepSize, y + stepSize).a;
            counter++;
        }
        //Check B
        if (x + stepSize < heightMap.width & y + stepSize < heightMap.height)
        {
            avg += heightMap.GetPixel(x + stepSize, y + stepSize).a;
            counter++;
        }
        //Check C
        if (x - stepSize >= 0 & y - stepSize >= 0)
        {
            avg += heightMap.GetPixel(x - stepSize, y - stepSize).a;
            counter++;
        }
        //Check D
        if (x + stepSize < heightMap.width & y - stepSize >= 0)
        {
            avg += heightMap.GetPixel(x + stepSize, y - stepSize).a;
            counter++;
        }

        avg += (float)randomGenerator.NextDouble() / randomModifier;
        avg /= counter;
        heightMap.SetPixel(x, y, new Color(avg, avg, avg));
    }

    void diamondStep(int stepSize, int x, int y)
    {
        /*
         *     B
         *  A  X  C
         *     D
         */
        float avg = 0;
        int counter = 0;

        //Check A
        if (x - stepSize >= 0)
        {
            avg += heightMap.GetPixel(x - stepSize, y).a;
            counter++;
        }
        //Check B
        if (y + stepSize < heightMap.height)
        {
            avg += heightMap.GetPixel(x, y + stepSize).a;
            counter++;
        }
        //Check C
        if (x + stepSize < heightMap.width)
        {
            avg += heightMap.GetPixel(x + stepSize, y ).a;
            counter++;
        }
        //Check D
        if (y - stepSize >= 0)
        {
            avg += heightMap.GetPixel(x, y - stepSize).a;
            counter++;
        }

        avg += (float)randomGenerator.NextDouble()/randomModifier;
        avg /= counter;
        heightMap.SetPixel(x, y, new Color(avg, avg, avg));
    }
}
