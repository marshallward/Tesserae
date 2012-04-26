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

    // Canvas tracks three coordinate systems: 
    // 1. pScreenWidth/Height: The window pixel count
    // 2. pWidth/Height: The unscaled (virtual) pixel count
    // 3. tWidth/Height: The (full) tile count        
    public class Canvas
    {
        // Window-defined properties
        public int pScreenWidth;          // Viewport width in pixels
        public int pScreenHeight;         // Viewport height in pixels
        
        // Unscaled window pixel size
        public int pWidth;
        public int pHeight;
        
        // User-defined (or derived) quantities
        public int tMinWidth;
        public int tMinHeight;
        public int tWidth;      // # of (full) tiles per width
        public int tHeight;     // # of (full) tiles per height
        public int tHalo;       // # of rendered tiles outside viewport
        
        // Camera position
        public int pX;      // Camera X position in pixels
        public int pY;      // Camera Y position in pixels
        public int tX;      // Tile.X containing camera
        public int tY;      // Tile.Y containing camera
        
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
            
            // Screen pixel count
            pScreenWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            pScreenHeight = game.GraphicsDevice.Viewport.Bounds.Height;            
            
            // Probably the best control parameters:
            // Define minimum tile width/height, allow to expand
            tMinWidth = 15;
            tMinHeight = 15;
            
            // Testing (Input arguments?)
            pTileWidth = pTileHeight = 16;
            //tHeight = 15;
            
            // Determine the minimum scaling
            var xScale = (float)pScreenWidth / (pTileWidth * tMinWidth);
            var yScale = (float)pScreenHeight / (pTileHeight * tMinHeight);
            tileScale = Math.Min(xScale, yScale);
            tWidth = (int)Math.Round(pScreenWidth / (pTileWidth * tileScale));
            tHeight = (int)Math.Round(pScreenHeight / (pTileHeight * tileScale));
            
            // Determine other tilecounts based on viewport pixel size
            //tileScale = (float)pScreenHeight / (float)(pTileHeight * tHeight);
            //tWidth = (int)Math.Round(pScreenWidth / (pTileWidth * tileScale));
            
            // Virtual height is prescribed (i.e. window-independent)
            pHeight = (int)Math.Round(pScreenHeight / tileScale);
            pWidth = (int)Math.Round(pScreenWidth / tileScale);
            
            // Initialize camera on center or left/below centre pixel
            pX = (pWidth - 1) / 2;
            pY = (pHeight - 1) / 2;
            tX = pX / pTileWidth;
            tY = pY / pTileHeight;
            Console.WriteLine("tX, tWidth: {0}, {1}", tX, tWidth);
            
            camera = tileScale * (new Vector2((float)pX, (float)pY));
            
            game.Window.ClientSizeChanged += new EventHandler<EventArgs>
                (UpdateViewport);
        }
        
        public void UpdateViewport(object sender, EventArgs e)
        {
            // Disable this event?
            
            // Screen pixel count
            pScreenWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            pScreenHeight = game.GraphicsDevice.Viewport.Bounds.Height;
            
            // Determine the minimum scaling
            var xScale = (float)pScreenWidth / (pTileWidth * tMinWidth);
            var yScale = (float)pScreenHeight / (pTileHeight * tMinHeight);
            tileScale = Math.Min(xScale, yScale);
            tWidth = (int)Math.Round(pScreenWidth / (pTileWidth * tileScale));
            tHeight = (int)Math.Round(pScreenHeight / (pTileHeight * tileScale));
            
            pWidth = (int)Math.Round(pScreenWidth / tileScale);
            pHeight = (int)Math.Round(pScreenHeight / tileScale);
            
            // Initialize camera
            pX = (pWidth - 1) / 2;
            pY = (pHeight - 1) / 2;
            tX = pX / pTileWidth;
            tY = pY / pTileHeight;
            
            // Junk
            camera = new Vector2((float)pX * tileScale, (float)pY * tileScale);
            
            // Testing
            Console.WriteLine(" Pixel Width: {0}", pScreenWidth);
            Console.WriteLine("Pixel Height: {0}", pScreenHeight);
            
            // Re-enable this event?
        }
    }
}
