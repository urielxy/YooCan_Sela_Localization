using System;

namespace Alto.Domain.Benefits
{
    public class FileUpload
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsUsed { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        
    }
}