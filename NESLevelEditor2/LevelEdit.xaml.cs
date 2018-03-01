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

            //loop through blocks and add them to the wrap panel.
            for (int i = 0; i < _blocks.Length - 1; i++)
            {

                var img = new System.Windows.Controls.Image
                {
                    Stretch = System.Windows.Media.Stretch.None,
                    Source = UtilsGDI.GetImageStream(_blocks[i])

                };

                var border = new Border
                {
                    BorderBrush = System.Windows.Media.Brushes.White,
                    BorderThickness = new Thickness(1)
                };

                border.Child = img;


                PnlBlocks.Children.Add(border);
            }
            
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
