namespace GamblingProject.Models
{
    public class ResponseDto
    {
        public ResponseDto(string title, string message, string status = "success")
        {
            Status = status;
            Message = message;
            Title = title;
        }

        public string Status { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
    }
}