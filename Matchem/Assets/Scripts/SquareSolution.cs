﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class SquareSolution : ISolution
    {
        public List<ITile> Tiles { get; set; }

        public int Seed { get; set; }

        public List<string> Symbols { get; set; }

        public List<Color> Colours { get; set; }

        private int _size;
        public int Size
        {
            get
            {
                return _size;
            }
        }

        public int SymbolDifficulty
        {
            get
            {
                return Symbols.Count;
            }
        }

        public int ColourDifficulty
        {
            get
            {
                return Colours.Count;
            }
        }

        public int TileDifficulity
        {
            get
            {
                return 4;
            }
        }

        public SquareSolution()
        {
            Tiles = new List<ITile>();
            Symbols = new List<string>();
            Colours = new List<Color>();
        }

        public void GenerateSolution(int seed, int size, int symbolDifficulty, int colourDifficulty)
        {
            // Reset the random sequence to generate the symbols on the tiles.
            TileHelper.SetRandomSequence(seed);

            _size = size;

            // Get a symbol set to use for this solution, controlled by the difficulty level.
            Symbols = TileHelper.GetSymbolSet(symbolDifficulty);

            // Get a colour set to use for this solution, also controlled by the difficulty level.
            Colours = TileHelper.GetColourSet(colourDifficulty);

            // For a square board, we start at the top and work across in rows. The number of rows will be the size.
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    Tiles.Add(GenerateTile(x, y));
                }
            }
        }

        private ITile GenerateTile(int x, int y)
        {
            SquareTile tile = new SquareTile();
            tile.x = x;
            tile.y = y;

            // A list of sides that make up the object:
            //  Square      
            //      top = 0
            //      right = 1
            //      bottom = 2
            //      left = 3

            // For all others - new symbols will be on right and bottom, read neighbours to get symbols

            //  if y = 0 && x = 0
            //      Generate T, R, B, L
            //  else if y = 0 && x != 0
            //      Generate T
            //      Set L to R of tile (x - 1) (Store from previous iteration?)
            //      Generate R, B
            //  else if y > 0 && y < size - 1 && x = 0
            //      Set T to B of tile (row - 1)
            //      Generate R, B, L
            //  else if y > 0 && y < size - 1 && x != 0
            //      Set T to B of tile (y - 1)
            //      Generate R, B
            //      Set L to R of tile (x - 1)
            //  else if y = size - 1 && x = 0
            //      Set T to B of tile (y - 1)
            //      Generate R, B, L
            //  else if y = size - 1 & x != 0
            //      Set T to B of tile (y - 1)
            //      Generate R, B
            //      Set L to R of tile (x - 1)

            //Debug.LogFormat("Generating tile: {0}, {1}", x, y);

            try
            {
                if (y == 0 && x == 0)
                {
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                }
                else if (y == 0 && x != 0)
                {
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(GetTileAt(y, x - 1).Sides[(int)SquareTile.SideEnum.Right]);      // TODO - Trap null here!
                }
                else if (y > 0 && y < _size - 1 && x == 0)
                {
                    tile.Sides.Add(GetTileAt(y - 1, x).Sides[(int)SquareTile.SideEnum.Bottom]);      // TODO - Trap null here!
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                }
                else if (y > 0 && y < _size - 1 && x != 0)
                {
                    tile.Sides.Add(GetTileAt(y - 1, x).Sides[(int)SquareTile.SideEnum.Bottom]);      // TODO - Trap null here!
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(GetTileAt(y, x - 1).Sides[(int)SquareTile.SideEnum.Right]);      // TODO - Trap null here!
                }
                else if (y == _size - 1 && x == 0)
                {
                    tile.Sides.Add(GetTileAt(y - 1, x).Sides[(int)SquareTile.SideEnum.Bottom]);      // TODO - Trap null here!
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                }
                else if (y == _size - 1 && x != 0)
                {
                    tile.Sides.Add(GetTileAt(y - 1, x).Sides[(int)SquareTile.SideEnum.Bottom]);      // TODO - Trap null here!
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(TileHelper.GetSide(Symbols, Colours));
                    tile.Sides.Add(GetTileAt(y, x - 1).Sides[(int)SquareTile.SideEnum.Right]);      // TODO - Trap null here!
                }
                else
                {
                    Debug.Log(string.Format("Tile generation failed at {0}, {1}", x, y));
                }
            }
            catch (Exception ex)
            {
                Debug.LogFormat("Error accessing neighbouring tile for {0}, {1} - {2}", x, y, ex.ToString());
            }



            //if (row == 0 && col == 0)
            //{
            //    tile.Sides[(int)SquareTile.SideEnum.Top] = TileHelper.GetSide(Symbols, Colours);
            //    tile.Sides[(int)SquareTile.SideEnum.Left] = TileHelper.GetSide(Symbols, Colours);
            //}
            //else if (row == 0 && col != 0)
            //{
            //    tile.Sides[(int)SquareTile.SideEnum.Top] = TileHelper.GetSide(Symbols, Colours);
            //    tile.Sides[(int)SquareTile.SideEnum.Left] = GetTileAt(row, col - 1).Sides[(int)SquareTile.SideEnum.Right];      // TODO - Trap null here!
            //}
            //else if (row > 0 && row < _size - 1 && col == 0)
            //{
            //    tile.Sides[(int)SquareTile.SideEnum.Top] = GetTileAt(row - 1, col).Sides[(int)SquareTile.SideEnum.Bottom];      // TODO - Trap null here!
            //    tile.Sides[(int)SquareTile.SideEnum.Left] = TileHelper.GetSide(Symbols, Colours);
            //}
            //else if (row > 0 && row < _size - 1 && col != 0)
            //{
            //    tile.Sides[(int)SquareTile.SideEnum.Top] = GetTileAt(row - 1, col).Sides[(int)SquareTile.SideEnum.Bottom];      // TODO - Trap null here!
            //    tile.Sides[(int)SquareTile.SideEnum.Left] = GetTileAt(row, col - 1).Sides[(int)SquareTile.SideEnum.Right];      // TODO - Trap null here!
            //}
            //else if (row == _size - 1 && col == 0)
            //{
            //    tile.Sides[(int)SquareTile.SideEnum.Top] = GetTileAt(row - 1, col).Sides[(int)SquareTile.SideEnum.Bottom];      // TODO - Trap null here!
            //    tile.Sides[(int)SquareTile.SideEnum.Left] = TileHelper.GetSide(Symbols, Colours);
            //}
            //else if (row == _size - 1 && col != 0)
            //{
            //    tile.Sides[(int)SquareTile.SideEnum.Top] = GetTileAt(row - 1, col).Sides[(int)SquareTile.SideEnum.Bottom];      // TODO - Trap null here!
            //    tile.Sides[(int)SquareTile.SideEnum.Left] = GetTileAt(row, col - 1).Sides[(int)SquareTile.SideEnum.Right];      // TODO - Trap null here!
            //}
            //else
            //{
            //    Debug.Log(string.Format("Tile generation failed at {0}, {1}", row, col));
            //}

            //// We always generate new symbols and colours for R, B
            //tile.Sides[(int)SquareTile.SideEnum.Right] = TileHelper.GetSide(Symbols, Colours);
            //tile.Sides[(int)SquareTile.SideEnum.Bottom] = TileHelper.GetSide(Symbols, Colours);
            //}

            return tile;
        }

        /// <summary>
        /// Return a single Tile at the specified x,y coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ITile GetTileAt(int x, int y)
        {
            // Get an 'x, y' tile from a single list.

            // Examples (size = 4):
            //      0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25
            //      a  b  c  d  e  f  g  h  i  j  k  l  m  n  o  p  q  r  s  t  u  v  w  x  y  z
            //
            //         0  1  2  3
            //      0  a  b  c  d
            //      1  e  f  g  h
            //      2  i  j  k  l
            //      3  m  n  o  p
            //
            //      GetTileAt(0, 0) = 0 + (0 * 4)   = 0     = a
            //      GetTileAt(3, 0) = 3 + (0 * 4)   = 3     = d
            //      GetTileAt(0, 2) = 0 + (2 * 4)   = 8     = i
            //      GetTileAt(1, 3) = 1 + (3 * 4)   = 13    = n
            //      GetTileAt(3, 3) = 3 + (3 * 4)   = 15    = p

            ITile tile = null;

            try
            {
                tile = Tiles[y + (x * _size)];
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Unable to get tile at: {0}, {1} - {2}", x, y, ex.ToString()));
            }

            return tile;
        }

    }
}