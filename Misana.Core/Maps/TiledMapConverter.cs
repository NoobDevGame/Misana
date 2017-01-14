using System;
using System.Collections.Generic;
using System.IO;
using Misana.Core.Ecs;
using Newtonsoft.Json;

namespace Misana.Core.Maps
{
    public static class TiledMapConverter
    {
        #region FileObject

        private class FileArea
        {
            public int Height;
            public int Width;

            public int Tileheight;
            public int Tilewidth;

            public FileLayer[] Layers;
            public FileTileSets[] Tilesets;
        }



        private class FileTileSets
        {
            public int Firstgid;
            public string Image;

            public int Spacing;

            public int Tilecount;

            public int Tileheight;
            public int Tilewidth;

            public int Columns;

            public Dictionary<int, FileTileProperty> Tileproperties;
        }

        private class FileTileProperty
        {
            public bool Blocked;

            public TileProperty GetTileProperty()
            {
                TileProperty property = new TileProperty();
                property.Blocked = Blocked;

                return property;
            }
        }

        private class FileLayer
        {
            public int Height;
            public int Width;

            public string Name;

            public bool Visible;

            public string Type;

            public int[] Data;

            public FileObject[] Objects;
        }

        private class FileObject
        {
            public float Height;

            public float Width;

            public float X;

            public float Y;

            public string Type;

            public string Name;

            public FileObjectProperty Properties;
        }

        private class FileObjectProperty
        {
            public bool Blocked;
            public string Destinationarea;
            public bool Collisionevent;
            public int Entitytype;
            public string Class;

        }

        #endregion

        public static Area LoadArea(string path,int id)
        {
            if (File.Exists(path))
            {
                using (var fs = File.OpenRead(path))
                using (var sr = new StreamReader(fs))
                {
                    var mapjson = sr.ReadToEnd();
                    var mapobject = JsonConvert.DeserializeObject<FileArea>(mapjson);
                    var map = Convert(mapobject,id, Path.GetFileNameWithoutExtension(path));
                    return map;
                }
            }

            return null;
        }

        private static Area Convert(FileArea fa, int id, string name)
        {
            var area = new Area(name, id, fa.Width, fa.Height);

            List<MapTexture> tilesheets = new List<MapTexture>();
            foreach(var ts in fa.Tilesets)
            {
                FileInfo ti = new FileInfo(ts.Image);

                var index = ti.Name.LastIndexOf(".", StringComparison.Ordinal);
                string key = ti.Name;

                if (index != -1)
                    key = key.Remove(index);

                var contenttexture = new MapTexture(key, ts.Firstgid, ts.Tilecount, ts.Spacing, ts.Tileheight, ts.Tilewidth, ts.Columns);
                tilesheets.Add(contenttexture);
            }

            Dictionary<string, int> idLookup = new Dictionary<string, int>();

            foreach(var tilesheet in tilesheets)
            {
                var i = area.AddTilesheet(tilesheet.Key);
                idLookup.Add(tilesheet.Key, i);
            }

            List<Layer> layers = new List<Layer>();
            foreach(var l in fa.Layers)
            {
                List<Tile> tiles = new List<Tile>();
                int count = 0;
                foreach(var t in l.Data)
                {
                    var tilesheetname = "";
                    var tileId = 0;
                    var blocked = false;

                    if (t == 0)
                    {
                        tiles.Add(new Tile(0, 0, false));
                        continue;
                    }
                        


                    foreach(var tilesheet in tilesheets)
                    {
                        if (t >= tilesheet.Firstgid && t <= tilesheet.Firstgid + tilesheet.Tilecount)
                        {
                            tilesheetname = tilesheet.Key;
                            tileId = t - tilesheet.Firstgid+1;
                            break;
                        }
                    }



                    tiles.Add(new Tile(tileId, idLookup[tilesheetname], blocked));
                }

                Layer lay = new Layer(count, tiles.ToArray());
                layers.Add(lay);
                count++;

            }

            area.Layers = layers;

            return area;
        }

        #region Old (for "direct" Tiled Conversion)
        //private static Area Convert(FileArea fa,int id, string name)
        //{


        //    List<Layer> layers = new List<Layer>();

        //    for (int t = 0; t < fa.Tilesets.Length; t++)
        //    {
        //        var ft = fa.Tilesets[t];
        //        FileInfo textureinfo = new FileInfo(ft.Image);



        //        if (ft.Tileproperties != null)
        //        {
        //            foreach (var tile in ft.Tileproperties)
        //            {
        //                contenttexture.SetTileProperty(tile.Key, tile.Value.GetTileProperty());
        //            }
        //        }

        //        //area.MapTextures.Add(key, contenttexture);
        //    }

        //    for (int l = 0; l < fa.Layers.Length; l++)
        //    {
        //        var fl = fa.Layers[l];

        //        if (fl.Type == "objectgroup")
        //        {

        //        }
        //        else
        //        {
        //            //layers.Add(new Layer(l, fl.Data));
        //        }
        //    }

        //    Area area = new Area(name,id,fa.Width,fa.Height,Vector2.Zero,layers.ToArray() );




        //    return area;
        //}
        #endregion
    }
}