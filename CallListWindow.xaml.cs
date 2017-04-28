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
using CallBook.Methods;
using System.Windows.Threading;
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
            dataBaseInit = DataBase.Init();

            if (!dataBaseInit)
            {
                MessageBox.Show("DataBase init wrong");
            }
            else
            {
                ItemsPreloader.SetUIRefresh(UIRefresh);
                //ItemsPreloader.LaunchFilter(DataBase.GetAll());
                ItemsPreloader.RefreshAll();
            }

        /*    Task.Run(delegate
            {
                while (true)
                {
                    SimpleTask();
                }
            });*/ //Лишь показыват что приложение не глючит! Приятного дня!
        }

        /*public void SimpleTask()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                firstName.Text = "";
            }));

            for (int i = 0; i < 55; i++)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    firstName.Text += "|";
                }));
                System.Threading.Thread.Sleep(50);
            }

        }*/

        public bool UIRefresh(List<DataBaseItem> dataBaseItem, bool clear)
        {
            if (dataBaseItem == null) return false;
            //Иногда проскакивает список с пустым элементо
            //Запрещаем такое вообще расматривать
            if (dataBaseItem.Count == 0) return false;

            if (clear)
            {
                //Добавляем в очередь для обработки в UI
                this.Dispatcher.Invoke(() => itemList.Children.Clear());
            }

            if (dataBaseInit)
            {
                //Добавляем в очередь для обработки в UI
                Dispatcher.Invoke(new Action(() =>
                {
                    //Небольшая оптимизация
                    int count = dataBaseItem.Count;
                    //Console.WriteLine("ItemsCount:" + count);
                    for (int i = 0; i < count; i++)
                    {
                        dataViewItem item = new dataViewItem();
                        item.firstName.Text = dataBaseItem[i].firstName;
                        item.secondName.Text = dataBaseItem[i].secondName;
                        item.lastName.Text = dataBaseItem[i].lastName;
                        item.phone.Text = dataBaseItem[i].phone;
                            //item.email.Text = list[i].email;
                            //item.address.Text = list[i].street;

                        item.Tag = dataBaseItem[i];
                        item.MouseDown += ItemMouseDown;
                        item.MouseEnter += ItemMouseEnter;
                        item.MouseLeave += ItemMouseLeave;

                        itemList.Children.Add(item);
                    }
                }));
            }

            //Уменьшаем подвисания
            //Поскольку этот метод запускает из другого потока нужно
            //Дать не большое время для того что бы диспечер успел
            //Выполнить предыдущие задание
            //При не обходимости подымаем это значение
            System.Threading.Thread.Sleep(1);

            return true;
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
                //Refresh();
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

        private void Search_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (itemList == null) return;
            ItemSearching.Searching(Search.Text);
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}
