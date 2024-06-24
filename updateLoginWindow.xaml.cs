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

namespace AutomaticLaborCostSystem
{
    /// <summary>
    /// Логика взаимодействия для updateLoginWindow.xaml
    /// </summary>
    public partial class updateLoginWindow : Window
    {
        public updateLoginWindow()
        {
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewLogTBox.Text))
            {
                Helper.usersession.Logins = NewLogTBox.Text;
                Helper.db.SaveChanges();
                MessageBox.Show("Логин изменен");
                this.Close();
            }
            else
            {
                MessageBox.Show("Заполните все поля", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
