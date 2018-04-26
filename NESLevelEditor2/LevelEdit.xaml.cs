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
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Rectangle = System.Windows.Shapes.Rectangle;

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
        private static int CurrentScreen = 0;
        private static int CurrentSelectedBlock = 0;
        private static int CurrentRowBlock = 0;
        private static int CurrentColBlock = 0;
        private static Border CurrentBorderBlock;
        private static string ConfigName = "";
        private static int ScreenWidth = 8;
        private static int ScreenHeight = 8;
        private System.Drawing.Image[] _blocks = new System.Drawing.Image[0];
        private BlockLayer[] _layers = new BlockLayer[2] { new BlockLayer(), new BlockLayer() };
        private BigBlock[] _blockIndexes;
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
            LoadMap();
        }

        /// <summary>
        /// Loads Blocks for level editor
        /// </summary>
        private void LoadBlocks()
        {
            var videoNes = new Video();
            var numCols = 5;
            var currentColumn = 0;
            var currentRow = 0;
            
            _blocks = videoNes.makeBigBlocks(0, 0, 0, 0, MapViewType.Tiles, MapViewType.Tiles, 0);

            GrdBlocks.Children.Clear();

            //loop through blocks and add them to the wrap panel.
            for (int i = 0; i < _blocks.Length; i++)
            {

                var img = new System.Windows.Controls.Image
                {
                    Stretch = System.Windows.Media.Stretch.Fill,
                    Source = UtilsGDI.GetImageStream(_blocks[i])

                };

                var border = new Border
                {
                    BorderBrush = System.Windows.Media.Brushes.White,
                    BorderThickness = new Thickness(1),
                    Child = img
                };

                

                if (currentColumn == numCols - 1)
                {
                    //new row
                    var rowDef = new RowDefinition();
                    GrdBlocks.RowDefinitions.Add(rowDef);

                    GrdBlocks.Children.Add(border);
                    Grid.SetColumn(border, currentColumn);
                    Grid.SetRow(border, currentRow);

                    currentColumn = 0;
                    currentRow++;
                }
                else
                {
                    GrdBlocks.Children.Add(border);
                    Grid.SetColumn(border, currentColumn);
                    Grid.SetRow(border, currentRow);

                    currentColumn++;
                }

               
            }
            
        }
        

        private void LoadMap()
        {
            //get block indexes. Not sure this is needed...
            //_blockIndexes = ConfigScript.getBigBlocksRecursive(0, 0);

            //get screens
            _layers[0].screens = Utils.setScreens(0);
            CurrentScreen = 0; //default to first screen

            //initialize map screen grids
            for (int y = 0; y < ScreenHeight; y++)
            {
                //row
                MapScreen.RowDefinitions.Add(new RowDefinition());
                MapScreenPrev.RowDefinitions.Add(new RowDefinition());
                MapScreenNext.RowDefinitions.Add(new RowDefinition());


                for (int j = 0; j < ScreenWidth; j++)
                {
                    //column
                    MapScreen.ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Auto});
                    MapScreenPrev.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    MapScreenNext.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                }
            }

            LoadMapScreenSelector();

            
            //MapScreenChanged();
            //LoadMapScreen(0, 8, 8, true, ref MapScreen);


        }

        /// <summary>
        /// loads the map screen selector
        /// </summary>
        private void LoadMapScreenSelector()
        {
            //first clear any children
            MapScreenSelector.Children.Clear();

            //add column for each screen to the MapScreenSelector grid
         
            for (int i = 0; i < _layers[0].screens.Length; i++)
            {
                var colDef = new ColumnDefinition();
                colDef.Width = GridLength.Auto;
                
                MapScreenSelector.ColumnDefinitions.Add(colDef);

                var colGrid = new Grid {HorizontalAlignment = HorizontalAlignment.Stretch};


                for (int y = 0; y < ScreenHeight; y++)
                {
                    //row
                    var rowDef = new RowDefinition();
                    colGrid.RowDefinitions.Add(rowDef);

                    for (int j = 0; j < ScreenWidth; j++)
                    {
                        //column
                        var col = new ColumnDefinition();
                        colGrid.ColumnDefinitions.Add(col);
                    }
                }

                //add image to map grid
                LoadMapScreen(i,ScreenWidth, ScreenHeight, false, ref colGrid);
                MapScreenSelector.Children.Add(colGrid);
                Grid.SetColumn(colGrid, i);

                
            }


            MapScreenChanged();

        }

        /// <summary>
        /// Loads an 8x8 screen onto the tile map
        /// </summary>
        /// <param name="screenNo">The 0 based screen number to load.</param>
        private void LoadMapScreen(int screenNo, int screenWidth, int screenHeight, bool hasBorder, ref Grid MapGrid)
        {
            //int screenWidth = 8;
            //int screenHeight = 8;
            int[] screen = _layers[0].screens[screenNo];
            int blockNo = 0;

            //first we need to remove old contents of grid to mitigate memory consumption
            MapGrid.Children.Clear();

            //iterate through block indexes and add appropriate block to map screen
            for (int i = 0; i < screenHeight; i++)
            {
                //for each row
                for (int y = 0; y < screenWidth; y++)
                {
                    //for each column
                    var currentBlockIdx = screen[blockNo]; //grab the index of the current block
                    
                    var img = new System.Windows.Controls.Image
                    {
                        Stretch = System.Windows.Media.Stretch.Fill,
                        Source = UtilsGDI.GetImageStream(_blocks[currentBlockIdx])

                    };

                    var border = new Border
                    {
                        BorderBrush = System.Windows.Media.Brushes.White,
                        BorderThickness = new Thickness(hasBorder ? 1 : 0),
                        Child = img
                    };

                    MapGrid.ShowGridLines = false;
                    
                    //add image to map grid
                    MapGrid.Children.Add(border);
                    Grid.SetColumn(border,y);
                    Grid.SetRow(border,i);
                    blockNo++;
                }
            }
        }

        /// <summary>
        /// Loads data for selected game and level
        /// </summary>
        private void InitGlobalData()
        {
            Globals.loadData(FileName, "", ConfigName);
            ScreenHeight = ConfigScript.getScreenHeight(0);
            ScreenWidth = ConfigScript.getScreenWidth(0);

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        /// <summary>
        /// Gets the currently selected column in a Grid.
        /// </summary>
        /// <param name="grid">The Grid passed in by ref</param>
        /// <returns></returns>
        private int GridSelectedColumn(ref Grid grid)
        {
            var point = Mouse.GetPosition(grid);
            int col = 0;
            double accumulatedWidth = 0.0;

            //determine which column was clicked on
            foreach (var columnDefinition in grid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }

            return col;
        }

        /// <summary>
        /// Gets the currently selected row in a Grid
        /// </summary>
        /// <param name="grid">the Grid passed in by ref</param>
        /// <returns></returns>
        private int GridSelectedRow(ref Grid grid)
        {
            var point = Mouse.GetPosition(grid);
            int row = 0;
            double accumulatedHeight = 0.0;

            //determine which column was clicked on
            foreach (var rowDefinition in grid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            return row;
        }

        private void MapScreenSelector_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CurrentScreen = GridSelectedColumn(ref MapScreenSelector);

          
            MapScreenChanged();
        }

        private void MapScreenChanged()
        {
            //remove rectangle overlay from old selection
            foreach (var child in MapScreenSelector.Children)
            {
                if (child is Rectangle)
                {
                    MapScreenSelector.Children.Remove((System.Windows.UIElement)child);
                    break;
                }
            }
           
            //for selected column add semi transparent rectangle overlay
            var rectOverlay = new System.Windows.Shapes.Rectangle();
            rectOverlay.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
            rectOverlay.Opacity = .5;
            rectOverlay.Stretch = Stretch.Fill;
            rectOverlay.Fill.Opacity = .5;
            
            MapScreenSelector.Children.Add(rectOverlay);
            Grid.SetColumn(rectOverlay, CurrentScreen);

            

            LoadMapScreen(CurrentScreen, ScreenWidth, ScreenHeight, false, ref MapScreen);

            if (CurrentScreen > 0)
            {
                LoadMapScreen(CurrentScreen - 1, ScreenWidth, ScreenHeight, false, ref MapScreenPrev);

            }
            else
            {
                //clear child elements from MapScreenPrev
                MapScreenPrev.Children.Clear();
            }

            if (CurrentScreen < _layers[0].screens.Length - 1)
            {
                LoadMapScreen(CurrentScreen + 1, ScreenWidth, ScreenHeight, false, ref MapScreenNext);

            }
            else
            {
                //clear child elements from MapScreenNext
                MapScreenNext.Children.Clear();
            }

        }

        private void BtnNext_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentScreen < _layers[0].screens.Length - 1)
            {
                CurrentScreen++;
                MapScreenChanged();
            }
        }

        private void BtnPrev_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentScreen > 0)
            {
                CurrentScreen--;
                MapScreenChanged();
            }
        }
        

        private void GrdBlocks_OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Get currently selected column and row (Note, may refactor later to use Grid.GetRow / Grid.GetColumn ?)
            var col = GridSelectedColumn(ref GrdBlocks);
            var row = GridSelectedRow(ref GrdBlocks);

            //navigate to column/row to get cell
            var cell = GrdBlocks.Children.Cast<UIElement>()
                .First(ctl => Grid.GetRow(ctl) == row && Grid.GetColumn(ctl) == col);

            CurrentRowBlock = row;
            CurrentColBlock = col;

            //clear out old selection
            foreach (var child in GrdBlocks.Children)
            {
                if (child is Border)
                {
                    if (Equals(((Border) child).BorderBrush, System.Windows.Media.Brushes.Red))
                    {
                        ((Border)child).BorderBrush = System.Windows.Media.Brushes.White;
                    }

                }
            }

            //highlight new selection
            if (cell is Border)
            {
                //highlight border
                if (Equals(((Border)cell).BorderBrush, System.Windows.Media.Brushes.White))
                {
                    ((Border)cell).BorderBrush = System.Windows.Media.Brushes.Red;
                }
                

                CurrentBorderBlock = (Border)cell;

            }
        }

        private void GrdBlocks_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //select current block
            //get current index of currenetly selected block
            CurrentSelectedBlock = (CurrentRowBlock * 5) + CurrentColBlock; //(CurrentColBlock) * (CurrentRowBlock + 1);

            //clear out old selection
            foreach (var child in GrdBlocks.Children)
            {
                if (child is Border)
                {
                    if (Equals(((Border)child).BorderBrush, System.Windows.Media.Brushes.Blue))
                    {
                        ((Border)child).BorderBrush = System.Windows.Media.Brushes.White;
                    }

                }
            }
            CurrentBorderBlock.BorderBrush = System.Windows.Media.Brushes.Blue;


        }

       
        /// <summary>
        /// replaces the tile on the map screen with the one selected from the block/tile screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapScreen_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //populate current selected cell with CurrentSelectedBlock
            //Get currently selected column and row (Note, may refactor later to use Grid.GetRow / Grid.GetColumn ?)
            var col = GridSelectedColumn(ref MapScreen);
            var row = GridSelectedRow(ref MapScreen);

            //navigate to column/row to get cell
            var cell = MapScreen.Children.Cast<UIElement>()
                .First(ctl => Grid.GetRow(ctl) == row && Grid.GetColumn(ctl) == col);

            //I'll eventually rewrite this to use a copy of the layer so it's not rewritten whenever the map is redrawn...
            if (cell is Border)
            {
                
                var img = new System.Windows.Controls.Image
                {
                    Stretch = System.Windows.Media.Stretch.Fill,
                    Source = UtilsGDI.GetImageStream(_blocks[CurrentSelectedBlock])

                };

                ((Border) cell).Child = img;
            }

        }
    }
}
