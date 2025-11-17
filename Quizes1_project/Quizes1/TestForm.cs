using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Quizes1.Program;

namespace Quizes1
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

        internal TestForm(TestData testData)
        {
            this.testData = testData;
            
            SetupTestForm();
            ShowQuestion(0);
        }

        private void SetupTestForm()
        {
            this.Text = testData.Title;
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            progressLabel = new Label()
            {
                Location = new Point(20, 20),
                Size = new Size(500, 20),
                Font = new Font("Arial", 10)
            };

            questionLabel = new Label()
            {
                Location = new Point(20, 50),
                Size = new Size(550, 60),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            answersPanel = new Panel()
            {
                Location = new Point(20, 120),
                Size = new Size(550, 250),
                AutoScroll = true
            };

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

            progressLabel.Text = $"Вопрос {questionIndex + 1} из {testData.Questions.Count}";

            questionLabel.Text = question.Text;

            answersPanel.Controls.Clear();

            int yPos = 10;
            for (int i = 0; i < question.Answers.Count; i++)
            {
                var radioButton = new RadioButton()
                {
                    Text = question.Answers[i].Text,
                    Tag = question.Answers[i].Points,
                    Location = new Point(10, yPos),
                    Size = new Size(500, 30),
                    Font = new Font("Arial", 10)
                };

                if (selectedAnswers.Count > questionIndex && selectedAnswers[questionIndex] == i)
                {
                    radioButton.Checked = true;
                }

                answersPanel.Controls.Add(radioButton);
                yPos += 35;
            }

            nextButton.Text = questionIndex == testData.Questions.Count - 1 ? "Завершить" : "Далее";
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
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
            // Determine result text
            string resultText = "Результат не определен";
            foreach (var result in testData.Results)
            {
                if (totalScore >= result.MinScore && totalScore <= result.MaxScore)
                {
                    resultText = result.Text;
                    break;
                }
            }

            // Show the 3D pyramid form
            string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "123.png");
            var pyramidForm = new ResultPyramidForm("Поздравляем с прохождением", totalScore, resultText, imagePath);
            pyramidForm.ShowDialog();
            this.Close();
        }
    }
}
