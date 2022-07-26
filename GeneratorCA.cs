using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomata_Trial
{
    class GeneratorCA
    {
        private int n;
        private int m;
        private int sizeN;
        private int sizeM;
        private bool[,] array;
        private bool[,] arrayScaled;
        private bool[,] tempArray;
        private int scale;
        private int steps;
        private int spawnChance;

        public GeneratorCA(int N, int M, int nS, int mS, int s, int st, int sCh)
        {
            n = N;
            m = M;
            sizeN = nS;
            sizeM = mS;
            arrayScaled = new bool[sizeN, sizeM];
            array = new bool[n, m];
            tempArray = new bool[n, m];
            scale = s;
            steps = st;
            spawnChance = sCh;
        }

        private int countWhiteScale(int x, int y, bool scaled)  //here's where the coordinates will be transfered
        {
            int number = 0;
            if (scaled)
            {
                number += arrayScaled[y - 1, x - 1] ? 1 : 0;
                number += arrayScaled[y - 1, x] ? 1 : 0;
                number += arrayScaled[y - 1, x + 1] ? 1 : 0;
                number += arrayScaled[y, x - 1] ? 1 : 0;
                number += arrayScaled[y, x + 1] ? 1 : 0;
                number += arrayScaled[y + 1, x - 1] ? 1 : 0;
                number += arrayScaled[y + 1, x] ? 1 : 0;
                number += arrayScaled[y + 1, x + 1] ? 1 : 0;
            }
            else
            {
                number += array[y - 1, x - 1] ? 1 : 0;
                number += array[y - 1, x] ? 1 : 0;
                number += array[y - 1, x + 1] ? 1 : 0;
                number += array[y, x - 1] ? 1 : 0;
                number += array[y, x + 1] ? 1 : 0;
                number += array[y + 1, x - 1] ? 1 : 0;
                number += array[y + 1, x] ? 1 : 0;
                number += array[y + 1, x + 1] ? 1 : 0;
            }
            return number;
        }

        private void fillOrigin(int x, int y)
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    array[y * scale + i, x * scale + j] = arrayScaled[y, x];
                    tempArray[y * scale + i, x * scale + j] = arrayScaled[y, x];
                }
            }
        }
        public void generate()
        {
            Random rand = new Random();
            //Fill the scaled image
            for (int i = 0; i < sizeN; i++)
            {
                for (int j = 0; j < sizeM; j++)
                {
                    if (i == 0 || j == 0 || i == sizeN - 1 || j == sizeM - 1)
                        arrayScaled[i, j] = false;
                    else if (rand.Next(1, 101) > spawnChance)
                        arrayScaled[i, j] = false;
                    else
                        arrayScaled[i, j] = true;
                }
            }
            
            //implement the algorithm
            //Apply the 5-born/3-stay scheme for the scaled picture
            for (int i = 0; i < 20; i++)
            {
                for (int x = 1; x < sizeM - 1; x++)
                {
                    for (int y = 1; y < sizeN - 1; y++)
                    {
                        int neighbours = countWhiteScale(x, y, true);
                        if (neighbours < 3)
                            arrayScaled[y, x] = false;
                        else if (neighbours >= 5)
                            arrayScaled[y, x] = true;
                    }
                }
            }
            
            //Rescaling the picture back to the original size
            for (int x = 1; x < sizeM - 1; x++)
            {
                for (int y = 1; y < sizeN - 1; y++)
                {
                    fillOrigin(x, y);
                }
            }
            
            //Now the smoothing part starts
            //This is the 5-born/5-stay scheme on the original-sized picture
            for (int i = 0; i < steps; i++)
            {
                for (int x = 1; x < m - 1; x++)
                {
                    for (int y = 1; y < n - 1; y++)
                    {
                        int neighbours = countWhiteScale(x, y, false);
                        if (neighbours < 5)
                            tempArray[y, x] = false;
                        else if (neighbours >= 5)
                            tempArray[y, x] = true;
                    }
                }
                for (int x = 0; x < m; x++)
                {
                    for (int y = 0; y < n; y++)
                        array[y, x] = tempArray[y, x];
                }
            }
        }

        public bool[,] getArray()
        {
            return array;
        }

        public bool[,] getArrayScaled()
        {
            return arrayScaled;
        }
    }
}
