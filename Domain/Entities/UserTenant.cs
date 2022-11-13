namespace TMS.Core.Domain.Entities
{
    public class UserTenant : BaseEntity<Guid>
    {

        public UserTenant()
        {
        }





        #region Properties

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public Guid TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }


        #endregion

    }

}
