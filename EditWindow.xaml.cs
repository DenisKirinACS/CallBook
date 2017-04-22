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
using System.Windows.Shapes;

namespace CallBook
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        

        EditControl firstname;
        EditControl phone;
        EditControl email;

        public DataBaseItem dataBaseItem = null;
        //Fname, Lname, Phone, Addr, Email
        public EditWindow(Window Owner, DataBaseItem dataBaseItem)
        {
            this.Owner = Owner;
            InitializeComponent();

            firstname = AddControl("Имя:");            
            phone = AddControl("Телефон:");            
            email = AddControl("Email:");

            if (dataBaseItem != null)
            {
                this.dataBaseItem = dataBaseItem;
                firstname.textEdit.Text = dataBaseItem.firstName;
                phone.textEdit.Text = dataBaseItem.phone;
                email.textEdit.Text = dataBaseItem.email;
            }

    }

    private EditControl AddControl(string text)
        {
            EditControl editControl = new EditControl();
            editControl.textLabel.Text = text;
            editPanel.Children.Add(editControl);
            return editControl;

        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            if (dataBaseItem == null)
            {
                DataBaseItem item = new DataBaseItem();
                item.firstName = firstname.textEdit.Text;
                item.phone = phone.textEdit.Text;
                item.email = email.textEdit.Text;

                if (DataBase.Add(item)) Close();
                else
                    MessageBox.Show("Wrong data format");
            }
            else
            {
                dataBaseItem.firstName = firstname.textEdit.Text;
                dataBaseItem.phone = phone.textEdit.Text;
                dataBaseItem.email = email.textEdit.Text;

                if (DataBase.Edit(dataBaseItem)) Close();
                else
                    MessageBox.Show("Wrong data format");
            }




        }

        private void Cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                OKbutton_Click(null, null);
            }
            else
            if (e.Key == Key.Escape)            
            {
                e.Handled = true;
                Cancelbutton_Click(null, null);
            }
        }

    }
}
