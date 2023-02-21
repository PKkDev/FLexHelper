using FLexHelper.UIT.Core;
using FLexHelper.UIT.MVVM.Model.PeopleGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FLexHelper.UIT.MVVM.ViewModel
{
    public class PeopleGameViewModel2 : ObservableObject
    {
        public List<MapPart> MapPart;
        public int MapPartSize = 20;

        public PeoplePart MainPeople;

        private Random rnd = new Random();

        private Canvas _gameMap;
        private Grid _gameMapGrid;

        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();

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
        private int _collectedRock;
        public int CollectedRock
        {
            get { return _collectedRock; }
            set { OnSetNewValue(ref _collectedRock, value); }
        }
        private int _countHouse;
        public int CountHouse
        {
            get { return _countHouse; }
            set { OnSetNewValue(ref _countHouse, value); }
        }

        private double _mapMinWidth;
        private double _mapMaxWidth;
        private double _mapMinHeight;
        private double _mapMaxHeight;

        public RelyCommand ReloadMapCommand { get; set; }

        public PeopleGameViewModel2(Canvas gameMap, Grid GameMapGrid)
        {
            _gameMap = gameMap;
            _gameMapGrid = GameMapGrid;
            MapPart = new List<MapPart>();

            gameTickTimer.Tick += GameTickTimer_Tick;

            ReloadMapCommand = new RelyCommand(param =>
            {
                StartSimulation();
            });
        }

        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            var moveDiection = (PeopleMoveDirection)rnd.Next(0, 4);
            MovePeople(moveDiection);
        }

        public void MovePeople(PeopleMoveDirection moveDiection)
        {
            double nextX = MainPeople.Position.X;
            double nextY = MainPeople.Position.Y;

            var canMove = true;

            switch (moveDiection)
            {
                case PeopleMoveDirection.Left:
                    {
                        nextX -= MapPartSize;
                        var diff = nextX - _mapMinWidth;
                        canMove = CheckCollision(nextX, nextY);
                        if (Math.Abs(diff) <= MapPartSize * 2 && canMove)
                        {
                            _mapMaxWidth -= MapPartSize * 2;
                            _mapMinWidth -= MapPartSize * 2;

                            DrawMap(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);
                        }
                        break;
                    }
                case PeopleMoveDirection.Right:
                    {
                        nextX += MapPartSize;
                        var diff = _mapMaxWidth - nextX;
                        canMove = CheckCollision(nextX, nextY);
                        if (Math.Abs(diff) <= MapPartSize * 2 && canMove)
                        {
                            var oldWidth = _mapMaxWidth;
                            _mapMaxWidth += MapPartSize * 2;
                            _mapMinWidth += MapPartSize * 2;

                            DrawMap(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);
                        }
                        break;
                    }
                case PeopleMoveDirection.Up:
                    {
                        nextY -= MapPartSize;
                        var diff = nextY - _mapMinHeight;
                        canMove = CheckCollision(nextX, nextY);
                        if (Math.Abs(diff) <= MapPartSize * 2 && canMove)
                        {
                            _mapMaxHeight -= MapPartSize * 2;
                            _mapMinHeight -= MapPartSize * 2;

                            DrawMap(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);
                        }
                        break;
                    }
                case PeopleMoveDirection.Down:
                    {
                        nextY += MapPartSize;
                        var diff = _mapMaxHeight - nextY;
                        canMove = CheckCollision(nextX, nextY);
                        if (Math.Abs(diff) <= MapPartSize * 2 && canMove)
                        {
                            var oldHeight = _mapMaxHeight;
                            _mapMaxHeight += MapPartSize * 2;
                            _mapMinHeight += MapPartSize * 2;

                            DrawMap(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);
                        }
                        break;
                    }
            }

            if (!canMove)
            {
                var moveDiectionNext = (PeopleMoveDirection)rnd.Next(0, 4);
                MovePeople(moveDiectionNext);
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

        private void BuildHouse(double oldX, double oldY)
        {
            MainPeople.CountInnerPeople -= 3;
            MainPeople.CountWood -= 5;

            CollectedWood -= 5;
            _collectedPeople -= 3;

            var searchMapPart = MapPart.FirstOrDefault(x => x.Position.X == oldX && x.Position.Y == oldY);
            if (searchMapPart != null)
            {
                TypeMapPartImg.TryGetValue(TypeMapPart.House, out var img);
                searchMapPart.TypeMapPart = TypeMapPart.House;
                ((Rectangle)searchMapPart.UiElement).Fill = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                };
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

                            TypeMapPartImg.TryGetValue(TypeMapPart.EmptyGrass, out var img);
                            searchMapPart.TypeMapPart = TypeMapPart.EmptyGrass;
                            ((Rectangle)searchMapPart.UiElement).Fill = new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                            };
                            ((Rectangle)searchMapPart.UiElement).ToolTip = "Grass";
                            result = true;
                            break;
                        }
                    case TypeMapPart.Rock:
                        {
                            CollectedRock++;
                            MainPeople.CountRock++;

                            TypeMapPartImg.TryGetValue(TypeMapPart.EmptyGrass, out var img);
                            searchMapPart.TypeMapPart = TypeMapPart.EmptyGrass;
                            ((Rectangle)searchMapPart.UiElement).Fill = new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                            };
                            ((Rectangle)searchMapPart.UiElement).ToolTip = "Grass";
                            result = true;
                            break;
                        }
                    case TypeMapPart.Water:
                        result = false;
                        break;
                    case TypeMapPart.House:
                        result = true;
                        break;
                    case TypeMapPart.PeopleOnGrass:
                        {
                            CollectedPeople++;
                            MainPeople.CountInnerPeople++;

                            TypeMapPartImg.TryGetValue(TypeMapPart.EmptyGrass, out var img);
                            searchMapPart.TypeMapPart = TypeMapPart.EmptyGrass;
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

        public void StartSimulation()
        {
            CollectedWood = 0;
            CollectedPeople = 0;
            CountHouse = 0;

            var _mapMinX = -500;
            var _mapMaxX = 500;
            var _mapMinY = -500;
            var _mapMaxY = 500;

            _mapMinWidth = 0;
            _mapMaxWidth = _gameMap.ActualWidth;
            _mapMinHeight = 0;
            _mapMaxHeight = _gameMap.ActualHeight;

            _gameMap.Focus();
            _gameMap.Children.Clear();

            //var countX = Math.Round((_mapMaxWidth - _mapMinWidth) / MapPartSize) / 2;
            //var initPosX = countX * MapPartSize;

            //var countY = Math.Round((_mapMaxHeight - _mapMinHeight) / MapPartSize) / 2;
            //var initPosY = countY * MapPartSize;

            //MainPeople = new PeoplePart() { Position = new Point(initPosX, initPosY) };

            MainPeople = new PeoplePart() { Position = new Point(MapPartSize * 5, MapPartSize * 5) };


            //if (_gameMap.ActualWidth > _mapMaxX)
            //{
            //    while (_mapMaxX < _gameMap.ActualWidth)
            //    {
            //        _mapMaxX += MapPartSize;
            //        _mapMinX -= MapPartSize;
            //    }
            //}

            //if (_gameMap.ActualHeight > _mapMaxY)
            //{
            //    while (_mapMaxY < _gameMap.ActualHeight)
            //    {
            //        _mapMaxY += MapPartSize;
            //        _mapMinY -= MapPartSize;
            //    }
            //}

            //if (_mapMaxWidth > _mapMaxX)
            //{
            //    _mapMinWidth = _mapMinX + _mapMaxWidth - _mapMaxX;
            //    _mapMaxWidth = _mapMaxX;
            //}

            //if (_mapMaxHeight > _mapMaxY)
            //{
            //    _mapMinHeight = _mapMinY + _mapMaxHeight - _mapMaxY;
            //    _mapMaxHeight = _mapMaxY;
            //}

            if (_gameMap.ActualWidth > _mapMaxX)
            {
                while (_mapMaxX < _gameMap.ActualWidth)
                {
                    _mapMaxX += MapPartSize;
                    _mapMinX -= MapPartSize;
                }
            }

            if (_gameMap.ActualHeight > _mapMaxY)
            {
                while (_mapMaxY < _gameMap.ActualHeight)
                {
                    _mapMaxY += MapPartSize;
                    _mapMinY -= MapPartSize;
                }
            }

            MapPart = ConstructInitialMap(_mapMinX, _mapMaxX, _mapMinY, _mapMaxY);
            ConstructMapBorder(_mapMinX, _mapMaxX, _mapMinY, _mapMaxY);

            DrawMap(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);
            DrawMainPeople(_mapMaxWidth - _gameMap.ActualWidth, _mapMaxHeight - _gameMap.ActualHeight);

            gameTickTimer.Interval = TimeSpan.FromMilliseconds(1000);
            gameTickTimer.IsEnabled = true;
        }

        private Dictionary<TypeMapPart, string> TypeMapPartImg = new Dictionary<TypeMapPart, string>()
        {
            { TypeMapPart.Tree, "Resources/Img/tree.png" },
            { TypeMapPart.Grass, "Resources/Img/grass3.png" },
            { TypeMapPart.People, "Resources/Img/people.png" },
            { TypeMapPart.Water, "Resources/Img/water.png" },
            { TypeMapPart.Rock, "Resources/Img/rock.png" },
            { TypeMapPart.PeopleOnGrass, "Resources/Img/peopleOnGrass.png" },
            { TypeMapPart.EmptyGrass, "Resources/Img/grass1.png" },
            { TypeMapPart.House, "Resources/Img/house.png" }
        };
        private List<MapPart> ConstructInitialMap(int mapMinX, int mapMaxX, int mapMinY, int mapMaxY)
        {
            var result = new List<MapPart>();

            double nextX = mapMinX;
            double nextY = mapMinY;
            int countRow = 0;
            bool doneDrawingBackground = false;

            //var borderX = new List<double>() { _mapMinX - MapPartSize, _mapMinX, _mapMinX + MapPartSize, _mapMaxX, _mapMaxX + MapPartSize };
            //var borderY = new List<double>() { _mapMinY - MapPartSize, _mapMinY, _mapMinY + MapPartSize, _mapMaxY, _mapMaxY + MapPartSize };

            while (!doneDrawingBackground)
            {
                TypeMapPart typeBlock = GetRandomMapPart();

                //if (borderX.Contains(nextX) || borderY.Contains(nextY))
                //    typeBlock = TypeMapPart.Water;
                //else
                //    typeBlock = GetRandomMapPart();

                TypeMapPartImg.TryGetValue(typeBlock, out var img);

                Rectangle rect = new Rectangle
                {
                    Width = MapPartSize,
                    Height = MapPartSize,
                    Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                    },
                    ToolTip = $"{Enum.GetName(typeof(TypeMapPart), typeBlock)}\n x:{nextX}\n y:{nextY}"
                };

                result.Add(new MapPart()
                {
                    TypeMapPart = typeBlock,
                    Position = new Point(nextX, nextY),
                    UiElement = rect
                });


                nextX += MapPartSize;

                if (nextX >= mapMaxX)
                {
                    nextX = mapMinX;
                    nextY += MapPartSize;
                    countRow++;
                }

                if (nextY >= mapMaxY)
                    doneDrawingBackground = true;
            }

            return result;
        }
        private TypeMapPart GetRandomMapPart()
        {
            var typeBlock = (TypeMapPart)rnd.Next(0, 5);

            return typeBlock;
        }
        private void ConstructMapBorder(int mapMinX, int mapMaxX, int mapMinY, int mapMaxY)
        {
            var borderWidth = 3;
            // left
            ReconstructXMap(mapMinX - borderWidth * MapPartSize, mapMinY - borderWidth * MapPartSize, borderWidth, mapMaxY);
            // right
            ReconstructXMap(mapMaxX, mapMinY - MapPartSize * borderWidth, borderWidth, mapMaxY);
            //top
            ReconstructYMap(mapMinX - borderWidth * MapPartSize, mapMinY - borderWidth * MapPartSize, borderWidth, mapMaxX);
            // bottom
            ReconstructYMap(mapMinX, mapMaxY, borderWidth, mapMaxX);
        }

        private void ReconstructXMap(double startX, double startY, int countNewCol, int mapMaxY)
        {
            double nextX = startX;
            double nextY = startY;
            int countColumn = 0;

            bool doneDrawingBackground = false;

            while (!doneDrawingBackground)
            {
                TypeMapPartImg.TryGetValue(TypeMapPart.Water, out var img);
                MapPart.Add(new MapPart()
                {
                    TypeMapPart = TypeMapPart.Water,
                    Position = new Point(nextX, nextY),
                    UiElement = new Rectangle
                    {
                        Width = MapPartSize,
                        Height = MapPartSize,
                        Fill = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                        },
                        ToolTip = $"Water\n x:{nextX}\n y:{nextY}"
                    }
                });

                nextY += MapPartSize;

                if (nextY >= mapMaxY + MapPartSize * countNewCol)
                {
                    nextY = startY;
                    nextX += MapPartSize;
                    countColumn++;
                }

                if (countColumn >= countNewCol)
                    doneDrawingBackground = true;
            }
        }
        private void ReconstructYMap(double startX, double startY, int countNewRow, int mapMaxX)
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
                    TypeMapPartImg.TryGetValue(TypeMapPart.Water, out var img);
                    MapPart.Add(new MapPart()
                    {
                        TypeMapPart = TypeMapPart.Water,
                        Position = new Point(nextX, nextY),
                        UiElement = new Rectangle
                        {
                            Width = MapPartSize,
                            Height = MapPartSize,
                            Fill = new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                            },
                            ToolTip = $"Water\n x:{nextX}\n y:{nextY}"
                        }
                    });
                }

                nextX += MapPartSize;

                if (nextX >= mapMaxX)
                {
                    nextX = startX;
                    nextY += MapPartSize;
                    countRow++;
                }

                if (countRow >= countNewRow)
                    doneDrawingBackground = true;
            }
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

        private void DrawMainPeople(double offsetX, double offsetY)
        {
            if (MainPeople.UiElement == null)
            {
                TypeMapPartImg.TryGetValue(TypeMapPart.People, out var img);
                MainPeople.UiElement = new Rectangle()
                {
                    Width = MapPartSize,
                    Height = MapPartSize,
                    Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri(img, UriKind.Relative))
                    },
                    ToolTip = "People"
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

        /// <summary>
        /// изменение масштаба карты
        /// </summary>
        /// <param name="mapScale"></param>
        /// <param name="originalGridW"></param>
        /// <param name="originalGridH"></param>
        public void OnMapScale(double mapScale, double originalGridW, double originalGridH)
        {
            var newMapScale = Math.Round(mapScale);

            //_gameMapGrid.Width = originalGridW * mapScale;
            //_gameMapGrid.Height = originalGridH * mapScale;
            //_gameMapGrid.RenderTransform = new ScaleTransform() { ScaleX = mapScale, ScaleY = mapScale };

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


    }
}
