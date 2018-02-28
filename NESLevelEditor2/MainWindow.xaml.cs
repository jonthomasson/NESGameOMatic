using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NESLevelEditor2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Entities _db;
       
        public static int GameId = 0;
        public static int LevelId = 0;

        public MainWindow()
        {
            InitializeComponent();
            _db = new Entities();

            BindGames();
        }

        /// <summary>
        /// Binds the games to the Select Game list
        /// </summary>
        private void BindGames()
        {
            CboGame.ItemsSource = _db.Games.ToList();
        }

        /// <summary>
        /// Runs when a game is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CboGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //bind levels with selected gameId

            //MessageBox.Show(CboGame.SelectedValue.ToString());
            var gameId = int.Parse(CboGame.SelectedValue.ToString());
            GameId = gameId;
            BindLevels(gameId);
        }

        /// <summary>
        /// binds the levels combo box with the levels for the selected game
        /// </summary>
        /// <param name="gameId"></param>
        private void BindLevels(int gameId)
        {
            var levels = _db.Levels.Where(x => x.GameId == gameId);
            CboLevel.ItemsSource = levels.ToList();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {

            //FileName = tbFileName.Text;
            //DumpName = ConfigScript.showDumpFileField ? tbDumpName.Text : "";
            //ConfigName = tbConfigName.Text;
            //DialogResult = DialogResult.OK;
            //Close();

            //Properties.Settings.Default["FileName"] = FileName;
            //Properties.Settings.Default["DumpName"] = DumpName;
            //Properties.Settings.Default["ConfigName"] = ConfigName;
            //Properties.Settings.Default.Save();

            LevelEdit p = new LevelEdit();
            p.LevelId = LevelId;
            p.GameId = GameId;
            p.Show();

            //GrdEditor.Visibility = Visibility.Visible;
            //Grid.SetRowSpan(GrdEditor, 2);
           
            //Grid.SetRow(GrdEditor,0);
            //GrdSelection.Visibility = Visibility.Collapsed;
        }

        private void CboLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var levelId = int.Parse(CboLevel.SelectedValue.ToString());
            LevelId = levelId;
            //MessageBox.Show(LevelId.ToString());
        }
    }
}
