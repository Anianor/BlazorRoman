namespace BlazorRoman.Models
{
    public class ReadDoctors
    {
        public string FullName { get; set; }
        public int? Cabinet { get; set; }
        public string? Specialization { get; set; }
        public int? Region { get; set; }
        public ReadDoctors()
        {
            FullName = null;
            Cabinet = 0;
            Specialization = null;
            Region = 0;
        }
    }
}
