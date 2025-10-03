namespace DoctoralManagement.Application.DoctoralPrograms.Commands
{
    public class CreateDoctoralProgramResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ScientificArea { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
        public decimal TuitionFee { get; set; }
        public decimal InternationalTuitionFee { get; set; }
        public string SpecialRequirements { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
