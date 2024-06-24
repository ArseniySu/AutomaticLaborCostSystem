using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AutomaticLaborCostSystem.DataBase;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using Aspose.Cells;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace AutomaticLaborCostSystem
{
    public partial class InfoLaborcostsWindow : System.Windows.Window
    {
        private DateTime _selectedDate;
        public ObservableCollection<ActiveWindow> ActiveWindowsData { get; set; } = new();
        public ObservableCollection<WriteDown> WriteDownData { get; set; } = new();
        public ObservableCollection<MissedDay> MissedDaysData { get; set; } = new();

        public InfoLaborcostsWindow(DateTime selectedDate)
        {
            InitializeComponent();
            _selectedDate = selectedDate;
            DataContext = this;

            LoadData();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadData()
        {
            int userId = Helper.usersession.Id;

            // Активные окна
            ActiveWindowsData = new ObservableCollection<ActiveWindow>(Helper.db.ActiveWindows
                .Include(x => x.IdWorkingHoursNavigation)
                .Where(x => x.IdWorkingHoursNavigation.IdUsers == userId && x.IdWorkingHoursNavigation.DatetimeStart.Date == _selectedDate)
                .Select(x => new ActiveWindow
                {
                    Title = x.Title,
                    ActualTime = x.ActualTime.HasValue
     ? TimeSpan.ParseExact(new TimeSpan(x.ActualTime.Value.Ticks).ToString(@"hh\:mm\:ss"), @"hh\:mm\:ss", null)
     : null
                }));

            // Списания 
            WriteDownData = new ObservableCollection<WriteDown>(Helper.db.WriteDowns
                .Include(x => x.IdWorkingHoursNavigation)
                .Where(x => x.IdWorkingHoursNavigation.IdUsers == userId && x.IdWorkingHoursNavigation.DatetimeStart.Date == _selectedDate)
                .Select(x => new WriteDown
                {
                    Reason = x.Reason,
                    ActualTime = x.ActualTime.HasValue
    ? TimeSpan.ParseExact(new TimeSpan(x.ActualTime.Value.Ticks).ToString(@"hh\:mm\:ss"), @"hh\:mm\:ss", null)
    : null,
                    Comment = x.Comment
                }));

            // Отсутствия
            MissedDaysData = new ObservableCollection<MissedDay>(Helper.db.MissedDays
                .Where(x => x.IdUsers == userId && x.DatetimeStart.Date == _selectedDate)
                .Select(x => new MissedDay
                {
                    Reason = x.Reason,
                    DatetimeStart = x.DatetimeStart, //  Форматирование дат происходит в XAML
                    DatetimeFinish = x.DatetimeFinish, //  Форматирование дат происходит в XAML
                    Comment = x.Comment
                }));
        }

        private void AddDeduction_Click(object sender, RoutedEventArgs e)
        {
            // Получаем IdWorkingHours для выбранной даты и пользователя
            int userId = Helper.usersession.Id;
            var workingHours = Helper.db.WorkingHours
                .FirstOrDefault(wh => wh.IdUsers == userId && wh.DatetimeStart.Date == _selectedDate);

            if (workingHours == null)
            {
                MessageBox.Show("Ошибка: Не найдены данные о рабочем времени для этой даты.");
                return;
            }

            // Создаем новый объект WriteDown
            var newWriteDown = new WriteDown
            {
                Reason = ReasonTextBox.Text,
                Comment = CommentTextBox.Text,
                IdWorkingHours = workingHours.Id
            };

            // Проверяем и парсим время из отдельных полей
            if (int.TryParse(HoursTextBox.Text, out int hours) &&
                int.TryParse(MinutesTextBox.Text, out int minutes) &&
                int.TryParse(SecondsTextBox.Text, out int seconds) &&
                hours >= 0 && hours <= 23 &&
                minutes >= 0 && minutes <= 59 &&
                seconds >= 0 && seconds <= 59)
            {
                newWriteDown.ActualTime = new TimeSpan(hours, minutes, seconds);
            }
            else
            {
                MessageBox.Show("не заполнены обязательный поля или не верно заполныны поля времени\nпроверьте правильность заполнения всех полей ");
                return;
            }

            // Добавляем запись в базу данных
            Helper.db.WriteDowns.Add(newWriteDown);
            Helper.db.SaveChanges();

            // Обновляем DataGrid
            WriteDownData.Add(newWriteDown);

            // Очищаем поля ввода
            ReasonTextBox.Clear();
            HoursTextBox.Clear();
            MinutesTextBox.Clear();
            SecondsTextBox.Clear();
            CommentTextBox.Clear();
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            // Создаем объект Excel
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = true;

            // Создаем новую книгу и лист
            Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Missing.Value);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = excelApp.ActiveSheet;

            // Заголовки таблиц
            worksheet.Cells[1, 1] = "Активные окна";
            worksheet.Cells[1, 4] = "Списания";
            worksheet.Cells[1, 8] = "Отсутствия";

            // Заголовки столбцов
            worksheet.Cells[2, 1] = "Наименование";
            worksheet.Cells[2, 2] = "Время";
            worksheet.Cells[2, 4] = "Наименование";
            worksheet.Cells[2, 5] = "Время";
            worksheet.Cells[2, 6] = "Комментарий";
            worksheet.Cells[2, 8] = "Наименование";
            worksheet.Cells[2, 9] = "Начало";
            worksheet.Cells[2, 10] = "Окончание";
            worksheet.Cells[2, 11] = "Комментарий";

            // Заполняем лист данными (начинаем с 3 строки)
            int rowActive = 3;
            foreach (var item in ActiveWindowsData)
            {
                worksheet.Cells[rowActive, 1] = item.Title;
                worksheet.Cells[rowActive, 2] = item.ActualTime?.ToString() ?? "";
                rowActive++;
            }

            int rowDeductions = 3;
            foreach (var item in WriteDownData)
            {
                worksheet.Cells[rowDeductions, 4] = item.Reason;
                worksheet.Cells[rowDeductions, 5] = item.ActualTime?.ToString() ?? "";
                worksheet.Cells[rowDeductions, 6] = item.Comment;
                rowDeductions++;
            }

            int rowAbsences = 3;
            foreach (var item in MissedDaysData)
            {
                worksheet.Cells[rowAbsences, 8] = item.Reason;
                worksheet.Cells[rowAbsences, 9] = item.DatetimeStart.ToString("dd.MM.yyyy HH:mm:ss");
                worksheet.Cells[rowAbsences, 10] = item.DatetimeFinish.ToString("dd.MM.yyyy HH:mm:ss");
                worksheet.Cells[rowAbsences, 11] = item.Comment;
                rowAbsences++;
            }

            // Границы для таблицы "Активные окна"
            Range activeWindowRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[rowActive - 1, 2]];
            Borders activeWindowBorders = activeWindowRange.Borders;
            activeWindowBorders.LineStyle = XlLineStyle.xlContinuous;
            activeWindowBorders.Weight = XlBorderWeight.xlThin;

            // Границы для таблицы "Списания"
            Range deductionsRange = worksheet.Range[worksheet.Cells[1, 4], worksheet.Cells[rowDeductions - 1, 6]];
            Borders deductionsBorders = deductionsRange.Borders;
            deductionsBorders.LineStyle = XlLineStyle.xlContinuous;
            deductionsBorders.Weight = XlBorderWeight.xlThin;

            // Границы для таблицы "Отсутствия"
            Range absencesRange = worksheet.Range[worksheet.Cells[1, 8], worksheet.Cells[rowAbsences - 1, 11]];
            Borders absencesBorders = absencesRange.Borders;
            absencesBorders.LineStyle = XlLineStyle.xlContinuous;
            absencesBorders.Weight = XlBorderWeight.xlThin;

            // Автоподбор ширины столбцов
            worksheet.Columns.AutoFit();
        }
    }
}