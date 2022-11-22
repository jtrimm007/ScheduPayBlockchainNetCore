using MongoDB.Bson.Serialization.Attributes;
using ScheduPayBlockchainNetCore.Blocks;
using System;


namespace ScheduPayBlockchainNetCore.Models
{
    [BsonDiscriminator("GenesisBlock")]
    public sealed class GenesisBlock : IBlock
    {
        public string CreationTimestamp { get; set; }

        /// <summary>
        /// Frequency needs to be set in the ServiceBlock
        /// </summary>
        //public int Frequency { get; set; }
        public string LastHash { get; set; } = null;

        /// <summary>
        /// Starting hash is the LCP's Unique Id
        /// </summary>
        /// <value>
        /// The LCP's Unique Id.
        /// </value>
        public string Hash { get; set; }
        private static GenesisBlock _instance = null;
        public static GenesisBlock Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new GenesisBlock();
                        }
                    }
                }
                return _instance;
            }
        }
        private static readonly object padlock = new object();

        public GenesisBlock(string creationTimestamp, string lastHash, string hash)
        {
            CreationTimestamp = creationTimestamp;
            LastHash = lastHash;
            Hash = hash;
        }

        public GenesisBlock(string hash)
        {
            CreationTimestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            Hash = hash;
        }

        GenesisBlock()
        {
            CreationTimestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        }
    }
}
