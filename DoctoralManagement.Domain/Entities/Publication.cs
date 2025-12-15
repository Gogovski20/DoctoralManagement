namespace DoctoralManagement.Domain.Entities
{
    public class Publication
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Journal { get; set; } = string.Empty;
        public DateTime PublishedOn { get; set; } 

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        
        public string Doi { get; set; } = string.Empty;
        public bool IsIndexedInScopus { get; set; }
        public bool IsIndexedInThomsonReuters { get; set; }
        public int EctsPoints { get; set; } 

        public bool IsApproved { get; set; } = false;

        public int? ActivityDocumentId { get; set; }
        public ActivityDocument? Document { get; set; }
    }
}
