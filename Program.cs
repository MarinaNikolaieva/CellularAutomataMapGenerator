using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CellularAutomata_Trial
{
    class Program
    {
        static int n;  //height
        static int m;  //width
        static Bitmap pict;
        static bool[,] img;  //picture NxM
        static Color[,] colored;
        static int sizeN;  //size of the scaled N
        static int sizeM;  //size of the scaled M
        static int scale;
        static double proportion;  //proportion between N and M (must be stable!)
        
        static int countEmptySpace(int x, int y)
        {
            int space = 0;

            space += img[y, x - 1] ? 1 : 0;
            space += img[y - 1, x - 1] ? 1 : 0;
            space += img[y - 2, x - 1] ? 1 : 0;
            space += img[y, x] ? 1 : 0;
            space += img[y - 1, x] ? 1 : 0;
            space += img[y - 2, x] ? 1 : 0;
            space += img[y, x + 1] ? 1 : 0;
            space += img[y - 1, x + 1] ? 1 : 0;
            space += img[y - 2, x + 1] ? 1 : 0;

            return space;
        }

        static bool checkFloor(int x, int y)
        {
            int floor = 0;

            floor += img[y + 1, x - 1] ? 0 : 1;
            floor += img[y + 1, x] ? 0 : 1;
            floor += img[y + 1, x + 1] ? 0 : 1;

            if (floor == 3)
                return true;
            else
                return false;
        }

        //static bool canBePlaced(int x, int y, int xLimit, int yLimit, bool needFloor, bool needWall, bool left, bool contour) 
        //    //for mob placing
        //    //NEEDED add the option to check just the square contours, not only the full-empty spaces
        //    //GUESS DONE, NEED TEST!
        //    //WHAT ELSE MAY BE NEEDED?
        //    //maybe place mobs on the ceiling?
        //    //maybe place mobs that can ride walls in circles/more complex pathways? This requires looking for floors
        //{
        //    int number = 0;
        //    bool floor = true;
        //    bool wall = true;

        //    for (int i = x; i < x + xLimit; i++)
        //    {
        //        for (int j = y + yLimit - 1; j >= y; j--)
        //        {
        //            if (contour)  //if we need just the bordering square
        //            {
        //                if (j != y + yLimit - 1 && j != y) {  //if we're on the vertical borders
        //                    if (i == x || i == x + xLimit - 1)  //then include only the borders, not the inside
        //                        number += colored[j, i] == Color.White ? 1 : 0;
        //                }
        //                else  //if we're on the horizontal borders, check every pixel of them
        //                {
        //                    number += colored[j, i] == Color.White ? 1 : 0;
        //                }

        //            }  //if we need the filled square
        //            else
        //                number += colored[j, i] == Color.White ? 1 : 0;
        //        }
        //    }

        //    if (needFloor)
        //    {
        //        for (int i = x; i < x + xLimit; i++)
        //            if (colored[y + 1, i] != Color.Black)
        //                floor = false;
        //    }

        //    if (needWall)
        //    {
        //        for (int i = y; i < y + yLimit; i++)
        //        {
        //            if (left)  //if we need the left wall
        //            {
        //                if (colored[x - 1, i] != Color.Black)
        //                    wall = false;
        //            }
        //            else  //if we need the right wall
        //            {
        //                if (colored[x + 1, i] != Color.Black)
        //                    wall = false;
        //            }
        //        }
        //    }

        //    if (((number == x * 2 + (y * 2 - 2) && contour) || (number == x * y && !contour)) && ((needFloor && floor) 
        //        || (needWall && wall) || (!needFloor && !needWall)))
        //        return true;
        //    else
        //        return false;
        //}

        static void Main(string[] args)
        {
            //init picture dimentions
            Console.WriteLine("Please enter how many pixels do you want");
            Console.WriteLine("Height: ");
            n = int.Parse(Console.ReadLine());
            Console.WriteLine("Width: ");
            m = int.Parse(Console.ReadLine());

            proportion = n > m ? n / m : m / n;
            sizeN = n > m ? (int)(50 * proportion) : 50;
            sizeM = n < m ? (int)(50 * proportion) : 50;

            scale = n / sizeN;

            pict = new Bitmap(m, n);
            img = new bool[n, m];
            colored = new Color[n, m];

            //black - solid, white - open
            Console.WriteLine("How many smoothing steps do you want? ");
            int steps = int.Parse(Console.ReadLine());

            Random rand = new Random();
            //chance of the point to spawn white
            //int spawnChance = rand.Next() * (100 - 1) + 1;
            //how many neighbour white points are there for the white point to stay white
            //int stayNum = rand.Next(1, 9);
            //how many neighbour white points are there for the white point to turn black
            //int leaveNum = rand.Next(stayNum, 9);
            //how many neighbour white points are there for the black point to turn white
            //int returnNum = rand.Next(1, 9);

            int spawnChance = 45;

            try
            {
                GeneratorCA gener = new GeneratorCA(n, m, sizeN, sizeM, scale, steps, spawnChance);
                gener.generate();
                img = gener.getArray().Clone() as bool[,];

                //Copying the image into the bitmap for export
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < m; j++)
                    {
                        if (img[i, j])
                        {
                            pict.SetPixel(j, i, Color.White);
                            colored[i, j] = Color.White;
                        }
                        else
                        {
                            pict.SetPixel(j, i, Color.Black);
                            colored[i, j] = Color.Black;
                        }
                    }

                //NEEDED Deal with empty unconnected spaces!

                //Now - set the Start & End points for the player
                bool stop = false;

                //start point
                for (int x = 1; x < m - 1; x++)
                {
                    for (int y = n - 2; y > 3; y--)
                    {
                        int count = countEmptySpace(x, y);
                        bool floor = checkFloor(x, y);
                        if (count == 9 && floor)
                        {
                            colored[y, x] = Color.Red;
                            pict.SetPixel(x, y, Color.Red);
                            stop = true;
                            break;
                        }
                    }
                    if (stop)
                    {
                        stop = false;
                        break;
                    }
                }

                //end point
                for (int x = m - 2; x > 0; x--)
                {
                    for (int y = n - 2; y > 3; y--)
                    {
                        int count = countEmptySpace(x, y);
                        bool floor = checkFloor(x, y);
                        if (count == 9 && floor)
                        {
                            colored[y, x] = Color.Red;
                            pict.SetPixel(x, y, Color.Red);
                            stop = true;
                            break;
                        }
                    }
                    if (stop)
                    {
                        stop = false;
                        break;
                    }
                }

                //export the bitmap into the JPEG file
                pict.Save("C:\\Data\\Diploma\\image1st.jpeg", ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught");
                Console.WriteLine(ex);
            }
        }
    }
}
