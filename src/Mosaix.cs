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
        public List<uint[,]> layerID;     // idMap[x,y]
        
        // Temporary
        public Map tmxMap;
        public int width, height;
        
        public Canvas(Map mapInput, ContentManager cntMgr)
        {
            // Note: Currently path-based
            tmxMap = mapInput;
            
            width = tmxMap.width;
            height = tmxMap.height;
            
            // Load spritesheets
            spriteSheet = new Dictionary<string, Texture2D>();
            foreach (Tileset ts in tmxMap.tileset)
            {
                var tsPath = Path.GetFileNameWithoutExtension(ts.image.source);
                spriteSheet.Add(ts.Name, cntMgr.Load<Texture2D>(tsPath));
                
                // Loop hoisting
                var widthCount = ts.image.width / ts.tileWidth;
                var heightCount = ts.image.height / ts.tileHeight;
                
                // Pre-compute tileset rectangles
                tileRect = new Dictionary<uint, Rectangle>();
                for (var j = 0; j < heightCount; j++)
                {
                    for (var i = 0; i < widthCount; i++)
                    {
                        var x = ts.spacing + i*(ts.tileWidth + ts.margin);
                        var y = ts.spacing + j*(ts.tileHeight + ts.margin);
                        var rect = new Rectangle(x, y,
                                                 ts.tileWidth, ts.tileHeight);
                        uint id = ts.firstGid + (uint)(i + j*widthCount);
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
            //var rect = new Rectangle(0, 0, 100, 100);
            //batch.Draw (spriteSheet["towntiles"], rect, Color.White);
            
            // Ignorant method: draw the entire map
            foreach (var idMap in layerID)
            {
                for (var i = 0; i < tmxMap.width; i++)
                {
                    for (var j = 0; j < tmxMap.height; j++)
                    {
                        if (idMap[i,j] == 0) continue;
                        var position = new Vector2((float)(tmxMap.tileWidth * i),
                                                  (float)(tmxMap.tileHeight * j));
                        batch.Draw(spriteSheet["towntiles"], position,
                            tileRect[idMap[i,j]], Color.White);
                    }
                }
            }
        }
    }
}
