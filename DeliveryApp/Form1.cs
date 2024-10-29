using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace DeliveryApp
{
    public partial class Form1 : Form
    {
        JArray jsonArray;
        JArray jsonArrayEnd;

        string folderPath1 = "";
        string folderPath2 = "";
        List<int> selectedId;
        public Form1()
        {
            InitializeComponent();
            string currentDirectory = Directory.GetCurrentDirectory();
            folderPath1 = Path.Combine(currentDirectory, "StandartLoggingPath.txt");
            label4.Text = folderPath1;
            string filePath = Path.Combine(currentDirectory,"db.json");
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                jsonArray = JArray.Parse(jsonContent);

                foreach (var item in jsonArray)
                {
                    if (item["delivery_area"] != null && !listBox1.Items.Contains(item["delivery_area"].ToString()))
                    {
                        listBox1.Items.Add(item["delivery_area"].ToObject<object>());
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("Ошибка поиска файла: " + e.Message);
                Logging.LoggingError(folderPath1, e.Message);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Ошибка ввода-вывода: " + ex.Message);
                Logging.LoggingError(folderPath1, ex.Message);
            }
            catch (JsonException e)
            {
                MessageBox.Show("Ошибка парсинга JSON: " + e.Message);
                Logging.LoggingError(folderPath1, e.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка: " + ex.Message);
                Logging.LoggingError(folderPath1, ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "blank.txt"
            };

            try
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    folderPath1 = Path.GetFullPath(dialog.FileName);
                    label4.Text = folderPath1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выборе файла: " + ex.Message);
                Logging.LoggingError(folderPath1, ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "blank.json"
            };

            try
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    folderPath2 = Path.GetFullPath(dialog.FileName);
                    label5.Text = folderPath2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выборе файла: " + ex.Message);
                Logging.LoggingError(folderPath1, ex.Message);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Logging.LoggingInfo(folderPath1, $"Пользователь нажал кнопку ОТПРАВИТЬ");
            if (folderPath1 == folderPath2)
            {
                MessageBox.Show("Выберете разные пути сохранения логов и выборки");
                return;
            }
            string SelectedRegion = listBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(SelectedRegion))
            {
                MessageBox.Show("Регион не выбран");
                Logging.LoggingError(folderPath1, "Пользователь не выбрал регион");
                return;
            }
            DateTime datePart = dateTimePicker1.Value;
            DateTime timePart = dateTimePicker2.Value;

            // Объединяем дату и время
            DateTime dateTime1 = new DateTime(datePart.Year, datePart.Month, datePart.Day, timePart.Hour, timePart.Minute, timePart.Second);
            jsonArrayEnd = new JArray();
            selectedId = new List<int>();
            try
            {
                foreach (var item in jsonArray)
                {
                    DateTime dateTime2 = DateTime.ParseExact(item["delivery_time"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    if ((item["delivery_area"].ToString() == SelectedRegion) && ((dateTime1.AddMinutes(30) >= dateTime2) && (dateTime1 <= dateTime2)))
                    {
                        jsonArrayEnd.Add(item);
                        selectedId.Add(Convert.ToInt32(item["order_id"]));
                    }
                }
            
                if (jsonArrayEnd.Count > 0)
                {
                    if (string.IsNullOrEmpty(folderPath2))
                    {
                        MessageBox.Show("Путь к файлу результатов не указан");
                        Logging.LoggingError(folderPath1, "Путь к файлу результатов не указан");
                        return;
                    }
                    string jsonString = jsonArrayEnd.ToString(Formatting.Indented);
                    File.WriteAllText(folderPath2, jsonString);
                    string selectedIdString = string.Join(", ", selectedId);
                    Logging.LoggingInfo(folderPath1, $"Пользователь получил список заказов со следующими Id: : {selectedIdString}");
                }
                else
                {
                    File.WriteAllText(folderPath2, $"[\"Не найдено заказов, попробуйте изменить условия поиска\"]");
                    Logging.LoggingInfo(folderPath1, "По результатам введенных данных пользователем не найдено ни одного заказа");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Строка не может быть преобразована в нужный формат: " + ex.Message);
                Logging.LoggingError(folderPath1, ex.Message);
            }
            catch (IOException ex) 
            {
                MessageBox.Show("Ошибка ввода-вывода: " + ex.Message);
                Logging.LoggingError(folderPath1, ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка: " + ex.Message);
                Logging.LoggingError(folderPath1, ex.Message);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime datePart = dateTimePicker1.Value;
            Logging.LoggingInfo(folderPath1, $"Пользователь выбрал новую дату: {datePart.Year}:{datePart.Month}:{datePart.Day}");
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            DateTime datePart = dateTimePicker2.Value;
            Logging.LoggingInfo(folderPath1, $"Пользователь выбрал новое время: {datePart.Hour}:{datePart.Minute}:{datePart.Second}");
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            Logging.LoggingInfo(folderPath1, $"Пользователь выбрал регион: {listBox1.Text}");
        }

        private void label4_TextChanged(object sender, EventArgs e)
        {
            Logging.LoggingInfo(folderPath1, $"Пользователь выбрал новый путь к логам: {label4.Text}");
        }

        private void label5_TextChanged(object sender, EventArgs e)
        {
            Logging.LoggingInfo(folderPath1, $"Пользователь выбрал путь к записи результирующего файла: {label5.Text}");
        }
    }
}
