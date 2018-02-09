namespace HiFaqBot01.Models
{
    public class QnAResult
    {
        public AnswerResult[] Answers { get; set; }
    }

    public class AnswerResult
    {
        public string Answer { get; set; }
        public string[] Questions { get; set; }
        public float Score { get; set; }
    }

}