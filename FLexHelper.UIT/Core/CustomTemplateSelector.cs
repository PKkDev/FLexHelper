using System.Windows;
using System.Windows.Controls;
using FLexHelper.UIT.MVVM.View;

namespace FLexHelper.UIT.Core
{
    public class CustomTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MouseMoverView { get; set; }
        public DataTemplate RDPConnectView { get; set; }
        public DataTemplate PeopleGameView { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return MouseMoverView;

            if (!(item is ViewTypes model))
                return MouseMoverView;

            ViewTypes value = (ViewTypes)item;
            switch (value)
            {
                case ViewTypes.MouseMoverView:
                    return MouseMoverView;
                case ViewTypes.RDPConnectView:
                    return RDPConnectView;
                case ViewTypes.PeopleGameView:
                    return PeopleGameView;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
