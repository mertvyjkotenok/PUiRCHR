using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Quizes2.Models;

namespace Quizes2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestData testData;
        private string selectedFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt";

            if (dialog.ShowDialog() == true)
            {
                selectedFile = dialog.FileName;
                try
                {
                    testData = ParseTestFile(selectedFile);
                    FileLabel.Text = $"Выбран: {System.IO.Path.GetFileName(selectedFile)}";
                    StartBtn.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка");
                }
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            new TestWindow(testData).ShowDialog();
        }

        private TestData ParseTestFile(string filePath)
        {
            // Читаем все строки файла (UTF-8)
            var lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);
            var data = new TestData();

            if (lines.Length == 0)
                throw new Exception("Файл теста пустой.");

            int i = 0;

            // Заголовок теста (первой строкой)
            data.Title = lines[i++].Trim();

            // Читаем вопросы
            while (i < lines.Length)
            {
                // Пропускаем пустые строки
                while (i < lines.Length && string.IsNullOrWhiteSpace(lines[i]))
                    i++;

                if (i >= lines.Length) break;

                // Раздел результатов
                if (lines[i].Trim() == "---RESULTS---")
                {
                    i++; // переходим к разделу результатов
                    break;
                }

                // Вопрос
                var q = new Question
                {
                    Text = lines[i++].Trim()
                };

                // Варианты ответа: до пустой строки или до разделителя
                while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    var parts = lines[i].Split(';');
                    if (parts.Length >= 2)
                    {
                        string answerText = parts[0].Trim();
                        string pointsStr = parts[1].Trim();

                        if (!int.TryParse(pointsStr, out int points))
                        {
                            // Если не удалось распарсить баллы — выбрасываем понятную ошибку
                            throw new FormatException($"Неверный формат баллов в строке {i + 1}: \"{lines[i]}\".");
                        }

                        q.Answers.Add(new Answer
                        {
                            Text = answerText,
                            Points = points
                        });
                    }
                    else
                    {
                        // Можно решить иначе: пропустить или выбросить; я выбрасываю, чтобы заметить ошибку в файле
                        throw new FormatException($"Ожидалось минимум 2 поля через ';' в строке {i + 1}: \"{lines[i]}\".");
                    }

                    i++;
                }

                data.Questions.Add(q);
            }

            // Читаем результаты после ---RESULTS---
            while (i < lines.Length)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    i++;
                    continue;
                }

                var parts = lines[i].Split(';');
                if (parts.Length >= 3)
                {
                    if (!int.TryParse(parts[0].Trim(), out int minScore))
                        throw new FormatException($"Неверный MinScore в строке {i + 1}: \"{lines[i]}\".");

                    if (!int.TryParse(parts[1].Trim(), out int maxScore))
                        throw new FormatException($"Неверный MaxScore в строке {i + 1}: \"{lines[i]}\".");

                    var text = parts[2].Trim();

                    data.Results.Add(new TestResult
                    {
                        MinScore = minScore,
                        MaxScore = maxScore,
                        Text = text
                    });
                }
                else
                {
                    throw new FormatException($"Ожидалось 3 поля через ';' в строке результата {i + 1}: \"{lines[i]}\".");
                }

                i++;
            }

            return data;
        }
    }
}