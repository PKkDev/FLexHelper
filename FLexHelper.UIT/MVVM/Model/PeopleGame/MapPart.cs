using System.Windows;

namespace FLexHelper.UIT.MVVM.Model.PeopleGame
{
    public class MapPart
    {
        public UIElement UiElement { get; set; }

        public Point Position { get; set; }

        public TypeMapPart TypeMapPart { get; set; }
    }

    public enum TypeMapPart
    {
        Grass,
        Tree,
        Water,
        Rock,
        PeopleOnGrass,

        People,
        House,
        EmptyGrass
    }
}
