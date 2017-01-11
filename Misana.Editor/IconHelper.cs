using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor
{
    public static class IconHelper
    {
        static ImageList _imageList;
        public static ImageList ImageList
        {
            get
            {
                if (_imageList == null)
                {
                    _imageList = new ImageList();
                    _imageList.Images.Add("Map", Properties.Resources.IconGlobe);
                    _imageList.Images.Add("Area", Properties.Resources.IconMap);
                    _imageList.Images.Add("Layer", Properties.Resources.IconLayers);
                }
                return _imageList;
            }
        }
    }
}
