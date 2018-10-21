﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace data.Migrations
{
    [DbContext(typeof(TeamContext))]
    partial class TeamContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("Domain.Manager", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CurrentTeamId");

                    b.HasKey("Id");

                    b.HasIndex("CurrentTeamId")
                        .IsUnique();

                    b.ToTable("Managers");
                });

            modelBuilder.Entity("Domain.ManagerTeamHistory", b =>
                {
                    b.Property<Guid>("ManagerId");

                    b.Property<Guid>("TeamId");

                    b.HasKey("ManagerId", "TeamId");

                    b.ToTable("ManagerTeamHistory");
                });

            modelBuilder.Entity("Domain.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("HomeColorsId");

                    b.Property<string>("HomeStadium");

                    b.Property<string>("Nickname");

                    b.Property<string>("TeamName");

                    b.Property<string>("YearFounded");

                    b.HasKey("Id");

                    b.HasIndex("HomeColorsId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Domain.UniformColors", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ShirtPrimary")
                        .IsRequired();

                    b.Property<string>("ShirtSecondary")
                        .IsRequired();

                    b.Property<string>("ShirtTertiary")
                        .IsRequired();

                    b.Property<string>("ShortsPrimary")
                        .IsRequired();

                    b.Property<string>("ShortsSecondary")
                        .IsRequired();

                    b.Property<string>("Socks")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("UniformColors");
                });

            modelBuilder.Entity("Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Position");

                    b.Property<Guid?>("TeamId");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Player");
                });

            modelBuilder.Entity("Domain.Manager", b =>
                {
                    b.HasOne("Domain.Team")
                        .WithOne("Manager")
                        .HasForeignKey("Domain.Manager", "CurrentTeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("SharedKernel.PersonFullName", "NameFactory", b1 =>
                        {
                            b1.Property<Guid>("ManagerId");

                            b1.Property<string>("First");

                            b1.Property<string>("Last");

                            b1.ToTable("Managers");

                            b1.HasOne("Domain.Manager")
                                .WithOne("NameFactory")
                                .HasForeignKey("SharedKernel.PersonFullName", "ManagerId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("Domain.ManagerTeamHistory", b =>
                {
                    b.HasOne("Domain.Manager")
                        .WithMany("PastTeams")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Team", b =>
                {
                    b.HasOne("Domain.UniformColors", "HomeColors")
                        .WithMany()
                        .HasForeignKey("HomeColorsId");
                });

            modelBuilder.Entity("Player", b =>
                {
                    b.HasOne("Domain.Team")
                        .WithMany("Players")
                        .HasForeignKey("TeamId");

                    b.OwnsOne("SharedKernel.PersonFullName", "NameFactory", b1 =>
                        {
                            b1.Property<Guid>("PlayerId");

                            b1.Property<string>("First");

                            b1.Property<string>("Last");

                            b1.ToTable("Player");

                            b1.HasOne("Player")
                                .WithOne("NameFactory")
                                .HasForeignKey("SharedKernel.PersonFullName", "PlayerId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
