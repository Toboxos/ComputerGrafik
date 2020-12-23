using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = System.Random;
using System.IO;

public class DiamondSquare : MonoBehaviour
{
    Texture2D heightMap;
    Random randomGenerator;
    public int TextureSize = 8;
    public int MaxRandomNumber = 128;
    public int Seed = 1234;

    private void Start()
    {
        diamondSquare(TextureSize, Seed);
    }

    // Start is called before the first frame update
    void diamondSquare(int Size, int seed)
    {
        int size = (int)Mathf.Pow(2, Size); // - 1 Removed -> Make it Tileable
        heightMap = new Texture2D(size, size);
        randomGenerator = new Random(seed);

        //Preseed Corners with Values
        float height = (float)randomGenerator.Next(MaxRandomNumber)/MaxRandomNumber;
        heightMap.SetPixel(0,0, new Color(height, height, height));
        height = (float)randomGenerator.Next(MaxRandomNumber) / MaxRandomNumber;


        int stepSize = size;
        int reach = stepSize / 2;

        while(reach >= 1)
        {
            //Do Square Step
            for (int x = reach; x < heightMap.width; x += stepSize)
            {
                for (int y = reach; y < heightMap.height; y += stepSize)
                {
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
                    diamondStep(reach, x, y);
                }
            }

            reach /= 2;
            stepSize /= 2;
            MaxRandomNumber = Mathf.Max(MaxRandomNumber / 2, 1);
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

        int xPos = convertToCircularIndex(x + reach, heightMap.width);
        int xNeg = convertToCircularIndex(x - reach, heightMap.width);
        int yPos = convertToCircularIndex(y + reach, heightMap.width);
        int yNeg = convertToCircularIndex(y - reach, heightMap.width);

        //Check A
        avg += heightMap.GetPixel( xNeg, yPos).r;
        counter++;
        //Check B
        avg += heightMap.GetPixel(xPos, yPos).r;
        counter++;
        //Check C
        avg += heightMap.GetPixel(xNeg, yNeg).r;
        counter++;
        //Check D
        avg += heightMap.GetPixel(xPos, yNeg).r;
        counter++;

        avg += (float)randomGenerator.Next(-MaxRandomNumber, MaxRandomNumber) / MaxRandomNumber;
        avg /= counter;
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

        int xPos = convertToCircularIndex(x + reach, heightMap.width);
        int xNeg = convertToCircularIndex(x - reach, heightMap.width);
        int yPos = convertToCircularIndex(y + reach, heightMap.width);
        int yNeg = convertToCircularIndex(y - reach, heightMap.width);

        //Check A
        avg += heightMap.GetPixel(xNeg, y).r;
        counter++;
        //Check B
        avg += heightMap.GetPixel(x, yPos).r;
        counter++;
        //Check C
        avg += heightMap.GetPixel(xPos, y ).r;
        counter++;
        //Check D
        avg += heightMap.GetPixel(x, yNeg).r;
        counter++;

        avg += (float)randomGenerator.Next(-MaxRandomNumber, MaxRandomNumber) / MaxRandomNumber;
        avg /= counter;
        heightMap.SetPixel(x, y, new Color(avg, avg, avg));
    }

    int convertToCircularIndex(int index, int MaxLength)
    {
        int i = index % MaxLength;
        return Mathf.Abs(i);
    }
}
