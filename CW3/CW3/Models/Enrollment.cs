namespace CW3.Models
{
    public class Enrollment
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public string Study { get; set; }
        public int IdStudy { get; set; }
        public string StartDate { get; set; }
    }
}