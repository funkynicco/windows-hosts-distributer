using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsHostDistributer
{
    public enum HostsSortingColumn
    {
        Unsorted,
        Name,
        IP
    }

    public class HostsListViewSorter : IComparer
    {
        public HostsSortingColumn Sorting { get; set; }
        public bool Inverted { get; set; }

        public HostsListViewSorter()
        {
            Sorting = HostsSortingColumn.Unsorted;
            Inverted = false;
        }
        
        private int InternalCompare(ListViewItem x, ListViewItem y)
        {
            switch (Sorting)
            {
                case HostsSortingColumn.Name:
                    return x.Text.CompareTo(y.Text);
                case HostsSortingColumn.IP:
                    return x.SubItems[1].Text.CompareTo(y.SubItems[1].Text);
            }

            return 0;
        }

        public int Compare(object x, object y)
        {
            var value = InternalCompare(
                x as ListViewItem,
                y as ListViewItem);

            if (Inverted && value != 0)
                value = -value;

            return value;
        }
    }
}
