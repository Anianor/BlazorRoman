﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorRoman.Models
{
    public partial class Region
    {
        public Region()
        {
            Doctors = new HashSet<Doctors>();
            Patients = new HashSet<Patients>();
        }
        [Key]
        public int Id { get; set; }
        public int Number { get; set; }

        public virtual ICollection<Doctors> Doctors { get; set; }
        public virtual ICollection<Patients> Patients { get; set; }
    }
}