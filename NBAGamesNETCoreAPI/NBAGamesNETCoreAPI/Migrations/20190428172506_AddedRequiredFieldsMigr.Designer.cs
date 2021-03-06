﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NBAGamesNETCoreAPI.DataContexts;

namespace NBAGamesNETCoreAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190428172506_AddedRequiredFieldsMigr")]
    partial class AddedRequiredFieldsMigr
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NBAGamesNETCoreAPI.Models.GameToFirestore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("GameDateUTC");

                    b.Property<string>("GameId");

                    b.Property<DateTime>("GameStartDateTimeUTC");

                    b.Property<string>("GameStartTimeUTC");

                    b.Property<string>("LastUpdated");

                    b.Property<int>("OrderNo");

                    b.Property<int>("StatusNum");

                    b.Property<string>("TeamAFullName");

                    b.Property<string>("TeamALogoSrc");

                    b.Property<string>("TeamAScore");

                    b.Property<string>("TeamATriCode");

                    b.Property<string>("TeamBFullName");

                    b.Property<string>("TeamBLogoSrc");

                    b.Property<string>("TeamBScore");

                    b.Property<string>("TeamBTriCode");

                    b.HasKey("Id");

                    b.ToTable("AllGames");
                });

            modelBuilder.Entity("NBAGamesNETCoreAPI.Models.GuessFromAndroid", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ByPts");

                    b.Property<string>("GameId");

                    b.Property<string>("SelTeam");

                    b.Property<string>("UserId");

                    b.HasKey("ID");

                    b.ToTable("AllGuesses");
                });
#pragma warning restore 612, 618
        }
    }
}
