using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace Mosaix
{
    public class Canvas
    {
        public Dictionary<string, Texture2D> spriteSheet;
        public Dictionary<uint, Rectangle> tileRect;
        public uint[,] idMap;
        
        // Temporary
        public Map tmxMap;
        public int winWidth, winHeight;    // Window size
        
        public Canvas(Map mapInput, ContentManager cntMgr)
        {
            // Note: Currently filepath-based
            tmxMap = mapInput;
            
            // Load spritesheets
            spriteSheet = new Dictionary<string, Texture2D>();
            foreach (Tileset ts in tmxMap.tileset)
            {
                var tsPath = Path.GetFileNameWithoutExtension(ts.image.source);
                spriteSheet.Add(ts.Name, cntMgr.Load<Texture2D>(tsPath));
                
                // Loop hoisting
                var tsWidth = ts.image.width / ts.tileWidth;
                var tsHeight = ts.image.height / ts.tileHeight;
                
                // Pre-compute tileset rectangles
                tileRect = new Dictionary<uint, Rectangle>();
                for (var j = 0; j < tsHeight; j++)
                {
                    for (var i = 0; i < tsWidth; i++)
                    {
                        var x = ts.spacing + i*(ts.tileWidth + ts.margin);
                        var y = ts.spacing + j*(ts.tileHeight + ts.margin);
                        var rect = new Rectangle(x, y,
                                                 ts.tileWidth, ts.tileHeight);
                        uint id = ts.firstGid + (uint)(i + j*tsWidth);
                        tileRect.Add(id, rect);
                    }
                }
                
                // Load tile maps
                {
                    // Load maps
                    // Pre-compute window rectangles
                }
            }
            
            // Load tile ID maps
            foreach (var layer in tmxMap.layer)
            {
                
            }
            
            // Temporary
            winWidth = tmxMap.width * tmxMap.tileWidth;
            winHeight = tmxMap.height * tmxMap.tileHeight;
        }
        
        public void Draw(SpriteBatch batch)
        {
            
        }
    }
}
