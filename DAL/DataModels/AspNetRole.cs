using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

public partial class AspNetRole
{
    [Key]
    [StringLength(128)]
    public string AspNetRoleId { get; set; } = null!;

    [StringLength(256)]
    public string Name { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new List<AspNetUserRole>();
}
