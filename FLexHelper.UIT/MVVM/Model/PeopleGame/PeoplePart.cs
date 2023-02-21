using System.Windows;

namespace FLexHelper.UIT.MVVM.Model.PeopleGame
{
    public class PeoplePart
    {
        public UIElement UiElement { get; set; }

        public Point Position { get; set; }

        public TypeMapPart TypeMapPart { get; set; }

        public int CountWood { get; set; } = 0;
        public int CountInnerPeople { get; set; } = 0;
        public int CountRock { get; set; } = 0;
    }

    public enum PeopleMoveDirection
    {
        Left,
        Right,
        Up,
        Down
    };
}
