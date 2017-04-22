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

namespace CallBook
{
    /// <summary>
    /// Interaction logic for dataViewItem.xaml
    /// </summary>
    public partial class dataViewItem : UserControl
    {
        public bool selected = false;
        public dataViewItem()
        {
            InitializeComponent();
        }

        private void phone_MouseDown(object sender, MouseButtonEventArgs e)
        {
          //  MessageBox.Show("Call:" + phone.Text);
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            DataBaseItem dbItem = this.Tag as DataBaseItem;
            System.Diagnostics.Process.Start("https://www.google.com/maps?q=" + dbItem.county + " " + dbItem.city + " " + dbItem.street + " " + dbItem.house);
        }

        public void Select(bool seleted)
        {
            //#E57AADBB unselect 
            //#E5D6EEF3 
            if (seleted) this.Background = new SolidColorBrush(Color.FromArgb(0xE5, 0xD6, 0xEE, 0xF3));
            else
                this.Background = new SolidColorBrush(Color.FromArgb(0xE5, 0x7A, 0xAD, 0xBB));
            this.selected = seleted;
        }
    }
}
