﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GameOfLife
{
    class ParGameOfLife
    {
        private readonly long _gridSize;
        private const long AliveThreshold = 25;
        private long[,] _curGrid;
        private long[,] _newGrid;

        public ParGameOfLife(int gridSize)
        {
            _gridSize = gridSize;
            _curGrid = new long[_gridSize, _gridSize];
            _newGrid = new long[_gridSize, _gridSize];

            var rnd = new Random();
            for (var row = 0; row < _gridSize; row++)
            {
                for (var col = 0; col < _gridSize; col++)
                {
                    _curGrid[row, col] = (rnd.Next(100) < AliveThreshold) ? 1 : 0;
                }
            }
        }

        public void Run(long numberOfIterations)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            while (numberOfIterations > 0)
            {
                var locationsAlive = 0;

                Parallel.For(0, _gridSize, row =>
                {
                    for (var col = 0; col < _gridSize; col++)
                    {
                        if (ShallLocationBeAlive(row, col))
                        {
                            ++locationsAlive;
                            _newGrid[row, col] = 1;
                        }
                        else
                        {
                            _newGrid[row, col] = 0;
                        }
                    }
                });
                Swap(ref _curGrid, ref _newGrid);
                Console.WriteLine("\t\t{0}", locationsAlive);
                numberOfIterations--;
            }
            stopwatch.Stop();
            System.Console.WriteLine("Parallel time elapsed {0}", stopwatch.ElapsedMilliseconds);
        }

        private bool ShallLocationBeAlive(long row, long col)
        {
            var liveNeighbors = CalcAliveNeighbors(row, col);

            if (_curGrid[row, col] == 1)
            {
                if ((liveNeighbors == 2) || (liveNeighbors == 3))
                    return true;    // Alive and with two or three neighbors - live on
            }
            else if (liveNeighbors == 3)
            {
                return true; // Dead and with exactly three neighbors - live
            }
            return false;   // Die
        }

        private long CalcAliveNeighbors(long row, long col)
        {
            long liveNeighbors;

            // Implementation of GameOfLife-rules
            if (row == 0)   // Top row
            {
                if (col == 0)    // Top left-hand corner
                    liveNeighbors = _curGrid[row, col + 1] +
                                       _curGrid[row + 1, col] + _curGrid[row + 1, col + 1];

                else if (col == _gridSize - 1) // Top right-hand corner
                    liveNeighbors = _curGrid[row, col - 1] +
                                       _curGrid[row + 1, col - 1] + _curGrid[row + 1, col];

                else // On top edge
                    liveNeighbors = _curGrid[row, col - 1] + _curGrid[row, col + 1] +
                                       _curGrid[row + 1, col - 1] + _curGrid[row + 1, col] + _curGrid[row + 1, col + 1];
            }
            else if (row == _gridSize - 1) // Bottom row
            {
                if (col == 0) // Bottom left-hand corner
                    liveNeighbors = _curGrid[row - 1, col] + _curGrid[row - 1, col + 1] +
                                                                   _curGrid[row, col + 1];

                else if (col == _gridSize - 1) // Bottom right-hand corner
                    liveNeighbors = _curGrid[row - 1, col - 1] + _curGrid[row - 1, col] +
                                       _curGrid[row, col - 1];

                else // On bottom edge
                    liveNeighbors = _curGrid[row - 1, col - 1] + _curGrid[row - 1, col] + _curGrid[row - 1, col + 1] +
                                    _curGrid[row, col - 1] + _curGrid[row, col + 1];
            }
            else if (col == 0)
            {
                // Must be left edge, since corners are covered above
                liveNeighbors = _curGrid[row - 1, col] + _curGrid[row - 1, col + 1] +
                                                            _curGrid[row, col + 1] +
                                _curGrid[row + 1, col] + _curGrid[row + 1, col + 1];
            }
            else if (col == _gridSize - 1)
            {
                // Must be right edge, since corners are covered above
                liveNeighbors = _curGrid[row - 1, col - 1] + _curGrid[row - 1, col] +
                                  _curGrid[row, col - 1] +
                                  _curGrid[row + 1, col - 1] + _curGrid[row + 1, col];

            }
            else
            {
                // Interior field
                liveNeighbors = _curGrid[row - 1, col - 1] + _curGrid[row - 1, col] + _curGrid[row, col + 1] +
                                    _curGrid[row + 1, col - 1] + _curGrid[row + 1, col] +
                                    _curGrid[row + 1, col - 1] + _curGrid[row + 1, col] + _curGrid[row + 1, col + 1];
            }


            return liveNeighbors;
        }

        private void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }
    }
}

