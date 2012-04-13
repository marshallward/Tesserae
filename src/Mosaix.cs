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
        public Dictionary<uint, string> idSheet;
        public List<uint[,]> layerID;     // layerID[x,y]

        public int width, height;
        
        // Temporary
        public Map tmxMap;
        
        public Canvas(Map mapInput, ContentManager cntMgr)
        {
            tmxMap = mapInput;
            
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
                var widthCount = ts.image.width / ts.tileWidth;
                var heightCount = ts.image.height / ts.tileHeight;
                
                // Pre-compute tileset rectangles
                for (var j = 0; j < heightCount; j++)
                {
                    for (var i = 0; i < widthCount; i++)
                    {
                        var x = ts.spacing + i*(ts.tileWidth + ts.margin);
                        var y = ts.spacing + j*(ts.tileHeight + ts.margin);
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
    }
}
