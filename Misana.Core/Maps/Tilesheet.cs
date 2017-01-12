using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Maps
{
    public class Tilesheet
    {
        public string Name { get; set; }
        public string TextureName { get; set; }
        public int TileCount { get; set; }
        public int Spacing { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public Bitmap Texture { get; set; }


        private class TilesheetFile
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public int Spacing { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public string Filename { get; set; }
        }

        public Tilesheet(string name, string texname, int count, int spacing, int width, int height, Bitmap texture)
        {
            Name = name;
            TextureName = texname;
            TileCount = count;
            Spacing = spacing;
            TileWidth = width;
            TileHeight = height;
            Texture = texture;
        }

        public static Tilesheet LoadTilesheet(string path, string name)
        {
            var text = File.ReadAllText(Path.Combine(path, name+".json"));
            TilesheetFile tf = JsonConvert.DeserializeObject<TilesheetFile>(text);

            return new Tilesheet(tf.Name,tf.Name, tf.Count, tf.Spacing, tf.Width, tf.Height, (Bitmap)Bitmap.FromFile(Path.Combine(path, tf.Filename+".png")));
        }
        
    }
}
