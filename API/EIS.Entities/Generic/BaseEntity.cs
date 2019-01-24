
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIS.Entities.Generic
{
    public class BaseEntity<T>
    { 
        public T Id { get; set; }
        public T TenantId { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public virtual byte[] RowVersion { get; set; }
    }
}
