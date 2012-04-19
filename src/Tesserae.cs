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
    public class Mosaic : DrawableGameComponent
    {
        public Dictionary<string, Texture2D> spriteSheet;
        public Dictionary<uint, Rectangle> tileRect;
        public Dictionary<uint, string> idSheet;
        public List<uint[,]> layerID;     // layerID[x,y]

        public int width, height;
        
        // Temporary
        public Map tmxMap;      // TMX data (try to remove this)
        public Canvas canvas;   // Viewport details
        public Tile tile;       // Tile element details
        
        public Mosaic(Game game, string mapName) : base(game)
        {
            // Temporary code
            tmxMap = new Map(mapName);
            width = tmxMap.width;
            height = tmxMap.height;
            
            // Temporary code
            canvas = new Canvas(game);
            canvas.tHeight = 15;
            
            // Load spritesheets
            // Note: Currently path-based
            spriteSheet = new Dictionary<string, Texture2D>();
            tileRect = new Dictionary<uint, Rectangle>();
            idSheet = new Dictionary<uint, string>();
            
            foreach (Tileset ts in tmxMap.tileset)
            {
                var newSheet = GetSpriteSheet(ts.image.source);
                spriteSheet.Add(ts.Name, newSheet);
                
                // Loop hoisting
                var w_start = ts.margin;
                var w_inc = ts.tileWidth + ts.spacing;
                var w_end = ts.image.width;
                
                var h_start = ts.margin;
                var h_inc = ts.tileHeight + ts.spacing;
                var h_end = ts.image.height;
                
                // Pre-compute tileset rectangles
                var id = ts.firstGid;
                for (var h = h_start; h < h_end; h += h_inc)
                {
                    for (var w = w_start; w < w_end; w += w_inc)
                    {
                        var rect = new Rectangle(w, h,
                                                 ts.tileWidth, ts.tileHeight);
                        idSheet.Add(id, ts.Name);
                        tileRect.Add(id, rect);
                        id += 1;
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
            {
                for (var i = 0; i < tmxMap.width; i++)
                {
                    for (var j = 0; j < tmxMap.height; j++)
                    {
                        var id = idMap[i,j];
                        
                        // Skip unmapped cells
                        if (id == 0) continue;
                        
                        var position = new Vector2(
                                    tmxMap.tileWidth * canvas.tileScale * i,
                                    tmxMap.tileHeight * canvas.tileScale * j);
                        batch.Draw(spriteSheet[idSheet[id]], position,
                            tileRect[id], Color.White, 0.0f, Vector2.Zero,
                            canvas.tileScale, SpriteEffects.None, 0);
                    }
                }
            }
        }
        
        public Texture2D GetSpriteSheet(string filepath)
        {
            Texture2D newSheet;
            
            var asm = Assembly.GetEntryAssembly();
            var manifest = asm.GetManifestResourceNames();
            
            var fileResPath = filepath.Replace(
                Path.DirectorySeparatorChar.ToString(), ".");
            var fileRes = Array.Find(manifest, s => s.EndsWith(fileResPath));
            if (fileRes != null)
            {
                Stream imgStream = asm.GetManifestResourceStream(fileRes);
                newSheet = Texture2D.FromFile(this.GraphicsDevice, imgStream);
            }
            else
                newSheet = Texture2D.FromFile(this.GraphicsDevice, filepath);
            
            return newSheet;
        }
    }
    
    
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
        public float tileScale = 3.0f;
        
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
            
            tileScale = (float)pHeight / (float)(tHeight * 15);
            
            // Testing
            Console.WriteLine("Pixel Width: {0}", pWidth);
            Console.WriteLine("Pixel Height: {0}", pHeight);
            Console.WriteLine("Aspect Ratio: {0}", aspectRatio);
        }
    }
    
    
    public class Tile
    {
        public int pWidth;      // Tile width in pixels
        public int pHeight;     // Tile height in pixels
    }
}
