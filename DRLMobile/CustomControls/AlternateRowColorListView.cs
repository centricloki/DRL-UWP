using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DRLMobile.CustomControls
{
    public class AlternateRowColorListView : ListView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var listViewItem = element as ListViewItem;
            if (listViewItem != null)
            {
                var index = IndexFromContainer(element);

                if (index % 2 == 0)
                {
                    listViewItem.Background = (SolidColorBrush)Application.Current.Resources["LoginBackgroundColor"];
                }
                else
                {
                    listViewItem.Background = new SolidColorBrush(Colors.White);
                }
            }

        }
    }
}
