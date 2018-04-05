using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WebAppP2P.Core.Messages;

namespace WebAppP2P.Core.Database
{
    public class ApplicationDatabase : DbContext
    {
        public ApplicationDatabase(DbContextOptions<ApplicationDatabase> options) : base(options)
        {

        }

        public virtual DbSet<EncryptedMessageStore> Messages { get; set; }

        public virtual DbSet<Node> Nodes { get; set; }

        public virtual DbSet<Block> BlockChain { get; set; }

        public virtual DbSet<NodeStatistics> Statistics { get; set; }

        public virtual DbSet<BlockMessages> BlockMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlockMessages>()
                .HasKey(c => new { c.BlockHash, c.StoreId });
        }
    }

    public class EncryptedMessageStore : EncryptedMessage
    {
        public EncryptedMessageStore()
        {

        }

        public EncryptedMessageStore(EncryptedMessage message)
        {
            Id = message.Id;
            Content = message.Content;
            From = message.From;
            FromKey = message.FromKey;
            IV = message.IV;
            Nonce = message.Nonce;
            Timestamp = message.Timestamp;
            Title = message.Title;
            To = message.To;
            ToKey = message.ToKey;
        }

        [Key]
        public ulong StoreId { get; set; }

        public List<BlockMessages> BlockMessages { get; set; }
    }

    public class Node
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public bool? IsActive { get; set; }
        public long? LastActiveTimestamp { get; set; }

        public List<NodeStatistics> Statistics { get; set; }
    }

    public class NodeStatistics
    {
        [Key]
        public int Id { get; set; }     
        public int NodeId { get; set; }
        public bool IsSuccess { get; set; }
        public int Latency { get; set; }
        public long Timestamp { get; set; }

        [ForeignKey("NodeId")]
        public Node Node { get; set; }
    }

    public class BlockMessages
    {
        public string BlockHash { get; set; }
        public ulong StoreId { get; set; }

        [ForeignKey("BlockHash")]
        public Block Block { get; set; }
        [ForeignKey("StoreId")]
        public EncryptedMessageStore EncryptedMessageStore { get; set; }
    }

    public class Block
    {
        [Key]
        public string BlockHash { get; set; }
        public string BlockHashPrevious { get; set; }
        public long Timestamp { get; set; }
        public ulong Nonce { get; set; }
        public uint Length { get; set; }

        public List<BlockMessages> BlockMessages { get; set; }

        [ForeignKey("BlockHashPrevious")]
        public Block PreviousBlock { get; set; }
        public bool IsInMainChain { get; set; }
    }

}
