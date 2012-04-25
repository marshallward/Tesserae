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
        // User-defined (or derived) quantities
        public int tWidth;      // # of (full) tiles per width
        public int tHeight;     // # of (full) tiles per height
        public int tHalo;       // # of rendered tiles outside viewport
        
        // Window-defined properties
        public int pWidth;          // Viewport width in pixels
        public int pHeight;         // Viewport height in pixels
        public float aspectRatio;  // Viewport width-to-height ratio
        
        // Necessary?
        Game game;
        public float tileScale;
        
        public Canvas(Game inGame)
        {
            game = inGame;
            
            pWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            pHeight = game.GraphicsDevice.Viewport.Bounds.Height;
            aspectRatio = (float)pWidth / (float)pHeight;
            
            // Temp
            tileScale = 3.0f;
            
            game.Window.ClientSizeChanged += new EventHandler<EventArgs>
                (UpdateViewport);
        }
        
        public void UpdateViewport(object sender, EventArgs e)
        {
            pWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            pHeight = game.GraphicsDevice.Viewport.Bounds.Height;
            aspectRatio = (float)pWidth / (float)pHeight;
            
            tileScale = (float)pHeight / (float)(tHeight * 16);
            
            // Testing
            Console.WriteLine("Pixel Width: {0}", pWidth);
            Console.WriteLine("Pixel Height: {0}", pHeight);
            Console.WriteLine("Aspect Ratio: {0}", aspectRatio);
        }
    }
}
