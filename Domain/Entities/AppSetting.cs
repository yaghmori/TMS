using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TMS.Core.Domain.Entities
{
    public class AppSetting : BaseEntity<Guid>
    {


        #region Properties
        public string Key { get; set; }=string.Empty;   
        public string Value { get; set; }= string.Empty;
        public string? Description { get; set; }
        public bool? IsReadOnly { get; set; }


        #endregion



    }
}
