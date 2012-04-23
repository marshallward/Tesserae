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
        public Dictionary<TmxTileset, Texture2D> spriteSheet;
        public Dictionary<uint, Rectangle> tileRect;
        public Dictionary<uint, TmxTileset> idSheet;
        public List<uint[,]> layerID;     // layerID[x,y]
        
        public int tWidth, tHeight;
        
        // Temporary
        public TmxMap map;      // TMX data (try to remove this)
        public Canvas canvas;   // Viewport details
        public Tile tile;       // Tile element details
        
        public Mosaic(Game game, string mapName) : base(game)
        {
            // Temporary code
            map = new TmxMap(mapName);
            tWidth = map.Width;
            tHeight = map.Height;
            
            // Temporary code
            canvas = new Canvas(game);
            canvas.tHeight = 15;
            
            // Load spritesheets
            // Note: Currently path-based
            spriteSheet = new Dictionary<TmxTileset, Texture2D>();
            tileRect = new Dictionary<uint, Rectangle>();
            idSheet = new Dictionary<uint, TmxTileset>();
            
            foreach (TmxTileset ts in map.Tilesets)
            {
                var newSheet = GetSpriteSheet(ts.Image.Source);
                spriteSheet.Add(ts, newSheet);
                
                // Loop hoisting
                var wStart = ts.Margin;
                var wInc = ts.TileWidth + ts.Spacing;
                var wEnd = ts.Image.Width;
                
                var hStart = ts.Margin;
                var hInc = ts.TileHeight + ts.Spacing;
                var hEnd = ts.Image.Height;
                
                // Pre-compute tileset rectangles
                var id = ts.FirstGid;
                for (var h = hStart; h < hEnd; h += hInc)
                {
                    for (var w = wStart; w < wEnd; w += wInc)
                    {
                        var rect = new Rectangle(w, h,
                                                 ts.TileWidth, ts.TileHeight);
                        idSheet.Add(id, ts);
                        tileRect.Add(id, rect);
                        id += 1;
                    }
                }
                
                // Ignore properties for now
            }
            
            // Load id maps
            layerID = new List<uint[,]>();
            foreach (TmxLayer layer in map.Layers)
            {
                var idMap = new uint[tWidth, tHeight];
                foreach (TmxLayerTile t in layer.Tiles)
                {
                    idMap[t.X, t.Y] = t.GID;
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
                for (var i = 0; i < map.Width; i++)
                {
                    for (var j = 0; j < map.Height; j++)
                    {
                        var id = idMap[i,j];
                        
                        // Skip unmapped cells
                        if (id == 0) continue;
                        
                        // Precompute these?
                        // (Won't work unless done for all 256 sub-tile pixels)
                        var position = new Vector2(
                                    map.TileWidth * canvas.tileScale * i,
                                    map.TileHeight * canvas.tileScale * j);
                        
                        batch.Draw(spriteSheet[idSheet[id]], position,
                            tileRect[id], Color.White, 0.0f, Vector2.Zero,
                            canvas.tileScale, SpriteEffects.None, 0);
                    }
                }
            }
        }
        
        // This routine nearly duplicate TiledSharp's ReadXML. Sharing options?
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
    
    
    public class Tile
    {
        public int pWidth;      // Tile width in pixels
        public int pHeight;     // Tile height in pixels
    }
}
