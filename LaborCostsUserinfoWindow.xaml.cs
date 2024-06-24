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
    public partial class LaborCostsUserinfoWindow : Window
    {
        private DateTime currentDate; // Текущая дата
        private Grid calendarGridContainer; // Ссылка на Grid элемент
        private DataBase.User selectedUser;

        public LaborCostsUserinfoWindow(DataBase.User user)
        {
            InitializeComponent();
            selectedUser = user; // Сохраняем пользователя

            // Устанавливаем текущий месяц и год
            currentDate = DateTime.Today;

            // Находим CalendarGridContainer в XAML
            calendarGridContainer = this.FindName("CalendarGridContainer") as Grid;
            if (calendarGridContainer != null)
            {
                // Заполняем комбобоксы
                InitializeComboBoxes();

                // Устанавливаем выбранный месяц и год в ComboBox после заполнения
                MonthsqweCBox.SelectedIndex = currentDate.Month - 1;
                YearsCBox.SelectedIndex = YearsCBox.Items.IndexOf(currentDate.Year.ToString());

                // Генерируем календарь
                GenerateCalendar();
            }
            else
            {
                MessageBox.Show("Ошибка: Не удалось найти элемент CalendarGrid.");
            }
        }

        private void GenerateCalendar()
        {
            // Очищаем предыдущий календарь
            calendarGridContainer.Children.Clear();

            // Получаем день недели для 1-го числа месяца
            int firstDay = (int)currentDate.DayOfWeek;

            // Получаем количество дней в месяце
            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            // Вычисляем позицию кнопки в Grid (колонку и ряд)
            // Начинаем с 1-го числа месяца, но вычисляем его позицию на основе первого дня недели
            int dayOfWeekIndex = ((int)currentDate.DayOfWeek - 1 + 7) % 7;  // Индекс дня недели (0 - Пн, 6 - Вс)
            int currentDay = 1;

            // Цикл для генерации кнопок с учетом первого дня недели
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (i == 0 && j < dayOfWeekIndex)
                    {
                        // Места для дней, которые предшествуют первому числу месяца
                        // (пустые кнопки, если они есть)
                    }
                    else if (currentDay <= daysInMonth)
                    {
                        DateTime selectedDate = new DateTime(currentDate.Year, currentDate.Month, currentDay); // Создаем дату из выбранного дня

                        // Получаем запись о работе пользователя за этот день
                        var workHour = Helper.db.WorkingHours.FirstOrDefault(wh =>
                            wh.IdUsers == selectedUser.Id && wh.DatetimeStart.Date == selectedDate.Date);

                        // Создаем кнопку для текущего дня
                        Button button = new Button
                        {
                            Content = $"{currentDay}",
                            FontSize = 14,
                            Margin = new Thickness(5),
                            // Устанавливаем высоту кнопок
                            Height = 110
                        };

                        // Устанавливаем позицию кнопки в Grid
                        Grid.SetRow(button, i + 1); // Начинаем с Row 1
                        Grid.SetColumn(button, j);
                        calendarGridContainer.Children.Add(button);

                        // Добавьте обработчик события Click для каждой кнопки
                        button.Click += DayButton_Click;

                        // Устанавливаем стиль для кнопок
                        if (selectedDate.Day == DateTime.Today.Day && selectedDate.Month == DateTime.Today.Month &&
                            selectedDate.Year == DateTime.Today.Year) // Текущий день
                        {
                            button.Background = Brushes.LightBlue;
                            button.Foreground = Brushes.Black;

                            if (workHour != null)
                            {
                                button.Content = $"{currentDay}\n\nвремя нач.{workHour.DatetimeStart:HH:mm:ss}";
                            }
                        }
                        else if (selectedDate.DayOfWeek == DayOfWeek.Saturday ||
                                 selectedDate.DayOfWeek == DayOfWeek.Sunday) // Выходные
                        {
                            button.Foreground = Brushes.Purple;
                            button.Background = Brushes.LightYellow;
                            if (workHour != null)
                            {
                                button.Content = $"{currentDay}\n\nвремя нач.{workHour.DatetimeStart:HH:mm:ss}\nвремя окнч.{workHour.DatetimeFinish:HH:mm:ss}\nфакт.{workHour.ActualTime}";
                            }
                        }
                        else
                        {
                            button.Background = Brushes.Transparent;
                            button.Foreground = Brushes.Black;

                            if (workHour != null)
                            {
                                button.Content = $"{currentDay}\n\nвремя нач.{workHour.DatetimeStart:HH:mm:ss}\nвремя окнч.{workHour.DatetimeFinish:HH:mm:ss}\nфакт.{workHour.ActualTime}";
                            }
                        }

                        currentDay++;
                    }
                    else
                    {
                        // Места для дней, которые следуют за последним днем месяца
                        // (пустые кнопки, если они есть)
                    }
                }
            }

            // Обновляем заголовок с названием месяца
            MonthName.Text = currentDate.ToString("MMMM yyyy");
        }

        private void InitializeComboBoxes()
        {
            MonthsqweCBox.ItemsSource = new List<MonthYear>
            {
                new MonthYear { title = "Январь"},
                new MonthYear { title = "Февраль" },
                new MonthYear { title = "Март" },
                new MonthYear { title = "Апрель" },
                new MonthYear { title = "Май" },
                new MonthYear { title = "Июнь" },
                new MonthYear { title = "Июль" },
                new MonthYear { title = "Август" },
                new MonthYear { title = "Сентябрь" },
                new MonthYear { title = "Октябрь" },
                new MonthYear { title = "Ноябрь" },
                new MonthYear { title = "Декабрь" }
            };

            YearsCBox.ItemsSource = Enumerable.Range(DateTime.Now.Year, 100).Select(year => new MonthYear { title = year.ToString() }).ToList();
        }

        private void MonthsCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthsqweCBox.SelectedItem != null && YearsCBox.SelectedItem != null)
            {
                MonthYear selectedMonth = MonthsqweCBox.SelectedItem as MonthYear;
                int year = int.Parse(YearsCBox.SelectedItem.ToString());

                currentDate = new DateTime(year, selectedMonth.title == "Январь" ? 1 : selectedMonth.title == "Февраль" ? 2 : selectedMonth.title == "Март" ? 3 : selectedMonth.title == "Апрель" ? 4 : selectedMonth.title == "Май" ? 5 : selectedMonth.title == "Июнь" ? 6 : selectedMonth.title == "Июль" ? 7 : selectedMonth.title == "Август" ? 8 : selectedMonth.title == "Сентябрь" ? 9 : selectedMonth.title == "Октябрь" ? 10 : selectedMonth.title == "Ноябрь" ? 11 : 12, 1);
                GenerateCalendar();
            }
        }

        private void YearsCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthsqweCBox.SelectedItem != null && YearsCBox.SelectedItem != null)
            {
                MonthYear selectedMonth = MonthsqweCBox.SelectedItem as MonthYear;
                int year = int.Parse(YearsCBox.SelectedItem.ToString());

                currentDate = new DateTime(year, selectedMonth.title == "Январь" ? 1 : selectedMonth.title == "Февраль" ? 2 : selectedMonth.title == "Март" ? 3 : selectedMonth.title == "Апрель" ? 4 : selectedMonth.title == "Май" ? 5 : selectedMonth.title == "Июнь" ? 6 : selectedMonth.title == "Июль" ? 7 : selectedMonth.title == "Август" ? 8 : selectedMonth.title == "Сентябрь" ? 9 : selectedMonth.title == "Октябрь" ? 10 : selectedMonth.title == "Ноябрь" ? 11 : 12, 1);
                GenerateCalendar();

                YearsCBox.SelectedIndex = YearsCBox.Items.IndexOf(YearsCBox.SelectedItem);
            }
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            GenerateCalendar();
            MonthsqweCBox.SelectedIndex = currentDate.Month - 1;
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            GenerateCalendar();
            MonthsqweCBox.SelectedIndex = currentDate.Month - 1;
        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем текст кнопки (номер дня)
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                int day = int.Parse(clickedButton.Content.ToString().Split('\n')[0]); // Извлекаем число из текста

                // Создаем дату из выбранного дня, месяца и года
                DateTime selectedDate = new DateTime(currentDate.Year, currentDate.Month, day);

                InfoLaborcostsWindow infoWindow = new InfoLaborcostsWindow(selectedDate);
                infoWindow.Show();
            }
        }


    }


    public class MonthYearq
    {
        public string title { get; set; }
        public override string ToString() => $"{title}";
    }
}
    