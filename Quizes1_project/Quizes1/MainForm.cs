using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static Quizes1.Program;

namespace Quizes1
{
    public partial class MainForm : Form
    {
        private TestData testData;
        private string selectedFilePath;

        public MainForm()
        {
            
            CreateMainMenu();
        }

        private void CreateMainMenu()
        {
            this.Text = "Тестирование";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            var selectFileButton = new Button()
            {
                Text = "Выбрать файл теста",
                Size = new Size(200, 50),
                Location = new Point(150, 50),
                Font = new Font("Arial", 12)
            };
            selectFileButton.Click += SelectFileButton_Click;

            var startTestButton = new Button()
            {
                Text = "Начать тест",
                Size = new Size(200, 50),
                Location = new Point(150, 120),
                Font = new Font("Arial", 12),
                Enabled = false
            };
            startTestButton.Click += StartTestButton_Click;

            var fileLabel = new Label()
            {
                Text = "Файл не выбран",
                Location = new Point(150, 200),
                Size = new Size(300, 30),
                Font = new Font("Arial", 10)
            };

            this.Controls.AddRange(new Control[] { selectFileButton, startTestButton, fileLabel });

            // Сохраняем ссылки для обновления
            this.startTestButton = startTestButton;
            this.fileLabel = fileLabel;
        }

        private Button startTestButton;
        private Label fileLabel;

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                openFileDialog.Title = "Выберите файл теста";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                    try
                    {
                        testData = ParseTestFile(selectedFilePath);
                        fileLabel.Text = $"Выбран: {Path.GetFileName(selectedFilePath)}";
                        startTestButton.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки файла: {ex.Message}", "Ошибка");
                    }
                }
            }
        }

        private void StartTestButton_Click(object sender, EventArgs e)
        {
            if (testData != null)
            {
                var testForm = new TestForm(testData);
                testForm.ShowDialog();
            }
        }

        private TestData ParseTestFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            var testData = new TestData();
            int currentLine = 0;

            // Читаем заголовок теста
            if (currentLine >= lines.Length) throw new Exception("Файл пустой");
            testData.Title = lines[currentLine++].Trim();

            // Читаем вопросы
            while (currentLine < lines.Length)
            {
                // Пропускаем пустые строки
                while (currentLine < lines.Length && string.IsNullOrWhiteSpace(lines[currentLine]))
                    currentLine++;

                if (currentLine >= lines.Length) break;

                // Если нашли разделитель результатов, выходим
                if (lines[currentLine].Trim() == "---RESULTS---")
                {
                    currentLine++;
                    break;
                }

                // Читаем вопрос
                var question = new Question { Text = lines[currentLine++].Trim() };

                // Читаем варианты ответов
                while (currentLine < lines.Length && !string.IsNullOrWhiteSpace(lines[currentLine]))
                {
                    var answerParts = lines[currentLine].Split(';');
                    if (answerParts.Length >= 2)
                    {
                        var answer = new Answer
                        {
                            Text = answerParts[0].Trim(),
                            Points = int.Parse(answerParts[1].Trim())
                        };
                        question.Answers.Add(answer);
                    }
                    currentLine++;
                }

                testData.Questions.Add(question);
            }

            // Читаем результаты
            while (currentLine < lines.Length)
            {
                if (string.IsNullOrWhiteSpace(lines[currentLine]))
                {
                    currentLine++;
                    continue;
                }

                var resultParts = lines[currentLine].Split(';');
                if (resultParts.Length >= 3)
                {
                    var result = new TestResult
                    {
                        MinScore = int.Parse(resultParts[0].Trim()),
                        MaxScore = int.Parse(resultParts[1].Trim()),
                        Text = resultParts[2].Trim()
                    };
                    testData.Results.Add(result);
                }
                currentLine++;
            }

            return testData;
        }
    }
}
