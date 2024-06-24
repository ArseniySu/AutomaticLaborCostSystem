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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AutomaticLaborCostSystem
{
    /// <summary>
    /// Логика взаимодействия для AddProductionCalendarWindow.xaml
    /// </summary>
    public partial class AddProductionCalendarWindow : Window
    {
        public AddProductionCalendarWindow()
        {
            InitializeComponent();
            MonthsCBox.ItemsSource = new Tablenum[] {
                new Tablenum{ title = "Январь"},
                new Tablenum{ title = "Февраль"},
                new Tablenum{ title = "Март"},
                new Tablenum{ title = "Апрель"},
                new Tablenum{ title = "Май"},
                new Tablenum{ title = "Июнь"},
                new Tablenum{ title = "Июль"},
                new Tablenum{ title = "Август"},
                new Tablenum{ title = "Сентябрь"},
                new Tablenum{ title = "Октябрь"},
                new Tablenum{ title = "Ноябрь"},
                new Tablenum{ title = "Декабрь"}

            };

            var Months = DateTime.Now.ToString("MM");
            MonthsCBox.SelectedIndex = int.Parse(Months);
            
        }

        public class Tablenum
        {
            public string title { get; set; }
            public override string ToString() => $"{title}";
        }

        private void newCalendBtn_Click(object sender, RoutedEventArgs e)
        {
            var year = DateTime.Now.ToString("yyyy");
            var a = MonthsCBox.SelectedIndex + 1;

            int countdya = 30;
            if (a == 1 || a == 3 || a == 5 || a == 7 || a == 8 || a == 10 || a == 12)
                countdya = 31;
            else if (a == 2 && int.Parse(year) % 4 == 0)
            {
                countdya = 29;
            }
            else if (a == 2) { countdya = 28; }

            for (int i = 1; i <= countdya; i++)
            {
                DateTime date1 = new DateTime(int.Parse(year), a, i);

                DataBase.ProductionCalendar productionCalendar = new DataBase.ProductionCalendar()
                {                  
                    Date_ = date1,
                };
                if (date1.ToString("dddd") == "суббота" || date1.ToString("dddd") == "воскресенье")
                    productionCalendar.NormHours = "Выходной день";
                else 
                    productionCalendar.NormHours = "8 ч.";
                Helper.db.Add(productionCalendar);

            }
            Helper.db.SaveChanges();
            MessageBox.Show($"Календарь заполнен");
            this.Close();
        }
    }
}
