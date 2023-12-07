using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Xceed.Wpf.Toolkit;
namespace Ref
{
    class ListView_Search
    {
            public static void TextSearchFilter(ICollectionView filteredView, WatermarkTextBox textBox)
            {
                string filterText = "";

                filteredView.Filter = delegate(object obj)
                {
                    if (String.IsNullOrEmpty(filterText))
                        return true;

                    string str = obj as string;
                    if (String.IsNullOrEmpty(str))
                        return false;

                    int index = str.IndexOf(
                        filterText,
                        0,
                        StringComparison.InvariantCultureIgnoreCase);

                    return index > -1;
                };

                textBox.TextChanged += delegate
                {
                    filterText = textBox.Text;
                    filteredView.Refresh();
                };

            }
        }
    
}
