using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Sniffer.Models
{
    public class ScrollingListView : ListView
    {
        public static readonly DependencyProperty FilterPredicateProperty = DependencyProperty.Register("AutoScroll", 
            typeof(bool), typeof(ListView), new PropertyMetadata(null));
        public bool AutoScroll
        {
            get { return (bool)GetValue(FilterPredicateProperty); }
            set { SetValue(FilterPredicateProperty, value); }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!AutoScroll || e.NewItems == null) return;
            var newItemCount = e.NewItems.Count;

            if (newItemCount > 0)
                ScrollIntoView(e.NewItems[newItemCount - 1]);

            base.OnItemsChanged(e);
        }
    }
}
