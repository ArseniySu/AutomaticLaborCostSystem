using AutomaticLaborCostSystem.DataBase;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public NewProjectWindow()
        {
            InitializeComponent();

            InitData();
        }
        public void InitData()
        {
            if (Helper.project != null)
            {
                DishDgrid.ItemsSource = Helper.db.Tasks.Include(q => q.IdStatusTasksNavigation).Include(q => q.IdDepartmentNavigation).Where(q => q.IdProject == Helper.project.Id).ToList();

            }

        }
        private void update_ClickHis(object sender, RoutedEventArgs e)
        {
            InitData();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            foreach (var c in Helper.db.Tasks)
            {
                if (c.IdProject == Helper.project.Id)
                {
                    Helper.db.Tasks.Remove(c);
                }
            }

            Helper.db.Projects.Remove(Helper.project);
            Helper.db.SaveChanges();
        }

        private void AddSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Helper.project = null;
        }

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {


            if (Helper.project == null)
            {
                DataBase.Project proj  = new DataBase.Project()
                {
                    DatetimeStart = DateTime.Now,
                    PlannedEndDate = DateTime.Parse(dateStart.ToString()),
                    Title = TittleTBox.Text

                };
                Helper.db.Add(proj);
                Helper.db.SaveChanges();
                Helper.project = Helper.db.Projects.OrderByDescending(r => r.Id).First();
                new AddTaskWindow().Show();
                
            }
            else
                new AddTaskWindow().Show();
        }
    }
}
