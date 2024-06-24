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
    /// Логика взаимодействия для CardUsersWindow.xaml
    /// </summary>
    public partial class CardUsersWindow : Window
    {
        DataBase.User user;
        public CardUsersWindow(DataBase.User user)
        {
            this.user = user;
            InitializeComponent();

            var role = Helper.db.Roles.FirstOrDefault(q=>q.Id == user.IdRole);
            var post = Helper.db.Posts.FirstOrDefault(q=>q.Id == user.IdPost);
            var depart = Helper.db.Departments.FirstOrDefault(q=>q.Id == user.IdDepartment);

            fioUsersTblok.Text = $"{user.Surname} {user.SecondName} {user.MiddlName}";
            RoleUserTblok.Text = $"Роль: {role.Title}";
            PhoneUserTblok.Text = $"Контактный телефон: {user.Phone}";
            EmailTblok.Text = $"Электронная почта: {user.Email}";
            DepartmentTblok.Text = $"Отдел: {depart.Title}";
            Status_Tblok.Text = $"Статус: {user.Status}";
            PostTblok.Text = $"Должность: {post.Title}";
            LoginTblok.Text = $"Логин: {user.Logins}";



        }

        private void DelitBtn_Click(object sender, RoutedEventArgs e)
        {
            if(user == Helper.usersession)
            {
                MessageBox.Show("ВЫ не можете уволить себя");
            }
            else
            {
                user.Status = "Уволен";
                Helper.db.SaveChanges();
                Status_Tblok.Text = $"Статус: {user.Status}";
            }
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void newPussBTN_Click(object sender, RoutedEventArgs e)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            int a = 8;
            while (0 < a--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            MessageBox.Show(res.ToString());

            user.Passwords = res.ToString();
            Helper.db.SaveChanges();

        }

        private void laborcostsBtn_Click(object sender, RoutedEventArgs e)
        {
            new LaborCostsUserinfoWindow(user).Show();
        }
    }
}
