using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.CustomControls
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

    public class CustomAlternateRowColorListView : ListView
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
                    //Grey background
                    listViewItem.Background = new SolidColorBrush(Color.FromArgb(223, 223, 223, 223));
                }
                else
                {
                    //White background
                    listViewItem.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                }
            }

        }
    }
}
