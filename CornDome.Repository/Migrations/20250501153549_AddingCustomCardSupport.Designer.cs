﻿// <auto-generated />
using System;
using CornDome.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CornDome.Repository.Migrations
{
    [DbContext(typeof(CardDatabaseContext))]
    [Migration("20250501153549_AddingCustomCardSupport")]
    partial class AddingCustomCardSupport
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.14");

            modelBuilder.Entity("CornDome.Models.Cards.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCustomCard")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("card");
                });

            modelBuilder.Entity("CornDome.Models.Cards.CardImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardImageTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<int>("RevisionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CardImageTypeId");

                    b.HasIndex("RevisionId");

                    b.ToTable("cardImage");
                });

            modelBuilder.Entity("CornDome.Models.Cards.CardImageType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Descriptor")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("cardImageType");
                });

            modelBuilder.Entity("CornDome.Models.Cards.CardRevision", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ability")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Attack")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Cost")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Defense")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("LandscapeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("RevisionNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SetId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TypeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("LandscapeId");

                    b.HasIndex("SetId");

                    b.HasIndex("TypeId");

                    b.ToTable("revision");
                });

            modelBuilder.Entity("CornDome.Models.Cards.CardSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("set");
                });

            modelBuilder.Entity("CornDome.Models.Cards.CardType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("cardType");
                });

            modelBuilder.Entity("CornDome.Models.Cards.Landscape", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("landscape");
                });

            modelBuilder.Entity("CornDome.Models.Cards.CardImage", b =>
                {
                    b.HasOne("CornDome.Models.Cards.CardImageType", "CardImageType")
                        .WithMany()
                        .HasForeignKey("CardImageTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CornDome.Models.Cards.CardRevision", "Revision")
                        .WithMany("CardImages")
                        .HasForeignKey("RevisionId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("CardImageType");

                    b.Navigation("Revision");
                });

            modelBuilder.Entity("CornDome.Models.Cards.CardRevision", b =>
                {
                    b.HasOne("CornDome.Models.Cards.Card", "Card")
                        .WithMany("Revisions")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CornDome.Models.Cards.Landscape", "Landscape")
                        .WithMany()
                        .HasForeignKey("LandscapeId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CornDome.Models.Cards.CardSet", "CardSet")
                        .WithMany()
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CornDome.Models.Cards.CardType", "CardType")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("CardSet");

                    b.Navigation("CardType");

                    b.Navigation("Landscape");
                });

            modelBuilder.Entity("CornDome.Models.Cards.Card", b =>
                {
                    b.Navigation("Revisions");
                });

            modelBuilder.Entity("CornDome.Models.Cards.CardRevision", b =>
                {
                    b.Navigation("CardImages");
                });
#pragma warning restore 612, 618
        }
    }
}
