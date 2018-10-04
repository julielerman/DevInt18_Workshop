﻿using System;
using Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Data {
    public class TeamContext : DbContext {
        public TeamContext (DbContextOptions<TeamContext> options) : base (options) { }
        public TeamContext () {

        }
        public DbSet<Team> Teams { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlite ("Data Source=TeamData.db");
            }
        }
        protected override void OnModelCreating (ModelBuilder modelBuilder) {
           //not mapped yet
            modelBuilder.Ignore<ManagerTeamHistory> ();
            modelBuilder.Ignore<UniformColors> ();
            modelBuilder.Ignore<PersonFullName> ();
           //
            modelBuilder.Entity<Team> ()
                .Property (b => b.TeamName)
                .HasField ("_teamname");

           }
    }
}