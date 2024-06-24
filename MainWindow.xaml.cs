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

namespace AutomaticLaborCostSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, введены ли логин и пароль
            if (string.IsNullOrWhiteSpace(LoginTBox.Text) || string.IsNullOrWhiteSpace(PasswordTBox.Password))
            {
                // Отображаем ошибку с сообщением
                ShowError("Пожалуйста, введите логин и пароль.");
                return; // Прерываем дальнейшую обработку
            }

            // Пробуем аутентифицировать пользователя
            DataBase.User staff = Auth(LoginTBox.Text, PasswordTBox.Password);
            if (staff != null)
            {
                // Сохраняем информацию о пользователе
                Helper.usersession = staff;
                // Проверяем роль пользователя
                if (staff.IdRole == 1 || staff.IdRole == 2) // Директор и администратор
                {
                    new AdminWindow().Show();
                    this.Close();
                }
                else if (staff.IdRole == 3)
                {
                    new HeadDepartmentWindow().Show();
                    this.Close();
                }
            }
            else
            {
                // Отображаем ошибку с сообщением
                ShowError("Неверный логин или пароль.");
            }
        }

        public DataBase.User Auth(string login, string password)
        {
            return Helper.db.Users.FirstOrDefault(x => x.Logins == login && x.Passwords == password);
        }

        // Метод для красивого отображения ошибки
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}