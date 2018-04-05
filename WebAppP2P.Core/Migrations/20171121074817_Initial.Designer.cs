﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core.Migrations
{
    [DbContext(typeof(ApplicationDatabase))]
    [Migration("20171121074817_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("WebAppP2P.Core.Database.Block", b =>
                {
                    b.Property<string>("BlockHash")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BlockHashPrevious");

                    b.Property<ulong>("Nonce");

                    b.Property<long>("Timestamp");

                    b.HasKey("BlockHash");

                    b.HasIndex("BlockHashPrevious");

                    b.ToTable("BlockChain");
                });

            modelBuilder.Entity("WebAppP2P.Core.Database.BlockMessages", b =>
                {
                    b.Property<string>("BlockHash");

                    b.Property<ulong>("StoreId");

                    b.HasKey("BlockHash", "StoreId");

                    b.HasIndex("StoreId");

                    b.ToTable("BlockMessages");
                });

            modelBuilder.Entity("WebAppP2P.Core.Database.EncryptedMessageStore", b =>
                {
                    b.Property<ulong>("StoreId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("From");

                    b.Property<string>("FromKey");

                    b.Property<string>("IV");

                    b.Property<string>("Id");

                    b.Property<ulong>("Nonce");

                    b.Property<long>("Timestamp");

                    b.Property<string>("Title");

                    b.Property<string>("To");

                    b.Property<string>("ToKey");

                    b.HasKey("StoreId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("WebAppP2P.Core.Database.Node", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool?>("IsActive");

                    b.Property<long?>("LastActiveTimestamp");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Nodes");
                });

            modelBuilder.Entity("WebAppP2P.Core.Database.NodeStatistics", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsSuccess");

                    b.Property<int>("Latency");

                    b.Property<int>("NodeId");

                    b.Property<long>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("NodeId");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("WebAppP2P.Core.Database.Block", b =>
                {
                    b.HasOne("WebAppP2P.Core.Database.Block", "PreviousBlock")
                        .WithMany()
                        .HasForeignKey("BlockHashPrevious");
                });

            modelBuilder.Entity("WebAppP2P.Core.Database.BlockMessages", b =>
                {
                    b.HasOne("WebAppP2P.Core.Database.Block", "Block")
                        .WithMany("BlockMessages")
                        .HasForeignKey("BlockHash")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebAppP2P.Core.Database.EncryptedMessageStore", "EncryptedMessageStore")
                        .WithMany()
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebAppP2P.Core.Database.NodeStatistics", b =>
                {
                    b.HasOne("WebAppP2P.Core.Database.Node", "Node")
                        .WithMany("Statistics")
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
