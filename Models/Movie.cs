using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public partial class Movie
{
    [Key]
    [Column("ID")]
    [StringLength(100)]
    [Unicode(false)]
    public required string Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public required string Name { get; set; }

    public int Year { get; set; }

    public bool Watched { get; set; }
}
