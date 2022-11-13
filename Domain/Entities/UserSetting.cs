using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TMS.Core.Domain.Entities
{
    public class UserSetting : BaseEntity<Guid>
    {


        #region Properties
        public Guid? DefaultTenantId { get; set; }
        public virtual Tenant? DefaultTenant { get; set; }
        public string Theme { get; set; } = string.Empty;
        public bool RightToLeft { get; set; }=false;
        public bool DarkMode { get; set; }=false;
        public string Culture { get; set; }="en-US";
        public Guid UserId { get; set; }
        public virtual User User { get; set; }



        #endregion



    }
}
