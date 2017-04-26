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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//version 1
//test
namespace CallBook
{
    public partial class CallListWindow : Window
    {
        private bool dataBaseInit = false;
        private Point mouseClickPos;
        public CallListWindow()
        {
            InitializeComponent();

            Log.addToLog("test1");
            Log.addToLog("test2");
            Log.addToLog("test1");



            dataBaseInit = DataBase.Init();

            if (!dataBaseInit)
            {
                MessageBox.Show("DataBase init wrong");
            }
            else
            {
                Refresh();
            }

            
        }

        private void Refresh()
        {
            itemList.Children.Clear();
            
            if (dataBaseInit)
            {
                List<DataBaseItem> list = DataBase.GetAll();
                for (int i = 0; i < list.Count; i++)
                {
                    dataViewItem item = new dataViewItem();
                    item.firstName.Text = list[i].firstName;
                    item.secondName.Text = list[i].secondName;
                    item.lastName.Text = list[i].lastName;
                    item.phone.Text = list[i].phone;
                    //item.email.Text = list[i].email;
                    //item.address.Text = list[i].street;

                    item.Tag = list[i];
                    item.MouseDown += ItemMouseDown;
                    item.MouseEnter += ItemMouseEnter;
                    item.MouseLeave += ItemMouseLeave;


                    itemList.Children.Add(item);
                    
                }
            }
        }

        private void ItemMouseLeave(object sender, MouseEventArgs e)
        {
            dataViewItem item = (dataViewItem)sender;

            item.BeginAnimation(Rectangle.HeightProperty, null);
            DoubleAnimation animation = new DoubleAnimation(item.ActualHeight, 52, TimeSpan.FromSeconds(0.1f));
            item.BeginAnimation(Rectangle.HeightProperty, animation);

        }

        private void ItemMouseEnter(object sender, MouseEventArgs e)
        {
            dataViewItem item = (dataViewItem)sender;

            item.BeginAnimation(Rectangle.HeightProperty, null);
            DoubleAnimation animation = new DoubleAnimation(item.ActualHeight, 67, TimeSpan.FromSeconds(0.2f));           
            item.BeginAnimation(Rectangle.HeightProperty, animation);
            
        }


        private void ItemMouseDown(object sender, MouseButtonEventArgs e)
        {            
            dataViewItem item = (dataViewItem)sender;
            if (item != null)
            {
                foreach(dataViewItem unselectItem in itemList.Children)
                {
                    if (unselectItem.selected) unselectItem.Select(false);

                    double w = itemDataPanel.ActualWidth;
                    itemDataPanel.BeginAnimation(Rectangle.WidthProperty, null);
                    DoubleAnimation animation = new DoubleAnimation(0, w, TimeSpan.FromSeconds(1.0f));
                    itemDataPanel.BeginAnimation(Rectangle.WidthProperty, animation);

                    
                }

                item.Select(true);
                //Show data 
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (dataBaseInit)
            {
                EditWindow editWindow = new EditWindow(this, null);
                editWindow.ShowDialog();
                Refresh();
            }

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (dataBaseInit)
            {
                if (dataListView.SelectedItem == null) return;
                //  editWindow.dataBaseItem = ((dataListView.SelectedItem as dataViewItem).Tag as DataBaseItem);
                dataViewItem item = dataListView.SelectedItem as dataViewItem;
                DataBaseItem dbItem = item.Tag as DataBaseItem;
                

                EditWindow editWindow = new EditWindow(this, dbItem);
                editWindow.ShowDialog();
                Refresh();
            }
            */
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (dataBaseInit)
            {
                if (dataListView.SelectedItem == null) return;                
                dataViewItem item = dataListView.SelectedItem as dataViewItem;
                DataBaseItem dbItem = item.Tag as DataBaseItem;
                DataBase.Delete(dbItem.id);
                Refresh();
            }
            */
        }

        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            
        }


        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseClickPos = e.GetPosition(null);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePos = e.GetPosition(null);
                this.Left += mousePos.X - mouseClickPos.X;
                this.Top += mousePos.Y - mouseClickPos.Y;
            }

        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Image_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            this.Height = 354;
            this.Width = 574;
        }

    }
}
