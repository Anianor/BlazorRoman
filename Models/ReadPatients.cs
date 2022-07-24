namespace BlazorRoman.Models
{
    public class ReadPatients
    {
        
        public string Surname { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Floor { get; set; }
        public int? Region { get; set; }
        public ReadPatients()
        {

            Surname = null;
            Name = null;
            MiddleName = null;
            Address = null;
            DateOfBirth = null;
            Floor = null;
            Region =null;
        }
    }
}
