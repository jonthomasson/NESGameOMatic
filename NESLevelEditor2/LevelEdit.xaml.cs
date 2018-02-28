using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Drawing;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace NESLevelEditor2
{
    /// <summary>
    /// Interaction logic for LevelEdit.xaml
    /// </summary>
    public partial class LevelEdit : Window
    {
        private static Entities _db;
        private static string FileName = "";
        private static string DumpName = "";
        private static string ConfigName = "";
        private System.Drawing.Image[] _blocks = new System.Drawing.Image[0];
        public int LevelId { get; set; }
        public int GameId { get; set; }

        public LevelEdit()
        {
            InitializeComponent();
            _db = new Entities();

            this.Loaded += new RoutedEventHandler(LevelEdit_Loaded);
           
        }

        /// <summary>
        /// called once Level Edit form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LevelEdit_Loaded(object sender, RoutedEventArgs e)
        {
            FileName = AppDomain.CurrentDomain.BaseDirectory + "/Games/" + _db.Games.FirstOrDefault(x => x.Id == GameId)?.ROM;
            ConfigName = AppDomain.CurrentDomain.BaseDirectory + "/Games/" + _db.Levels.FirstOrDefault(x => x.Id == LevelId)?.Config;
            
            //initalize level parameters
            InitGlobalData();

            //load blocks
            LoadBlocks();

            //load map
        }

        /// <summary>
        /// Loads Blocks for level editor
        /// </summary>
        private void LoadBlocks()
        {
            var videoNes = new Video();
            _blocks = videoNes.makeBigBlocks(0, 0, 0, 0, MapViewType.Tiles, MapViewType.Tiles, 0);

           

            //UtilsGui.resizeBlocksScreen(_blocks, blocksScreen, layers[0].blockWidth, layers[0].blockHeight, curScale);

            //redraw blocks
            //BlocksScreen.Source. = _blocks[1];
            BlocksScreen.Source = GetImageStream(_blocks[1]);
           // BlocksScreen.InvalidateVisual();
        }

        public static BitmapSource GetImageStream(System.Drawing.Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmpPt,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        private void blocksScreen_OnRender(object sender, PaintEventArgs e)
        {
            //if (!fileLoaded)
            //    return;
            var g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            //var visibleRect = UtilsGui.getVisibleRectangle(pnBlocks, blocksScreen);
            MapEditor.RenderAllBlocks(e.Graphics, BlocksScreen, _blocks, 32, 32, null, 2, 0, true);
        }

        private void InitGlobalData()
        {
            Globals.loadData(FileName, "", ConfigName);
            
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }
    }
}
