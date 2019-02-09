
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIS.Entities.Generic
{
    public class BaseEntity<T>
    { 
        public T Id { get; set; }
        public T TenantId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public virtual byte[] RowVersion { get; set; }
    }
}
