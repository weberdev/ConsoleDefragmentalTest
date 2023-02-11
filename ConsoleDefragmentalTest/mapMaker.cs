using System.Collections;
using System.Collections.Generic;
using System;
using Random = System.Random;

namespace mapMaker
{

    public class Map 
    {
        public char[,] positions;
        public int spawnX;
        public int spawnY;
        public int stairsX;
        public int stairsY;
        public static char[,] fillMap(char[,] inputMap, int sideLength, int spawnX, int spawnY)
        {
            char[,] positions = inputMap;
            Random whimsOfAzathoth = new Random();
            int numberOfRooms = whimsOfAzathoth.Next(sideLength / 2, sideLength);
            int priorRoomX = spawnX;
            int priorRoomY = spawnY;
            int larger;
            int smaller;
            for (int i = 0; i < numberOfRooms; i++)
            {
                if (i % 2 == 0)
                {
                    int roomX = boundedRand(sideLength, 9, sideLength - 9);
                    int roomY = priorRoomY;
                    int roomWidth = boundedRand(64, 2, 5);
                    int roomLength = boundedRand(64, 2, 5);
                    ;
                    for (int h = roomX - roomWidth; h < (roomX + roomWidth + 1); h++)
                    {
                        for (int j = roomY - roomLength; j < (roomY + roomLength + 1); j++)
                        {
                            if (positions[h, j] == 'X')
                            {
                                positions[h, j] = '.';
                            }
                            if ((positions[h, j] == 'C') || (positions[h, j] == 'R'))
                            {
                                positions[h, j] = 'R';
                            }
                        }
                        smaller = Math.Min(roomX, priorRoomX);
                        larger = Math.Max(roomX, priorRoomX);

                        for (int c = smaller; c < larger; c++)
                        {
                            positions[c, roomY] = '.';
                        }
                        positions[roomX, roomY] = 'C';
                        if (i == numberOfRooms - 1)
                        {
                            positions[roomX, roomY] = 'S';
                        }
                        priorRoomX = roomX;
                        priorRoomY = roomY;


                    }
                }

                else
                {
                    int roomX = priorRoomX;
                    int roomY = boundedRand(sideLength, 9, sideLength - 9);
                    int roomWidth = boundedRand(64, 2, 7);
                    int roomLength = boundedRand(64, 2, 7);
                    ;
                    for (int h = roomX - roomWidth; h < (roomX + roomWidth + 1); h++)
                    {
                        for (int j = roomY - roomLength; j < (roomY + roomLength + 1); j++)
                        {
                            if (positions[h, j] == 'X')
                            {
                                positions[h, j] = '.';
                            }
                            if (positions[h, j] == 'C')
                            {
                                positions[h, j] = 'R';
                            }
                        }
                        smaller = Math.Min(roomY, priorRoomY);
                        larger = Math.Max(roomY, priorRoomY);

                        for (int c = smaller; c < larger; c++)
                        {
                            positions[roomX, c] = '.';
                        }
                        positions[roomX, roomY] = 'C';
                        priorRoomX = roomX;
                        priorRoomY = roomY; positions[roomX, roomY] = 'C';
                        if (i == numberOfRooms - 1)
                        {
                            positions[roomX, roomY] = 'S';
                        }
                        priorRoomX = roomX;
                        priorRoomY = roomY;
                    }
                }
            }
            return positions;
        }

        public static int getSegment(int sideValue)
        {
            Random dieRoller = new Random();
            int segment = dieRoller.Next(0, sideValue);
            return segment;
        }

        //creates a char array map of 
        public char[,] courtyardMap(int lengthRoot)
        {
            int length = lengthRoot * lengthRoot;
            char[,] positions = new char[length, length];
            int maxRooms = lengthRoot + 1;
            Random EntropicDecay = new Random();
            int maxRoomSize = lengthRoot - 1;
            int decay;
            int decayChance = 8;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    decay = EntropicDecay.Next(1, decayChance + 1);
                    if (((i % (length - 1) == 0) || (j % (length - 1) == 0)) || (j % maxRoomSize == 0) && (i % maxRoomSize == 0))
                    {
                        positions[i, j] = 'X';
                    }
                    else if ((i % maxRoomSize == 1) && (decay == decayChance) && (j % maxRoomSize == 0))
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            positions[i + k, j] = '.';
                        }
                    }
                    else if ((j % maxRoomSize == 1) && (decay == decayChance) && (i % maxRoomSize == 0))
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            positions[i, j + k] = '.';
                        }
                    }
                    else if (((i % maxRoomSize == 0) | (j % maxRoomSize) == 0) && (decay != decayChance))
                    {
                        if (positions[i, j] != '.')
                        {
                            positions[i, j] = 'X';
                        }
                    }

                    else
                    {

                        positions[i, j] = '.';
                    }
                }
            }
            int sectionSpawnX = getSegment(maxRoomSize - 2);
            sectionSpawnX++;
            int spawnX = EntropicDecay.Next(1, maxRoomSize) + (maxRoomSize * sectionSpawnX);
            int sectionSpawnY = getSegment(maxRoomSize - 2);
            sectionSpawnY++;
            int spawnY = EntropicDecay.Next(1, maxRoomSize) + (maxRoomSize * sectionSpawnY);
            int sectionStairsX = getSegment(maxRoomSize);
            int stairsX = EntropicDecay.Next(1, maxRoomSize) + (maxRoomSize * sectionStairsX);
            int sectionStairsY = getSegment(maxRoomSize);
            int stairsY = EntropicDecay.Next(1, maxRoomSize) + (maxRoomSize * sectionStairsY);
            while (((Math.Abs(sectionSpawnX - sectionStairsX) + Math.Abs(sectionSpawnY - sectionStairsY) < 8)))
            {
                sectionSpawnX = getSegment(maxRoomSize - 2);
                sectionSpawnX++;
                sectionSpawnY = getSegment(maxRoomSize - 2);
                sectionSpawnY++;
                sectionStairsY = getSegment(maxRoomSize);
                sectionStairsX = getSegment(maxRoomSize);
                spawnX = EntropicDecay.Next(1, maxRoomSize) + (maxRoomSize * sectionSpawnX);
                spawnY = EntropicDecay.Next(1, maxRoomSize) + (maxRoomSize * sectionSpawnY);
                stairsX = EntropicDecay.Next(1, maxRoomSize) + (maxRoomSize * sectionStairsX);
                stairsY = EntropicDecay.Next(1, maxRoomSize) + (maxRoomSize * sectionStairsY);
            }

            positions[spawnX, spawnY] = '@';
            positions[stairsX, stairsY] = 'S';
            while ((sectionSpawnX != sectionStairsX) | (sectionSpawnY != sectionStairsY))
            {
                if (sectionSpawnX < sectionStairsX)
                {
                    positions[(maxRoomSize * sectionSpawnX) + maxRoomSize, (maxRoomSize * sectionSpawnY) + EntropicDecay.Next(1, maxRoomSize)] = 'O';
                    sectionSpawnX++;
                }
                else if (sectionSpawnX > sectionStairsX)
                {
                    positions[(maxRoomSize * sectionSpawnX), (maxRoomSize * sectionSpawnY) + EntropicDecay.Next(1, maxRoomSize)] = 'O';
                    sectionSpawnX--;
                }
                if (sectionSpawnY < sectionStairsY)
                {
                    positions[(maxRoomSize * sectionSpawnX) + EntropicDecay.Next(1, maxRoomSize), (maxRoomSize * sectionSpawnY) + maxRoomSize] = 'O';
                    sectionSpawnY++;
                }
                else if (sectionSpawnY > sectionStairsY)
                {
                    positions[(maxRoomSize * sectionSpawnX) + EntropicDecay.Next(1, maxRoomSize), (maxRoomSize * sectionSpawnY)] = 'O';
                    sectionSpawnY--;
                }
            }


            return positions;
        }

        public char[,] instantiateMap(int lengthRoot)
        {
            int length = lengthRoot * lengthRoot;
            Random dartsAtAWall = new Random();

            spawnX = dartsAtAWall.Next(((lengthRoot / 2) + 1), length - ((lengthRoot / 2) + 1));
            spawnY = dartsAtAWall.Next(((lengthRoot / 2) + 1), length - ((lengthRoot / 2) + 1));
            positions = new char[length, length];
            stairsX = dartsAtAWall.Next(((lengthRoot / 2) + 1), length - ((lengthRoot / 2) + 1));
            stairsY = dartsAtAWall.Next(((lengthRoot / 2) + 1), length - ((lengthRoot / 2) + 1));
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    positions[i, j] = 'X';
                }
            }
            while ((Math.Abs(spawnX - stairsX) + Math.Abs(spawnY - stairsY) < 45))
            {
                stairsX = dartsAtAWall.Next(5, length - 5);
                stairsY = dartsAtAWall.Next(5, length - 5);
            }

            for (int i = spawnX - 2; i < spawnX + 3; i++)
            {
                for (int j = spawnY - 2; j < spawnY + 3; j++)
                {
                    positions[i, j] = '.';
                }
            }
            for (int i = stairsX - 2; i < stairsX + 3; i++)
            {
                for (int j = stairsY - 2; j < stairsY + 3; j++)
                {
                    positions[i, j] = '.';
                }
            }
            positions[spawnX, spawnY] = '@';
            positions[stairsX, stairsY] = 'S';
            
            int numberOfRooms = dartsAtAWall.Next(length / 2, length);
            int priorRoomX = spawnX;
            int priorRoomY = spawnY;
            int larger;
            int smaller;
            for (int i = 0; i < numberOfRooms; i++)
            {
                if (i % 2 == 0)
                {
                    int roomX = dartsAtAWall.Next(((lengthRoot / 2) + 1), length - ((lengthRoot / 2) + 1));
                    int roomY = priorRoomY;
                    int roomWidth = dartsAtAWall.Next(2, (lengthRoot/2));
                    int roomLength = dartsAtAWall.Next(2, (lengthRoot / 2));
                    ;
                    for (int h = roomX - roomWidth; h < (roomX + roomWidth + 1); h++)
                    {
                        for (int j = roomY - roomLength; j < (roomY + roomLength + 1); j++)
                        {
                            if (positions[h, j] == 'X')
                            {
                                positions[h, j] = '.';
                            }
                            if ((positions[h, j] == 'C') || (positions[h, j] == 'R'))
                            {
                                positions[h, j] = 'R';
                            }
                        }
                        smaller = Math.Min(roomX, priorRoomX);
                        larger = Math.Max(roomX, priorRoomX);

                        for (int c = smaller; c < larger; c++)
                        {
                            positions[c, roomY] = '.';
                        }
                        positions[roomX, roomY] = 'C';
                        if (i == numberOfRooms - 1)
                        {
                            positions[roomX, roomY] = 'S';
                        }
                        priorRoomX = roomX;
                        priorRoomY = roomY;


                    }
                }

                else
                {
                    int roomX = priorRoomX;
                    int roomY = dartsAtAWall.Next(((lengthRoot / 2) + 1), length - ((lengthRoot / 2) + 1));
                    int roomWidth = dartsAtAWall.Next(2, (lengthRoot/2));
                    int roomLength = dartsAtAWall.Next(2, (lengthRoot / 2));
                    ;
                    for (int h = roomX - roomWidth; h < (roomX + roomWidth + 1); h++)
                    {
                        for (int j = roomY - roomLength; j < (roomY + roomLength + 1); j++)
                        {
                            if (positions[h, j] == 'X')
                            {
                                positions[h, j] = '.';
                            }
                            if (positions[h, j] == 'C')
                            {
                                positions[h, j] = 'R';
                            }
                        }
                        smaller = Math.Min(roomY, priorRoomY);
                        larger = Math.Max(roomY, priorRoomY);

                        for (int c = smaller; c < larger; c++)
                        {
                            positions[roomX, c] = '.';
                        }
                        positions[roomX, roomY] = 'C';
                        priorRoomX = roomX;
                        priorRoomY = roomY; positions[roomX, roomY] = 'C';
                        if (i == numberOfRooms - 1)
                        {
                            positions[roomX, roomY] = 'S';
                        }
                        priorRoomX = roomX;
                        priorRoomY = roomY;
                    }
                }
            }
            positions[spawnX, spawnY] = '@';
            return positions;
        }
        public char[,] corridorMap(int sideRoot)
        {
            Random rand = new Random();
            int sideLength = sideRoot * sideRoot;

            int halfSideLength = sideLength / 2;
            int spawnX = 2 * rand.Next(1, halfSideLength - 1);
            int spawnY = 2 * rand.Next(1, halfSideLength - 1);
            int stairsX = 2 * rand.Next(1, halfSideLength - 1);
            int stairsY = 2 * rand.Next(1, halfSideLength - 1);
            positions = new char[sideLength, sideLength];
            while ((Math.Abs(spawnX - stairsX) + Math.Abs(spawnY - stairsY) < 45))
            {
                stairsX = 2 * rand.Next(1, halfSideLength - 1);
                stairsY = 2 * rand.Next(1, halfSideLength - 1);
            }
            for (int i = 0; i < sideLength; i++)
            {
                for (int j = 0; j < sideLength; j++)
                {
                    positions[i, j] = 'X';
                }
            }
            int numberOfRooms = rand.Next(sideLength, 2 * sideLength);
            int priorRoomX = spawnX;
            int priorRoomY = spawnY;
            int larger;
            int smaller;
            for (int i = 0; i < numberOfRooms; i++)
            {
                if (i % 2 == 0)
                {
                    int roomX = 2 * rand.Next((sideRoot / 2), halfSideLength - ((sideRoot / 2)));
                    int roomY = priorRoomY;
                    smaller = Math.Min(roomX, priorRoomX);
                    larger = Math.Max(roomX, priorRoomX);

                    for (int c = smaller; c < larger; c++)
                    {
                        positions[c, roomY] = '.';
                    }
                    positions[roomX, roomY] = 'C';
                    if (i == numberOfRooms - 1)
                    {
                        positions[roomX, roomY] = 'S';
                    }
                    priorRoomX = roomX;
                    priorRoomY = roomY;


                }


                else
                {
                    int roomX = priorRoomX;
                    int roomY = 2 * rand.Next(((sideRoot / 2)), halfSideLength - ((sideRoot / 2)));

                    smaller = Math.Min(roomY, priorRoomY);
                    larger = Math.Max(roomY, priorRoomY);

                    for (int c = smaller; c < larger; c++)
                    {
                        positions[roomX, c] = '.';
                    }
                    positions[roomX, roomY] = 'C';
                    priorRoomX = roomX;
                    priorRoomY = roomY; positions[roomX, roomY] = 'C';
                    if (i == numberOfRooms - 1)
                    {
                        positions[roomX, roomY] = 'S';
                    }
                    priorRoomX = roomX;
                    priorRoomY = roomY;
                }
            }
            positions[spawnX, spawnY] = '@';
            positions[stairsX, stairsY] = 'S';

            return positions;
        }
        public void printArray(char[,] array, int n)
        {
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((array[i, j] == '@') || (array[i, j] == 'S'))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(array[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if ((array[i, j] != 'X') && (array[i, j] != '.'))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(array[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if ((array[i, j] == '.'))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(array[i, j] + " ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(array[i, j] + " ");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();

            }
            Console.WriteLine();
        }

        public static int boundedRand(int mapSize, int lowerBound, int upperBound)
        {
            Random dartsAtAWall = new Random();
            int value = dartsAtAWall.Next(lowerBound, upperBound);
            return value;
        }

        static void Main(string[] args)
        {
            int sideRoot = 8;
            int sideLength = sideRoot * sideRoot;
            Map currentMap = new Map();
            currentMap.positions = currentMap.instantiateMap(sideRoot);
            currentMap.printArray(currentMap.positions, sideLength);
            char[,] mapAngular = currentMap.courtyardMap(sideRoot);
            currentMap.printArray(mapAngular, sideLength);
            Console.WriteLine("LEGEND");
            Console.WriteLine("'@': player spawn. 'S': stairs down. 'X': impassible wall. 'C': center of room. 'R': center of overlapped room. '.': clear ground. 'O': ground guaranteed to be cleared for traversal.");
            Console.WriteLine("Note that not all icons are present on all maps.");
            Console.ReadKey();
            Main(args);
        }
    }
}