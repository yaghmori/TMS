namespace TMS.Core.Domain.Entities
{
    public class Person : BaseEntity<Guid>
    {
        public Person()
        {
        }


        #region Properties







        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FatherName { get; set; }
        public string? NationalCode { get; set; }
        public string? EconomicalCode { get; set; }
        public string? BirthIdentityNumber { get; set; }
        public string? BirthIdentitySerial { get; set; }
        public string? InsuranceNumber { get; set; }
        public string? AdditionalInsuranceNumber { get; set; }
        public int? InsuranceHistory { get; set; }
        public int? Children { get; set; }
        public int? FamilyCount { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? BirthStatusDate { get; set; }
        public DateTime? MarriageDate { get; set; }
        public string? EducationalField { get; set; }
        public int? FieldOfActivity { get; set; }
        public string? Description { get; set; }
        public virtual User? User { get; set; }
        public Guid? UserId { get; set; }








        #endregion

    }
}
