using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace Tesserae
{
    // 
    public class Mosaic : DrawableGameComponent
    {
        public Dictionary<string, Texture2D> spriteSheet;
        public Dictionary<uint, Rectangle> tileRect;
        public Dictionary<uint, string> idSheet;
        public List<uint[,]> layerID;     // layerID[x,y]

        public int width, height;
        
        // Temporary
        public Map tmxMap;
        
        public Mosaic(Game game, string mapName) : base(game)
        {
            // Temporary code
            tmxMap = new Map(mapName);
            var cntMgr = game.Content;
            
            width = tmxMap.width;
            height = tmxMap.height;
            
            // Load spritesheets
            // Note: Currently path-based
            spriteSheet = new Dictionary<string, Texture2D>();
            tileRect = new Dictionary<uint, Rectangle>();
            idSheet = new Dictionary<uint, string>();
            
            foreach (Tileset ts in tmxMap.tileset)
            {
                var tsPath = Path.GetFileNameWithoutExtension(ts.image.source);
                spriteSheet.Add(ts.Name, cntMgr.Load<Texture2D>(tsPath));
                
                // Loop hoisting
                var widthCount = (ts.image.width - 2*ts.margin)
                                    / (ts.tileWidth + ts.spacing);
                var heightCount = (ts.image.height - 2*ts.margin)
                                    / (ts.tileHeight + ts.spacing);
                
                // Pre-compute tileset rectangles
                for (var j = 0; j < heightCount; j++)
                {
                    for (var i = 0; i < widthCount; i++)
                    {
                        var x = ts.margin + i*(ts.tileWidth + ts.spacing);
                        var y = ts.margin + j*(ts.tileHeight + ts.spacing);
                        var rect = new Rectangle(x, y,
                                                 ts.tileWidth, ts.tileHeight);
                        uint id = ts.firstGid + (uint)(i + j*widthCount);
                        idSheet.Add(id, ts.Name);
                        tileRect.Add(id, rect);
                    }
                }
                
                // Ignore properties for now
            }
            
            // Load id maps
            layerID = new List<uint[,]>();
            foreach (Layer layer in tmxMap.layer)
            {
                var idMap = new uint[width, height];
                foreach (LayerTile t in layer.tile)
                {
                    idMap[t.x, t.y] = t.gid;
                }
                
                layerID.Add(idMap);
                
                // Ignore properties for now
            }
        }
        
        public void Draw(SpriteBatch batch)
        {
            // Ignorant method: draw the entire map
            foreach (var idMap in layerID)
                for (var i = 0; i < tmxMap.width; i++)
                    for (var j = 0; j < tmxMap.height; j++)
                    {
                        var id = idMap[i,j];
                        
                        // Skip unmapped cells
                        if (id == 0) continue;
                        
                        var position = new Vector2(
                                            (float)(tmxMap.tileWidth * i),
                                            (float)(tmxMap.tileHeight * j));
                        batch.Draw(spriteSheet[idSheet[id]], position,
                            tileRect[id], Color.White);
                    }
        }   // end Draw
    } // End Canvas
    
    
    // Don't worry about updating this information at the moment, just focus on
    // integrating its data with Draw
    public class Canvas
    {
        // User-defined (or derived) quantities
        public int tWidth;      // # of (full) tiles per width
        public int tHeight;     // # of (full) tiles per height
        public int tHalo;       // # of rendered tiles outside viewport
        
        // Window-defined properties
        public int pWidth;          // Viewport width in pixels
        public int pHeight;         // Viewport height in pixels
        public double aspectRatio;  // Viewport width-to-height ratio
        
        public Canvas(Game game)
        {
            pWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            pHeight = game.GraphicsDevice.Viewport.Bounds.Height;
            aspectRatio = (double)pWidth / (double)pHeight;
            
            game.Window.ClientSizeChanged += new EventHandler<EventArgs>
                (Window_ClientSizeChanged);
        }
        
        public void UpdateViewport(Game game)
        {
            pWidth = game.GraphicsDevice.Viewport.Bounds.Width;
            pHeight = game.GraphicsDevice.Viewport.Bounds.Height;
            aspectRatio = (float)pWidth / (float)pHeight;
        }
        
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            //UpdateViewport(window);
        }
    }
}
