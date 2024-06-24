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
    /// Логика взаимодействия для CardProjectWindow.xaml
    /// </summary>
    public partial class CardProjectWindow : Window
    {
        DataBase.Project Project;
        public CardProjectWindow(DataBase.Project Project)
        {
            InitializeComponent();
            this.Project = Project;
            TittleProjTBlock.Text = $"Наименование проекта: {Project.Title}";
            DateStartTBlock.Text = $"Дата старта проекта: {Project.DatetimeStart}";
            DatePlanTBlock.Text = $"Планируемая дата окончания проекта: {Project.PlannedEndDate}";

            TaskDgrid.ItemsSource = Helper.db.Tasks.Where(q=>q.IdProject == Project.Id).Include(q=>q.IdStatusTasksNavigation).ToList();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Project.DatetimeFinish == null)
            {
                Project.DatetimeFinish = DateTime.Now;
                Helper.db.SaveChanges();
                this.Close();
            }
            else 
            {
                MessageBox.Show("Проект уже завершен!");
                this.Close();
            }
        }
    }
}
