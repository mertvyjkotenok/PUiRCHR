using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quizes
{
    public partial class TestForm : Form
    {
        private TestData testData;
        private int currentQuestionIndex = 0;
        private int totalScore = 0;
        private List<int> selectedAnswers = new List<int>();

        private Label questionLabel;
        private Panel answersPanel;
        private Button nextButton;
        private Label progressLabel;

        public TestForm(TestData testData)
        {
            this.testData = testData;
            InitializeComponent();
            SetupTestForm();
            ShowQuestion(0);
        }

        private void SetupTestForm()
        {
            this.Text = testData.Title;
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Прогресс
            progressLabel = new Label()
            {
                Location = new Point(20, 20),
                Size = new Size(500, 20),
                Font = new Font("Arial", 10)
            };

            // Вопрос
            questionLabel = new Label()
            {
                Location = new Point(20, 50),
                Size = new Size(550, 60),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            // Панель для ответов с прокруткой
            answersPanel = new Panel()
            {
                Location = new Point(20, 120),
                Size = new Size(550, 250),
                AutoScroll = true
            };

            // Кнопка Далее
            nextButton = new Button()
            {
                Text = "Далее",
                Size = new Size(100, 40),
                Location = new Point(450, 380),
                Font = new Font("Arial", 11)
            };
            nextButton.Click += NextButton_Click;

            this.Controls.AddRange(new Control[] { progressLabel, questionLabel, answersPanel, nextButton });
        }

        private void ShowQuestion(int questionIndex)
        {
            if (questionIndex >= testData.Questions.Count)
            {
                ShowResults();
                return;
            }

            var question = testData.Questions[questionIndex];

            // Обновляем прогресс
            progressLabel.Text = $"Вопрос {questionIndex + 1} из {testData.Questions.Count}";

            // Показываем вопрос
            questionLabel.Text = question.Text;

            // Очищаем предыдущие ответы
            answersPanel.Controls.Clear();

            // Добавляем варианты ответов (RadioButton)
            int yPos = 10;
            for (int i = 0; i < question.Answers.Count; i++)
            {
                var radioButton = new RadioButton()
                {
                    Text = question.Answers[i].Text,
                    Tag = question.Answers[i].Points, // Сохраняем баллы в Tag
                    Location = new Point(10, yPos),
                    Size = new Size(500, 30),
                    Font = new Font("Arial", 10)
                };

                // Если ответ уже был выбран ранее, восстанавливаем выбор
                if (selectedAnswers.Count > questionIndex && selectedAnswers[questionIndex] == i)
                {
                    radioButton.Checked = true;
                }

                answersPanel.Controls.Add(radioButton);
                yPos += 35;
            }

            // Обновляем текст кнопки
            nextButton.Text = questionIndex == testData.Questions.Count - 1 ? "Завершить" : "Далее";
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            // Сохраняем выбранный ответ
            int selectedIndex = -1;
            for (int i = 0; i < answersPanel.Controls.Count; i++)
            {
                if (answersPanel.Controls[i] is RadioButton radio && radio.Checked)
                {
                    selectedIndex = i;
                    totalScore += (int)radio.Tag;
                    break;
                }
            }

            if (selectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите ответ", "Внимание");
                return;
            }

            // Сохраняем выбор
            if (selectedAnswers.Count > currentQuestionIndex)
            {
                selectedAnswers[currentQuestionIndex] = selectedIndex;
            }
            else
            {
                selectedAnswers.Add(selectedIndex);
            }

            currentQuestionIndex++;
            ShowQuestion(currentQuestionIndex);
        }

        private void ShowResults()
        {
            string resultText = "Результат не определен";

            foreach (var result in testData.Results)
            {
                if (totalScore >= result.MinScore && totalScore <= result.MaxScore)
                {
                    resultText = result.Text;
                    break;
                }
            }

            var resultForm = new Form()
            {
                Text = "Результат теста",
                Size = new Size(500, 300),
                StartPosition = FormStartPosition.CenterScreen
            };

            var titleLabel = new Label()
            {
                Text = "Ваш результат:",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(400, 30)
            };

            var scoreLabel = new Label()
            {
                Text = $"Набрано баллов: {totalScore}",
                Font = new Font("Arial", 12),
                Location = new Point(20, 60),
                Size = new Size(400, 30)
            };

            var resultLabel = new Label()
            {
                Text = resultText,
                Font = new Font("Arial", 11),
                Location = new Point(20, 100),
                Size = new Size(440, 150),
                TextAlign = ContentAlignment.TopLeft
            };

            var closeButton = new Button()
            {
                Text = "Закрыть",
                Size = new Size(100, 40),
                Location = new Point(350, 200)
            };
            closeButton.Click += (s, e) => { this.Close(); resultForm.Close(); };

            resultForm.Controls.AddRange(new Control[] { titleLabel, scoreLabel, resultLabel, closeButton });
            resultForm.ShowDialog();
            this.Close();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {

        }
    }
}
