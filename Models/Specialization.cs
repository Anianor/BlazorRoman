﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorRoman.Models
{
    public partial class Specialization
    {
        public Specialization()
        {
            Doctors = new HashSet<Doctors>();
        }
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Doctors> Doctors { get; set; }
    }
}