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

namespace AutomaticLaborCostSystem.DataBase
{
    /// <summary>
    /// Логика взаимодействия для AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        public AddTaskWindow()
        {
            InitializeComponent();
            DepartmentCBox.ItemsSource = Helper.db.Departments.ToList();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TitleTaskTBox.Text) && !string.IsNullOrWhiteSpace(DescriptionsTBox.Text) && !string.IsNullOrWhiteSpace(DepartmentCBox.Text) )
            {
                
                    DataBase.Task task = new DataBase.Task()
                    {
                        IdProject = Helper.project.Id,
                        Title = TitleTaskTBox.Text,
                        IdStatusTasks = 1,
                        Descriptions = DescriptionsTBox.Text,
                        IdDepartment = int.Parse(DepartmentCBox.SelectedValue.ToString()),
                    };

                    Helper.db.Add(task);
                    Helper.db.SaveChanges();

                    this.Close();
                
            }
            else
                MessageBox.Show("Заполните поле", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
