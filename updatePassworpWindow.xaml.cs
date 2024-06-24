using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutomaticLaborCostSystem
{
    public partial class updatePassworpWindow : Window
    {
        public updatePassworpWindow()
        {
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(OldPassTBox.Text) &&
                !string.IsNullOrWhiteSpace(NewPassTBox.Text) &&
                !string.IsNullOrWhiteSpace(NewDPassTBox.Text))
            {

                if (Helper.usersession.Passwords == OldPassTBox.Text)
                {
                    OldPassTBox.ToolTip = "";
                    OldPassTBox.Foreground = Brushes.Black;
                    if (NewPassTBox.Text == NewDPassTBox.Text)
                    {
                        NewDPassTBox.ToolTip = "";
                        NewDPassTBox.Foreground = Brushes.Black;
                        Helper.usersession.Passwords = NewPassTBox.Text;
                        Helper.db.SaveChanges();
                        MessageBox.Show("Пароль изменен");
                        this.Close();

                    }
                    else
                    {
                        NewDPassTBox.ToolTip = "Введен неверный пароль";
                        NewDPassTBox.Foreground = Brushes.Red;
                    }
                }
                else
                {
                    OldPassTBox.ToolTip = "Пароль не верный";
                    OldPassTBox.Foreground = Brushes.Red;
                }
            }
            else
            {
                
                MessageBox.Show("Заполните все поля", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
