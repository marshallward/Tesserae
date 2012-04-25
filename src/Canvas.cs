using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace Tesserae
{
    public class Canvas
    {
        // Window-defined properties
        public int pWidth;          // Viewport width in pixels
        public int pHeight;         // Viewport height in pixels
        public float aspectRatio;   // Viewport width-to-height ratio
        
        // User-defined (or derived) quantities
        public int tWidth;      // # of (full) tiles per width
        public int tHeight;     // # of (full) tiles per height
        public int tHalo;       // # of rendered tiles outside viewport
        
        // Camera position
        public int pX;      // Camera X position in pixels
        public int pY;      // Camera Y position in pixels
        public int tX;      // Tile.X containing camera
        public int tY;      // Tile.Y containing camera
        
        // Loop index bounds
        public int tXStart;
        public int tXEnd;
        public int tYStart;
        public int tYEnd;
        
        // Mosaic parameters
        public int pTileWidth;      // Tile width in pixels
        public int pTileHeight;     // Tile height in pixels
        public float tileScale;     // Displayed / actual pixel height
        
        // Necessary?
        Game game;
        public Vector2 camera;
        
        public Canvas(Game gameInput)
        {
            game = gameInput;
            
            pWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            pHeight = game.GraphicsDevice.Viewport.Bounds.Height;
            aspectRatio = (float)pWidth / (float)pHeight;
            
            // Testing
            pTileWidth = pTileHeight = 16;
            tHeight = 16;
            
            Console.WriteLine("pWidth, pTileWidth, tileScale: {0}, {1}, {2}", pWidth, pTileWidth, tileScale);
            
            // Determine other tilecounts based on viewport pixel size
            tileScale = (float)pHeight / (float)(pTileHeight * tHeight);
            tWidth = (int)Math.Round(pWidth / (pTileWidth * tileScale));

            // Initialize camera
            tX = tWidth / 2;
            tY = tHeight / 2;
            pX = tX * pTileWidth;
            pY = tY * pTileHeight;
            
            // Loop index bounds
            tXStart = tX - tWidth / 2 + 1;
            tXEnd = tX + tWidth / 2 + 1;
            
            // Junk
            camera = new Vector2((float)pX * tileScale, (float)pY * tileScale);
            
            Console.WriteLine("tX, tWidth: {0}, {1}", tX, tWidth);
            
            game.Window.ClientSizeChanged += new EventHandler<EventArgs>
                (UpdateViewport);
        }
        
        public void UpdateViewport(object sender, EventArgs e)
        {
            // Disable this event?
            
            pWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            pHeight = game.GraphicsDevice.Viewport.Bounds.Height;
            aspectRatio = (float)pWidth / (float)pHeight;
            
            tileScale = (float)pHeight / (float)(tHeight * 16);
            
            // Testing
            Console.WriteLine(" Pixel Width: {0}", pWidth);
            Console.WriteLine("Pixel Height: {0}", pHeight);
            Console.WriteLine("Aspect Ratio: {0}", aspectRatio);
            
            // Re-enable this event?
        }
    }
}
