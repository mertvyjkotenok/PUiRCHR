using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quizes
{

    public class Question
    {
        public string Text { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }

    public class Answer
    {
        public string Text { get; set; }
        public int Points { get; set; }
    }

    public class TestResult
    {
        public int MinScore { get; set; }
        public int MaxScore { get; set; }
        public string Text { get; set; }
    }

    public class TestData
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<TestResult> Results { get; set; } = new List<TestResult>();
    }

    
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
