using AutomaticLaborCostSystem.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static AutomaticLaborCostSystem.AddProductionCalendarWindow;

namespace AutomaticLaborCostSystem
{
    /// <summary>
    /// Логика взаимодействия для HeadDepartmentWindow.xaml
    /// </summary>
    public partial class HeadDepartmentWindow : Window
    {
        private Grid calendarGridContainer; // Ссылка на Grid элемент
        private DateTime currentDate; // Текущая дата

        DispatcherTimer _timer;
        TimeSpan _time;

        private DateTime datetime = DateTime.Now;
        private const int idleTimeThreshold = 10 * 60; // 10 минут в секундах

        // Словарь для хранения времени активности окон
        Dictionary<string, TimeSpan> windowActivity = new Dictionary<string, TimeSpan>();
        DateTime lastActiveTime = DateTime.Now;

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        static int GetLastInputTime()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (UInt32)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                int lastInputTick = (Int32)lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : 0);


        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        private void stop1_Click()
        {
            // Запускаем цикл для отслеживания активности
            while (true)
            {
                // Проверяем время простоя
                if (GetLastInputTime() >= idleTimeThreshold)
                {
                    // Останавливаем таймер
                    _timer.Stop();
                }
                else
                {
                    // Запускаем таймер
                    _timer.Start();
                }

                // Ожидание 1 секунды
                Thread.Sleep(1000);
            }
        }

        private void stop_ClickQQ()
        {

            // Запускаем цикл для отслеживания активности окон
            while (true)
            {
                // Получение окна, которое находится в фокусе
                IntPtr foregroundWindow = GetForegroundWindow();

                // Проверяем, изменилось ли активное окно
                if (foregroundWindow != IntPtr.Zero)
                {
                    // Получаем заголовок активного окна
                    string windowTitle = GetWindowText(foregroundWindow);

                    // Проверяем, не пустой ли заголовок
                    if (!string.IsNullOrEmpty(windowTitle))
                    {
                        // Обновляем время активности для текущего окна
                        if (windowActivity.ContainsKey(windowTitle))
                        {
                            windowActivity[windowTitle] += DateTime.Now - lastActiveTime;
                        }
                        else
                        {
                            windowActivity.Add(windowTitle, DateTime.Now - lastActiveTime);
                        }

                        lastActiveTime = DateTime.Now;
                    }
                }

                // Ожидание 1 секунды
                Thread.Sleep(1000);
            }
        }

        // Функция для получения текста окна
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        static string GetWindowText(IntPtr hWnd)
        {
            const int nChars = 256;
            StringBuilder sb = new StringBuilder(nChars);
            GetWindowText(hWnd, sb, nChars);
            return sb.ToString();
        }



        public HeadDepartmentWindow()
        {
            InitializeComponent();
            var userinfo = Helper.usersession;
            UsersInfo.Text = $"{userinfo.Surname} {userinfo.SecondName} {userinfo.MiddlName}";


            // Получаем сегодняшнюю дату (с 00:00:00)
            DateTime today = DateTime.Today;

            // Получаем запись, где дата начала работы совпадает с сегодняшней датой
            var worktoday = Helper.db.WorkingHours.FirstOrDefault(q =>
                q.DatetimeStart.Date == today.Date && q.IdUsers == userinfo.Id
             );

            // Проверяем, была ли найдена запись
            if (worktoday == null)
            {
                // Работаем с найденной записью
                DataBase.WorkingHour workingHour = new DataBase.WorkingHour()
                {
                    IdUsers = userinfo.Id,
                    DatetimeStart = datetime,
                };
                Helper.db.Add(workingHour);
                Helper.db.SaveChanges();
            }

            _time = TimeSpan.FromSeconds(1);
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                Timerwork.Text = $"Отработанно за сегодня: {_time.ToString("c")}";
                _time = _time.Add(TimeSpan.FromSeconds(1));
            }, Application.Current.Dispatcher);


            Thread tm = new Thread(stop1_Click);
            tm.Start();


            Thread tm1 = new Thread(stop_ClickQQ);
            tm1.Start();

            calendarGridContainer = this.FindName("CalendarGridContainer") as Grid;
            if (calendarGridContainer != null)
            {
                // Заполняем комбобоксы
                InitializeComboBoxes();

                // Устанавливаем текущий месяц и год
                currentDate = DateTime.Today;

                // Устанавливаем выбранный месяц и год в ComboBox после заполнения
                MonthsqweCBox.SelectedIndex = currentDate.Month - 1;
                YearsCBox.SelectedIndex = YearsCBox.Items.IndexOf(currentDate.Year.ToString());

                GenerateCalendar();
            }
            else
            {
                MessageBox.Show("Ошибка: Не удалось найти элемент CalendarGrid.");
            }



            InitData();
        }

        private void InitData() // Функция заполнения датаГридов
        {
            //заполнение проетов
            ProjektDGrid.ItemsSource = Helper.db.Projects.ToList();
            //заполнение производственного каллендаря
            // 1. Получаем текущий месяц
            int currentMonth = DateTime.Now.Month;
            // 2. Фильтруем данные по текущему месяцу.
            WorkCalendDGrid.ItemsSource = Helper.db.ProductionCalendars
                                                .Where(q => q.Date_.Month == currentMonth)
                                                .ToList();



            var Months = DateTime.Now.ToString("MM.yyyy");
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e) // кнопка "назад"
        {
            new MainWindow().Show();
            this.Close();
            var userinfo = Helper.usersession;
            var workingHour = Helper.db.WorkingHours.FirstOrDefault(q =>
                    q.IdUsers == userinfo.Id && q.DatetimeStart.Date == datetime.Date);
            if (workingHour != null)
            {
                workingHour.DatetimeFinish = DateTime.Now;
                // Проверка на null и инициализация, если нужно
                if (workingHour.ActualTime == null)
                {
                    workingHour.ActualTime = TimeSpan.Zero; // Инициализируем нулевым значением
                }
                // Прибавление времени
                workingHour.ActualTime += _time;

                foreach (var window in windowActivity)
                {
                    DataBase.ActiveWindow activeWindow = new DataBase.ActiveWindow()
                    {
                        IdWorkingHours = workingHour.Id,
                        Title = window.Key,
                        ActualTime = window.Value
                    };
                    Helper.db.Add(activeWindow);
                }
            }
            else
            {
                MessageBox.Show("");
            }
            Helper.db.SaveChanges();

        }
        private void NewProjektBtn_Click(object sender, RoutedEventArgs e) // Кнопка создания проекта
        {
            new NewProjectWindow().Show();
        }
        private void update_ClickHis(object sender, RoutedEventArgs e) // Кнопка обновления датаГридов
        {
            InitData();
        }
        private void ProjektDGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) //Обработка двойного нажатия кнопки мыши, переход в карточку прокта
        {
            if ((ProjektDGrid.SelectedItem as DataBase.Project) == null)
            {
                MessageBox.Show("пусто");
            }
            else
            {
                new CardProjectWindow(ProjektDGrid.SelectedItem as DataBase.Project).Show();

                InitData();
            }
        }

        private void updatePassworpBtn_Click(object sender, RoutedEventArgs e) //кнопка изменения пароля
        {
            new updatePassworpWindow().Show();
        }

        private void updateLoginBtn_Click(object sender, RoutedEventArgs e) // кнопка изменения логина
        {
            new updateLoginWindow().Show();
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

                        // Форматируем дату для сравнения с данными из БД
                        string formattedDate = selectedDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

                        // Получаем выбранный месяц из ComboBox
                        MonthYear selectedMonth = MonthsqweCBox.SelectedItem as MonthYear;
                        var times = Helper.db.WorkingHours.FirstOrDefault(q => q.IdUsers == Helper.usersession.Id && q.DatetimeStart.Date == selectedDate.Date);

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

                            if (times != null)
                            {
                                button.Content = $"{currentDay}\n\nвремя нач.{times.DatetimeStart:HH:mm:ss}";
                            }
                        }
                        else if (selectedDate.DayOfWeek == DayOfWeek.Saturday ||
                                 selectedDate.DayOfWeek == DayOfWeek.Sunday) // Выходные
                        {
                            button.Foreground = Brushes.Purple;
                            button.Background = Brushes.LightYellow;
                            if (times != null)
                            {
                                button.Content = $"{currentDay}\n\nвремя нач.{times.DatetimeStart:HH:mm:ss}\nвремя окнч.{times.DatetimeFinish:HH:mm:ss}\nфакт.{times.ActualTime}";
                            }
                        }
                        else
                        {

                            button.Background = Brushes.Transparent;
                            button.Foreground = Brushes.Black;

                            // Добавляем время и отработанное время в текст кнопки, если оно есть
                            if (times != null)
                            {
                                button.Content = $"{currentDay}\n\nвремя нач.{times.DatetimeStart:HH:mm:ss}\nвремя окнч.{times.DatetimeFinish:HH:mm:ss}\nфакт.{times.ActualTime}";
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

            // Заполняем YearsCBox (например, с 2024 по 2100 год)
            YearsCBox.ItemsSource = Enumerable.Range(DateTime.Now.Year, 100).Select(year => new MonthYear { title = year.ToString() }).ToList();

            MonthsCBox.ItemsSource = new List<MonthYear>
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

        }

        private void MonthsCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthsqweCBox.SelectedItem != null && YearsCBox.SelectedItem != null)
            {
                MonthYear selectedMonth = MonthsqweCBox.SelectedItem as MonthYear;
                int year = int.Parse(YearsCBox.SelectedItem.ToString());

                // Обновление currentDate, учитывая выбранный месяц и год
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

                // Обновление currentDate, учитывая выбранный месяц и год
                currentDate = new DateTime(year, selectedMonth.title == "Январь" ? 1 : selectedMonth.title == "Февраль" ? 2 : selectedMonth.title == "Март" ? 3 : selectedMonth.title == "Апрель" ? 4 : selectedMonth.title == "Май" ? 5 : selectedMonth.title == "Июнь" ? 6 : selectedMonth.title == "Июль" ? 7 : selectedMonth.title == "Август" ? 8 : selectedMonth.title == "Сентябрь" ? 9 : selectedMonth.title == "Октябрь" ? 10 : selectedMonth.title == "Ноябрь" ? 11 : 12, 1);
                GenerateCalendar();

                // Обновляем YearsCBox, чтобы он показывал выбранный год
                YearsCBox.SelectedIndex = YearsCBox.Items.IndexOf(YearsCBox.SelectedItem);
            }
        }

        // Обработчик события для перехода к следующему месяцу
        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            GenerateCalendar();
            // Обновляем MonthsCBox
            MonthsqweCBox.SelectedIndex = currentDate.Month - 1;
        }

        // Обработчик события для перехода к предыдущему месяцу
        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            GenerateCalendar();
            // Обновляем MonthsCBox
            MonthsqweCBox.SelectedIndex = currentDate.Month - 1;
        }

        // Обработчик события нажатия на кнопку дня
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

        private void SearchCalendarBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Получаем выбранный месяц из ComboBox
            MonthYear selectedMonth = MonthsCBox.SelectedItem as MonthYear;
            if (selectedMonth == null)
            {
                MessageBox.Show("Выберите месяц для поиска.");
                return;
            }

            // 2. Фильтруем данные по выбранному месяцу.
            WorkCalendDGrid.ItemsSource = Helper.db.ProductionCalendars
                                        .Where(q =>
                                            q.Date_.Month == (selectedMonth.title == "Январь" ? 1 :
                                                             selectedMonth.title == "Февраль" ? 2 :
                                                             selectedMonth.title == "Март" ? 3 :
                                                             selectedMonth.title == "Апрель" ? 4 :
                                                             selectedMonth.title == "Май" ? 5 :
                                                             selectedMonth.title == "Июнь" ? 6 :
                                                             selectedMonth.title == "Июль" ? 7 :
                                                             selectedMonth.title == "Август" ? 8 :
                                                             selectedMonth.title == "Сентябрь" ? 9 :
                                                             selectedMonth.title == "Октябрь" ? 10 :
                                                             selectedMonth.title == "Ноябрь" ? 11 :
                                                             selectedMonth.title == "Декабрь" ? 12 : 0)
                                        )
                                        .ToList();
        }

        private void ResetCalendarBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Получаем текущий месяц
            int currentMonth = DateTime.Now.Month;

            // 2. Фильтруем данные по текущему месяцу.
            WorkCalendDGrid.ItemsSource = Helper.db.ProductionCalendars
                                                .Where(q => q.Date_.Month == currentMonth)
                                                .ToList();
        }

        private void ShowAllCalendarBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Сбрасываем фильтры и показываем все записи.
            WorkCalendDGrid.ItemsSource = Helper.db.ProductionCalendars.ToList();
        }
    }


    public class MonthYearHear
    {
        public string title { get; set; }
        public override string ToString() => $"{title}";
    }
}
    

