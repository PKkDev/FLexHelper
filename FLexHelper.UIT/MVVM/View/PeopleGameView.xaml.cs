using FLexHelper.UIT.MVVM.Model.PeopleGame;
using FLexHelper.UIT.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FLexHelper.UIT.MVVM.View
{
    public partial class PeopleGameView : UserControl
    {
        PeopleGameViewModel2 viewModel;

        public PeopleGameView()
        {
            InitializeComponent();
            DataContext = viewModel = new PeopleGameViewModel2(GameMap, null);
            originalGridW = 0;// GameMapGrid.Width;
            originalGridH = 0;// GameMapGrid.Height;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
                viewModel.AcceptClose();
        }

        private double originalGridW = 0;
        private double originalGridH = 0;
        //private void button_ClickSSSSS(object sender, RoutedEventArgs e)
        //{
        //    var scleV = slider.Value;

        //    GameMapGrid.Width = originalGridW * scleV;
        //    GameMapGrid.Height = originalGridH * scleV;
        //    GameMapGrid.RenderTransform = new ScaleTransform() { ScaleX = scleV, ScaleY = scleV };
        //}

        private void MapScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (viewModel != null)
                viewModel.OnMapScale(MapScale.Value, originalGridW, originalGridH);
        }

        private void GameSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (viewModel != null)
                viewModel.OnGameSpeed(GameSpeed.Value);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Up:
                    viewModel.MovePeople(PeopleMoveDirection.Up);
                    break;
                case Key.Down:
                    viewModel.MovePeople(PeopleMoveDirection.Down);
                    break;
                case Key.Left:
                    viewModel.MovePeople(PeopleMoveDirection.Left);
                    break;
                case Key.Right:
                    viewModel.MovePeople(PeopleMoveDirection.Right);
                    break;
            }
        }
    }
}
