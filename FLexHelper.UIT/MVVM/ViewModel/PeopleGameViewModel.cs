using FLexHelper.UIT.Core;
using FLexHelper.UIT.MVVM.Model.PeopleGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FLexHelper.UIT.MVVM.ViewModel
{
    public class PeopleGameViewModel : ObservableObject
    {
        public List<MapPart> MapPart;
        public int MapPartSize = 20;

        private Random rnd = new Random();

        public PeoplePart MainPeople;

        private Canvas _gameMap;
        private Grid _gameMapGrid;

        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();

        private double _mapMaxHeight;
        private double _mapMinHeight;
        private double _mapMaxWidth;
        private double _mapMinWidth;

        public RelyCommand ReloadMapCommand { get; set; }


        private int _collectedWood;
        public int CollectedWood
        {
            get { return _collectedWood; }
            set { OnSetNewValue(ref _collectedWood, value); }
        }
        private int _collectedPeople;
        public int CollectedPeople
        {
            get { return _collectedPeople; }
            set { OnSetNewValue(ref _collectedPeople, value); }
        }
        private int _countHouse;
        public int CountHouse
        {
            get { return _countHouse; }
            set { OnSetNewValue(ref _countHouse, value); }
        }

        SynchronizationContext _uiContext { get; set; }

        public PeopleGameViewModel(Canvas gameMap, Grid GameMapGrid)
        {
            _gameMap = gameMap;
            _gameMapGrid = GameMapGrid;
            MapPart = new List<MapPart>();
            gameTickTimer.Tick += GameTickTimer_Tick;

            CollectedWood = 0;
            CollectedPeople = 0;
            CountHouse = 0;

            _mapMaxHeight = _gameMap.ActualHeight;
            _mapMinHeight = 0;
            _mapMaxWidth = _gameMap.ActualWidth;
            _mapMinWidth = 0;

            _uiContext = SynchronizationContext.Current;

            ReloadMapCommand = new RelyCommand(param =>
            {
                StartSimulation();
            });
        }

        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            MovePeople(PeopleMoveDirection.Down);
        }

        /// <summary>
        /// изменение масштаба карты
        /// </summary>
        /// <param name="mapScale"></param>
        /// <param name="originalGridW"></param>
        /// <param name="originalGridH"></param>
        public void OnMapScale(double mapScale, double originalGridW, double originalGridH)
        {
            var newMapScale = Math.Round(mapScale);

            _gameMapGrid.Width = originalGridW * mapScale;
            _gameMapGrid.Height = originalGridH * mapScale;
            _gameMapGrid.RenderTransform = new ScaleTransform() { ScaleX = mapScale, ScaleY = mapScale };

            // _gameMap.Height = _gameMap.Height * 1.5;
            // _gameMap.Width = _gameMap.Width * 1.5;

            //_mapMinHeight = _mapMinHeight * 2;
            //_mapMaxHeight = _mapMaxHeight * 2;
            //_mapMinWidth = _mapMaxWidth * 2;
            //_mapMaxWidth = _mapMaxWidth * 2;

            //MapPartSize = 5;
            //foreach (var mapPrt in MapPart)
            //{
            //    mapPrt.Position = new Point(mapPrt.Position.X / 2, mapPrt.Position.Y / 2);
            //    ((Rectangle)mapPrt.UiElement).Width = ((Rectangle)mapPrt.UiElement).Width / 2;
            //    ((Rectangle)mapPrt.UiElement).Height = ((Rectangle)mapPrt.UiElement).Height / 2;
            //}
            //MainPeople.Position = new Point(MainPeople.Position.X / 2, MainPeople.Position.Y / 2);
            //((Rectangle)MainPeople.UiElement).Width = ((Rectangle)MainPeople.UiElement).Width / 2;
            //((Rectangle)MainPeople.UiElement).Height = ((Rectangle)MainPeople.UiElement).Height / 2;

            //DrawMap(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);
        }

        /// <summary>
        /// изменения скорости игры
        /// </summary>
        /// <param name="gameSpeed"></param>
        public void OnGameSpeed(double gameSpeed)
        {
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(gameSpeed);
        }

        public void StartSimulation()
        {
            CollectedWood = 0;
            CollectedPeople = 0;
            CountHouse = 0;

            _mapMaxHeight = _gameMap.ActualHeight;
            _mapMinHeight = 0;
            _mapMaxWidth = _gameMap.ActualWidth;
            _mapMinWidth = 0;

            _gameMap.Focus();
            _gameMap.Children.Clear();
            MainPeople = new PeoplePart() { Position = new Point(MapPartSize * 5, MapPartSize * 5) };
            MapPart = ConstructInitialMap();
            DrawMap(0, 0);
            DrawMainPeople(0, 0);

            gameTickTimer.Interval = TimeSpan.FromMilliseconds(1000);
            gameTickTimer.IsEnabled = true;
        }

        private int countPartX = 0;
        private int countPartY = 0;

        private readonly int _countAllPeople = 1;
        private readonly int _countTree = 25;
        private readonly int _countWater = 2;
        private readonly int _countRock = 2;
        private Dictionary<TypeMapPart, SolidColorBrush> TypeMapPartColor = new Dictionary<TypeMapPart, SolidColorBrush>()
        {
            { TypeMapPart.Tree, Brushes.Brown },
            { TypeMapPart.Water, Brushes.Blue },
            { TypeMapPart.Rock, Brushes.Gray },
            { TypeMapPart.Grass, Brushes.Green },
            { TypeMapPart.People, (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE4B5") },
            { TypeMapPart.House, (SolidColorBrush)new BrushConverter().ConvertFrom("#000000") }
        };
        private Dictionary<TypeMapPart, string> TypeMapPartImg = new Dictionary<TypeMapPart, string>()
        {
            { TypeMapPart.Tree, "Resources/Img/tree.png" },
            { TypeMapPart.Grass, "Resources/Img/grass3.png" }
        };
        private List<MapPart> ConstructInitialMap()
        {
            int sq = (int)(_gameMap.ActualWidth * _gameMap.ActualHeight);
            int countPart = sq / (MapPartSize * MapPartSize);
            countPartX = (int)(_gameMap.ActualWidth / MapPartSize);
            countPartY = (int)(_gameMap.ActualHeight / MapPartSize);
            var countGrass = countPart - _countTree - _countWater - _countRock - _countAllPeople;

            //int sq = (int)(width * height);
            //int countPart = sq / (MapPartSize * MapPartSize);
            //countPartX = (int)(width / MapPartSize);
            //countPartY = (int)(height / MapPartSize);
            //var countGrass = countPart - _countTree - _countWater - _countRock - _countAllPeople;

            var result = new List<MapPart>();

            #region add Tree

            for (var i = 0; i < _countTree; i++)
            {
                var nextX = rnd.Next(0, countPartX) * MapPartSize;
                var nextY = rnd.Next(0, countPartY) * MapPartSize;

                var already = result.FirstOrDefault(x => x.Position.X == nextX && x.Position.Y == nextY);
                if (already != null)
                    i--;
                else
                {
                    TypeMapPartColor.TryGetValue(TypeMapPart.Tree, out var color);
                    TypeMapPartImg.TryGetValue(TypeMapPart.Tree, out var img);

                    Rectangle rect = new Rectangle
                    {
                        Width = MapPartSize,
                        Height = MapPartSize,
                        // Fill = color,
                        Fill = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                        },
                        ToolTip = "Tree"
                    };

                    result.Add(new MapPart()
                    {
                        TypeMapPart = TypeMapPart.Tree,
                        Position = new Point(nextX, nextY),
                        UiElement = rect
                    });
                }
            }

            #endregion

            #region add Water

            for (var i = 0; i < _countWater; i++)
            {
                double nextX = rnd.Next(0, countPartX) * MapPartSize;
                var nextY = rnd.Next(0, countPartY) * MapPartSize;

                var already = result.FirstOrDefault(x => x.Position.X == nextX && x.Position.Y == nextY);
                if (already != null)
                    i--;
                else
                {
                    TypeMapPartColor.TryGetValue(TypeMapPart.Water, out var color);
                    Rectangle rect = new Rectangle
                    {
                        Width = MapPartSize,
                        Height = MapPartSize,
                        Fill = color,
                        ToolTip = "Water"
                    };

                    result.Add(new MapPart()
                    {
                        TypeMapPart = TypeMapPart.Water,
                        Position = new Point(nextX, nextY),
                        UiElement = rect
                    });
                }
            }

            #endregion

            #region add Rock

            for (var i = 0; i < _countRock; i++)
            {
                var nextX = rnd.Next(0, countPartX) * MapPartSize;
                var nextY = rnd.Next(0, countPartY) * MapPartSize;

                var already = result.FirstOrDefault(x => x.Position.X == nextX && x.Position.Y == nextY);
                if (already != null)
                    i--;
                else
                {
                    TypeMapPartColor.TryGetValue(TypeMapPart.Rock, out var color);
                    Rectangle rect = new Rectangle
                    {
                        Width = MapPartSize,
                        Height = MapPartSize,
                        Fill = color,
                        ToolTip = "Rock"
                    };

                    result.Add(new MapPart()
                    {
                        TypeMapPart = TypeMapPart.Rock,
                        Position = new Point(nextX, nextY),
                        UiElement = rect
                    });
                }
            }

            #endregion

            #region add People

            for (var i = 0; i < _countAllPeople; i++)
            {
                var nextX = rnd.Next(0, countPartX) * MapPartSize;
                var nextY = rnd.Next(0, countPartY) * MapPartSize;

                var already = result.FirstOrDefault(x => x.Position.X == nextX && x.Position.Y == nextY);
                if (already != null)
                    i--;
                else
                {
                    TypeMapPartColor.TryGetValue(TypeMapPart.People, out var color);
                    Rectangle rect = new Rectangle
                    {
                        Width = MapPartSize,
                        Height = MapPartSize,
                        Fill = color,
                        ToolTip = "People"
                    };

                    result.Add(new MapPart()
                    {
                        TypeMapPart = TypeMapPart.People,
                        Position = new Point(nextX, nextY),
                        UiElement = rect
                    });
                }
            }

            #endregion

            #region add Grass

            for (var i = 0; i < countGrass; i++)
            {
                var nextX = rnd.Next(0, countPartX) * MapPartSize;
                var nextY = rnd.Next(0, countPartY) * MapPartSize;

                var already = result.FirstOrDefault(x => x.Position.X == nextX && x.Position.Y == nextY);
                if (already != null)
                    i--;
                else
                {
                    TypeMapPartColor.TryGetValue(TypeMapPart.Grass, out var color);
                    TypeMapPartImg.TryGetValue(TypeMapPart.Grass, out var img);
                    Rectangle rect = new Rectangle
                    {
                        Width = MapPartSize,
                        Height = MapPartSize,
                        // Fill = color,
                        Fill = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                        },
                        ToolTip = "Grass"
                    };

                    result.Add(new MapPart()
                    {
                        TypeMapPart = TypeMapPart.Grass,
                        Position = new Point(nextX, nextY),
                        UiElement = rect
                    });
                }
            }

            #endregion

            return result;
        }
        private void DrawMap(double offsetX, double offsetY)
        {
            var mapPartInView = MapPart
                .Where(x =>
                x.Position.X >= _mapMinWidth &&
                x.Position.X < _mapMaxWidth &&
                x.Position.Y >= _mapMinHeight &&
                x.Position.Y < _mapMaxHeight)
                .ToList();
            _gameMap.Children.Clear();
            if (MainPeople != null)
                MainPeople.UiElement = null;
            foreach (var part in mapPartInView)
            {
                _gameMap.Children.Add(part.UiElement);
                Canvas.SetTop(part.UiElement, part.Position.Y - offsetY);
                Canvas.SetLeft(part.UiElement, part.Position.X - offsetX);
            }
        }

        public void MovePeople(PeopleMoveDirection diection)
        {
            var moveDiection = (PeopleMoveDirection)rnd.Next(0, 4);
            // var moveDiection = PeopleMoveDirection.Right;
            // var moveDiection = diection;

            double nextX = MainPeople.Position.X;
            double nextY = MainPeople.Position.Y;
            switch (moveDiection)
            {
                case PeopleMoveDirection.Left:
                    {
                        nextX -= MapPartSize;
                        var diff = nextX - _mapMinWidth;
                        if (Math.Abs(diff) <= MapPartSize * 3)
                        {
                            _mapMaxWidth -= MapPartSize * 3;
                            _mapMinWidth -= MapPartSize * 3;

                            // _mapMaxHeight -= MapPartSize * 3;
                            // _mapMinHeight -= MapPartSize * 3;

                            ReconstructXMap(_mapMinWidth, _mapMinHeight, 3);
                        }
                        //if (nextX < 0)
                        //{
                        //    MovePeople();
                        //    return;
                        //}
                        break;
                    }
                case PeopleMoveDirection.Right:
                    {
                        nextX += MapPartSize;
                        var diff = _mapMaxWidth - nextX;
                        if (Math.Abs(diff) <= MapPartSize * 3)
                        {
                            var oldWidth = _mapMaxWidth;
                            _mapMaxWidth += MapPartSize * 3;
                            _mapMinWidth += MapPartSize * 3;

                            //_mapMaxHeight += MapPartSize * 3;
                            //_mapMinHeight += MapPartSize * 3;

                            ReconstructXMap(oldWidth, _mapMinHeight, 3);
                        }
                        //if (nextX >= _gameMap.ActualWidth)
                        //{
                        //    MovePeople();
                        //    return;
                        //}
                        break;
                    }
                case PeopleMoveDirection.Up:
                    {
                        nextY -= MapPartSize;
                        var diff = nextY - _mapMinHeight;
                        if (Math.Abs(diff) <= MapPartSize * 3)
                        {
                            _mapMaxHeight -= MapPartSize * 3;
                            _mapMinHeight -= MapPartSize * 3;

                            //_mapMaxWidth -= MapPartSize * 3;
                            //_mapMinWidth -= MapPartSize * 3;

                            ReconstructMap(_mapMinWidth, _mapMinHeight, 3);
                        }
                        //if (nextY < 0)
                        //{
                        //    MovePeople();
                        //    return;
                        //}
                        break;
                    }
                case PeopleMoveDirection.Down:
                    {
                        nextY += MapPartSize;
                        var diff = _mapMaxHeight - nextY;
                        if (Math.Abs(diff) <= MapPartSize * 3)
                        {
                            var oldHeight = _mapMaxHeight;
                            _mapMaxHeight += MapPartSize * 3;
                            _mapMinHeight += MapPartSize * 3;

                            //_mapMaxWidth += MapPartSize * 3;
                            //_mapMinWidth += MapPartSize * 3;

                            ReconstructMap(_mapMinWidth, oldHeight, 3);
                        }
                        //if (nextY >= _mapMaxHeight)
                        //{
                        //    MovePeople();
                        //    return;
                        //}
                        break;
                    }
            }

            var res = CheckCollision(nextX, nextY);
            if (!res)
            {
                MovePeople(PeopleMoveDirection.Down);
                return;
            }

            double oldX = MainPeople.Position.X;
            double oldY = MainPeople.Position.Y;
            MainPeople.Position = new Point(nextX, nextY);
            DrawMainPeople(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);

            if (MainPeople.CountInnerPeople >= 3 && MainPeople.CountWood >= 5)
            {
                BuildHouse(oldX, oldY);
            }
        }

        private void ReconstructXMap(double startX, double startY, int countNewCol)
        {
            double nextX = startX;
            double nextY = startY;
            int countColumn = 0;

            bool doneDrawingBackground = false;

            while (!doneDrawingBackground)
            {
                var already = MapPart.FirstOrDefault(x => x.Position.X == nextX && x.Position.Y == nextY);
                if (already == null)
                {
                    var typeBlock = GetRandomMapPart();
                    TypeMapPartColor.TryGetValue(typeBlock, out var color);
                    TypeMapPartImg.TryGetValue(typeBlock, out var img);

                    Rectangle rect = new Rectangle
                    {
                        Width = MapPartSize,
                        Height = MapPartSize,
                        // Fill = color,
                        Fill = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                        },
                        ToolTip = Enum.GetName(typeof(TypeMapPart), typeBlock)
                    };

                    MapPart.Add(new MapPart()
                    {
                        TypeMapPart = typeBlock,
                        Position = new Point(nextX, nextY),
                        UiElement = rect
                    });
                }

                nextY += MapPartSize;

                if (nextY >= _mapMaxHeight)
                {
                    nextY = startY;
                    nextX += MapPartSize;
                    countColumn++;
                }

                if (countColumn >= countNewCol)
                    doneDrawingBackground = true;
            }
            DrawMap(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);
        }

        private void ReconstructMap(double startX, double startY, int countNewRow)
        {
            double nextX = startX;
            double nextY = startY;
            int countRow = 0;

            bool doneDrawingBackground = false;

            while (!doneDrawingBackground)
            {
                var already = MapPart.FirstOrDefault(x => x.Position.X == nextX && x.Position.Y == nextY);
                if (already == null)
                {
                    var typeBlock = GetRandomMapPart();
                    TypeMapPartColor.TryGetValue(typeBlock, out var color);
                    TypeMapPartImg.TryGetValue(typeBlock, out var img);

                    Rectangle rect = new Rectangle
                    {
                        Width = MapPartSize,
                        Height = MapPartSize,
                        // Fill = color,
                        Fill = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                        },
                        ToolTip = Enum.GetName(typeof(TypeMapPart), typeBlock)
                    };

                    MapPart.Add(new MapPart()
                    {
                        TypeMapPart = typeBlock,
                        Position = new Point(nextX, nextY),
                        UiElement = rect
                    });
                }

                nextX += MapPartSize;

                if (nextX >= _mapMaxWidth)
                {
                    nextX = startX;
                    nextY += MapPartSize;
                    countRow++;
                }

                if (countRow >= countNewRow)
                    doneDrawingBackground = true;
            }
            DrawMap(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);
        }

        private TypeMapPart GetRandomMapPart()
        {
            var typeBlock = (TypeMapPart)rnd.Next(0, 1);

            return typeBlock;
        }

        private void BuildHouse(double oldX, double oldY)
        {
            MainPeople.CountInnerPeople -= 3;
            MainPeople.CountWood -= 5;

            CollectedWood -= 5;
            _collectedPeople -= 3;

            var searchMapPart = MapPart.FirstOrDefault(x => x.Position.X == oldX && x.Position.Y == oldY);
            if (searchMapPart != null)
            {
                TypeMapPartColor.TryGetValue(TypeMapPart.House, out var color);
                searchMapPart.TypeMapPart = TypeMapPart.House;
                ((Rectangle)searchMapPart.UiElement).Fill = color;
                ((Rectangle)searchMapPart.UiElement).ToolTip = "House";
                CountHouse++;
            }
        }

        private bool CheckCollision(double nextX, double nextY)
        {
            var result = false;

            var searchMapPart = MapPart.FirstOrDefault(x => x.Position.X == nextX && x.Position.Y == nextY);
            if (searchMapPart != null)
            {
                switch (searchMapPart.TypeMapPart)
                {
                    case TypeMapPart.Tree:
                        {
                            CollectedWood++;
                            MainPeople.CountWood++;

                            TypeMapPartColor.TryGetValue(TypeMapPart.Grass, out var color);
                            TypeMapPartImg.TryGetValue(TypeMapPart.Grass, out var img);
                            searchMapPart.TypeMapPart = TypeMapPart.Grass;
                            //((Rectangle)searchMapPart.UiElement).Fill = color;
                            ((Rectangle)searchMapPart.UiElement).Fill = new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                            };
                            ((Rectangle)searchMapPart.UiElement).ToolTip = "Grass";
                            result = true;
                            break;
                        }
                    case TypeMapPart.Rock:
                        result = false;
                        break;
                    case TypeMapPart.Water:
                        result = false;
                        break;
                    case TypeMapPart.House:
                        result = false;
                        break;
                    case TypeMapPart.People:
                        {
                            CollectedPeople++;
                            MainPeople.CountInnerPeople++;

                            TypeMapPartColor.TryGetValue(TypeMapPart.Grass, out var color);
                            TypeMapPartImg.TryGetValue(TypeMapPart.Grass, out var img);
                            searchMapPart.TypeMapPart = TypeMapPart.Grass;
                            //((Rectangle)searchMapPart.UiElement).Fill = color;
                            ((Rectangle)searchMapPart.UiElement).Fill = new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                            };
                            ((Rectangle)searchMapPart.UiElement).ToolTip = "Grass";
                            result = true;
                            break;
                        }
                    default:
                        result = true;
                        break;
                }
            }
            return result;
        }

        private void DrawMainPeople(double offsetX, double offsetY)
        {
            if (MainPeople.UiElement == null)
            {
                TypeMapPartColor.TryGetValue(TypeMapPart.People, out var color);
                MainPeople.UiElement = new Rectangle()
                {
                    Width = MapPartSize,
                    Height = MapPartSize,
                    Fill = color
                };
                _gameMap.Children.Add(MainPeople.UiElement);
                Canvas.SetZIndex(MainPeople.UiElement, 100);
                Canvas.SetTop(MainPeople.UiElement, MainPeople.Position.Y - offsetY);
                Canvas.SetLeft(MainPeople.UiElement, MainPeople.Position.X - offsetX);
            }
            else
            {
                Canvas.SetZIndex(MainPeople.UiElement, 100);
                Canvas.SetTop(MainPeople.UiElement, MainPeople.Position.Y - offsetY);
                Canvas.SetLeft(MainPeople.UiElement, MainPeople.Position.X - offsetX);
            }
        }

        public void AcceptClose()
        {
            gameTickTimer.Stop();
        }
    }
}
