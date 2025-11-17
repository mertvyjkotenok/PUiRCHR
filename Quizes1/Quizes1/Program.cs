namespace Quizes1
{
    internal static class Program
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


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}