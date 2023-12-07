using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
namespace Ref
{
    /// <summary>
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class Inventory : UserControl
    {
        public List<String> file_list = new List<String>();
        public List<String> vm_list = new List<String>() ;

        public void set_filelist_inventory(List<string> x)
        {

            if (file_list.SequenceEqual(x)) { }
            else
            {
                file_list = x;

                Dispatcher.Invoke(() =>
                {
                    listview1.ItemsSource = file_list;
                });
            }
        }

        public void set_vmlist_inventory(List<string> x)
        {
            if (vm_list.SequenceEqual(x)) { }
            else
            {
                vm_list = x;
            }
                Dispatcher.Invoke(() =>
                {
                   listview2.ItemsSource = vm_list;
                   ICollectionView view1 = CollectionViewSource.GetDefaultView(vm_list);
                   Ref.ListView_Search.TextSearchFilter(view1, vm_search);
                });
            
        }

        public Inventory()
        {
            InitializeComponent();
            General.IISfile();
            file_list = General.GetFileList();
            Discover d = new Discover();
            d.send(d);
            vm_list = General.GetVMList();

            listview1.ItemsSource = file_list;
            listview2.ItemsSource = vm_list;

            ICollectionView view = CollectionViewSource.GetDefaultView(file_list);
            Ref.ListView_Search.TextSearchFilter(view, f_search);



            ICollectionView view1 = CollectionViewSource.GetDefaultView(vm_list);
            Ref.ListView_Search.TextSearchFilter(view1, vm_search);
        }

        private void vm_search_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
     
    }
}
