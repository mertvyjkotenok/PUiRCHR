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
using Quizes2.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Quizes2
{
    /// <summary>
    /// Логика взаимодействия для TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private TestData testData;
        private int currentQuestion = 0;
        private int totalScore = 0;

        private List<int> selectedAnswers = new();

        internal TestWindow(TestData data)
        {
            InitializeComponent();
            testData = data;

            Title = testData.Title;
            ShowQuestion(0);
        }

        private void ShowQuestion(int index)
        {
            if (index >= testData.Questions.Count)
            {
                FinishTest();
                return;
            }

            var q = testData.Questions[index];

            ProgressText.Text = $"Вопрос {index + 1} из {testData.Questions.Count}";
            QuestionText.Text = q.Text;

            AnswersPanel.Children.Clear();

            for (int i = 0; i < q.Answers.Count; i++)
            {
                var rb = new RadioButton()
                {
                    Content = q.Answers[i].Text,
                    Tag = q.Answers[i].Points,
                    Margin = new Thickness(0, 8, 0, 8),
                    FontSize = 16
                };

                if (selectedAnswers.Count > index && selectedAnswers[index] == i)
                    rb.IsChecked = true;

                AnswersPanel.Children.Add(rb);
            }

            NextButton.Content = index == testData.Questions.Count - 1
                ? "Завершить"
                : "Далее";
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = -1;
            int points = 0;

            for (int i = 0; i < AnswersPanel.Children.Count; i++)
            {
                if (AnswersPanel.Children[i] is RadioButton rb && rb.IsChecked == true)
                {
                    selectedIndex = i;
                    points = (int)rb.Tag;
                    break;
                }
            }

            if (selectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите ответ", "Внимание");
                return;
            }

            if (selectedAnswers.Count > currentQuestion)
                selectedAnswers[currentQuestion] = selectedIndex;
            else
                selectedAnswers.Add(selectedIndex);

            totalScore += points;

            currentQuestion++;
            ShowQuestion(currentQuestion);
        }

        private void FinishTest()
        {
            string resultText = "Результат не определён";

            int maxscore = 0;

            foreach (var r in testData.Results)
            {
                if (totalScore >= r.MinScore && totalScore <= r.MaxScore)
                {
                    resultText = r.Text;
                    maxscore = r.MaxScore;
                    break;
                }

                
            }

            
            

            new ResultWindow(this.Title, "Поздравляем с прохождением!", totalScore, maxscore, resultText)
                .ShowDialog();

            Close();
        }

    }
}
