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
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        string log;
        public AddUserWindow()
        {
            InitializeComponent();
            RolrCBox.ItemsSource = Helper.db.Roles.ToList();
            PostCBox.ItemsSource = Helper.db.Posts.ToList();
            DepartCBox.ItemsSource = Helper.db.Departments.ToList();

            while (true)
            {
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                Random rnd = new Random();
                int a = 8;
                while (0 < a--)
                {
                    res.Append(valid[rnd.Next(valid.Length)]);
                }
                var q = Helper.db.Users.FirstOrDefault(q => q.Logins == res.ToString());
                if (q == null)
                {
                    LoginTblock.Text = "Логин: "+res.ToString();
                    PussTblock.Text = "Пароль: " + res.ToString();
                    log = res.ToString();
                    break;
                }
            }
            

        }

        private void AddSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FnameTBox.Text) &&
                !string.IsNullOrWhiteSpace(SnameTBox.Text) &&
                !string.IsNullOrWhiteSpace(MnameTBox.Text) &&
                !string.IsNullOrWhiteSpace(RolrCBox.Text) &&
                !string.IsNullOrWhiteSpace(PostCBox.Text) &&
                !string.IsNullOrWhiteSpace(DepartCBox.Text) &&
                !string.IsNullOrWhiteSpace(PhonTBox.Text) &&
                !string.IsNullOrWhiteSpace(EmailTBox.Text))
            {
                if (Regex.IsMatch(FnameTBox.Text, @"[а-я]$") &&
                Regex.IsMatch(SnameTBox.Text, @"[а-я]$") &&
                Regex.IsMatch(MnameTBox.Text, @"[а-я]$") &&
                Regex.IsMatch(PhonTBox.Text, @"[0-9]$") && PhonTBox.Text.Length == 11
                )
                {

                    DataBase.User user = new DataBase.User()
                    {
                        Surname = FnameTBox.Text,
                        SecondName = SnameTBox.Text,
                        MiddlName = MnameTBox.Text,
                        IdRole = int.Parse(RolrCBox.SelectedValue.ToString()),
                        Phone = PhonTBox.Text,
                        Email = EmailTBox.Text,
                        IdDepartment = int.Parse(DepartCBox.SelectedValue.ToString()),
                        Logins = log,
                        Passwords = log,
                        Status = "Работает",
                        IdPost = int.Parse(PostCBox.SelectedValue.ToString()),


                    };
                    Helper.db.Users.Add(user);
                    Helper.db.SaveChanges();
                    MessageBox.Show("Регистрация успешна", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();


                }
                else
                {
                    MessageBox.Show("Введены недопустимые символы", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
