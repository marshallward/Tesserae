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
            // Loop hoisting (Determined from Canvas)
            var iStart = 0;
            var iEnd = map.Width;
            
            var jStart = 0;
            var jEnd = map.Height;
            
            // Ignorant method: draw the entire map
            foreach (var idMap in layerID)
            {
                for (var i = iStart; i < iEnd; i++)
                {
                    for (var j = jStart; j < jEnd; j++)
                    {
                        var id = idMap[i,j];
                        
                        // Skip unmapped cells
                        if (id == 0) continue;
                        
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
        
        // This routine nearly duplicates TiledSharp's ReadXML (merge?)
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
    
    
    public class Tile
    {
        public int pWidth;      // Tile width in pixels
        public int pHeight;     // Tile height in pixels
    }
}