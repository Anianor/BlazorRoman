﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlazorRoman.Models
{
    public partial class RomanDBContext : DbContext
    {
        public RomanDBContext()
        {
        }

        public RomanDBContext(DbContextOptions<RomanDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cabinets> Cabinets { get; set; }
        public virtual DbSet<Doctors> Doctors { get; set; }
        public virtual DbSet<Patients> Patients { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Specialization> Specialization { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctors>(entity =>
            {
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("Full_Name");

                entity.HasOne(d => d.CabinetNavigation)
                    .WithMany(p => p.Doctors)
                    .HasForeignKey(d => d.Cabinet)
                    .HasConstraintName("FK__Doctors__Cabinet__32E0915F");

                entity.HasOne(d => d.RegionNavigation)
                    .WithMany(p => p.Doctors)
                    .HasForeignKey(d => d.Region)
                    .HasConstraintName("FK__Doctors__Region__38996AB5");

                entity.HasOne(d => d.SpecializationNavigation)
                    .WithMany(p => p.Doctors)
                    .HasForeignKey(d => d.Specialization)
                    .HasConstraintName("FK__Doctors__Special__35BCFE0A");
            });

            modelBuilder.Entity<Patients>(entity =>
            {
                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Floor).HasMaxLength(50);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(100)
                    .HasColumnName("Middle_Name");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Surname).HasMaxLength(100);

                entity.HasOne(d => d.RegionNavigation)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.Region)
                    .HasConstraintName("FK__Patients__Region__3E52440B");
            });

            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(            
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\BlazorRoman\DbForRoman.mdf;Integrated Security=True;Connect Timeout=30");           
        
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}