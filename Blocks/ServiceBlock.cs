using MongoDB.Bson.Serialization.Attributes;
using ScheduPayBlockchainNetCore.Extensions;
using ScheduPayBlockchainNetCore.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ScheduPayBlockchainNetCore.Blocks
{
    [BsonDiscriminator("ServiceBlock")]
    public class ServiceBlock : IBlock, IComparable<ServiceBlock>, IEquatable<ServiceBlock>
    {
        private string _dateTimestamp { get; set; }
        public string DateTimestamp
        {
            get
            {
                if (_dateTimestamp == null)
                {
                    _dateTimestamp = Timestamp.UnixTimestampToday().ToString();
                }
                return _dateTimestamp;
            }
            set { _dateTimestamp = value; }
        }

        public string LastHash { get; set; }
        private string _hash;
        public string Hash
        {
            get
            {
                if (_hash == null)
                {
                    int outHash;
                    var result = Int32.TryParse(LastHash, out outHash);
                    HashAlgorithm sha = SHA256.Create();
                    double valueToHash = (double.Parse(DateTimestamp) + (result == true ? outHash : 0));
                    byte[] convertyLongToByte = BitConverter.GetBytes(valueToHash);
                    byte[] hash = sha.ComputeHash(convertyLongToByte);
                    long convertByteArrayToLong = BitConverter.ToInt32(hash, 0);

                    _hash = convertByteArrayToLong.ToString();
                }
                return _hash;
            }
            set { _hash = value; }

        }
        public ServiceDetails ServiceDetails { get; set; }
        public List<InvoiceDetails> Invoices { get; set; } = new List<InvoiceDetails>();
        public ServiceBlock()
        {
        }
        public ServiceBlock(string lastHash)
        {
            LastHash = lastHash;
        }
        public ServiceBlock(string lastHash, ServiceDetails serviceDetails)
        {
            LastHash = lastHash;
            ServiceDetails = serviceDetails;
        }

        public ServiceBlock(string dateTimestamp, string lastHash, string hash)
        {
            DateTimestamp = dateTimestamp;
            LastHash = lastHash;
            Hash = hash;
        }

        public ServiceBlock(string dateTimestamp, string lastHash, ServiceDetails serviceDetails)
        {
            DateTimestamp = dateTimestamp;
            LastHash = lastHash;
            ServiceDetails = serviceDetails;
        }

        public ServiceBlock(string dateTimestamp, string lastHash, string hash, ServiceDetails serviceDetails)
        {
            DateTimestamp = dateTimestamp;
            LastHash = lastHash;
            //Hash = hash;
            ServiceDetails = serviceDetails;
        }

        public int CompareTo(ServiceBlock other)
        {
            if (other == null) return 1;

            return double.Parse(DateTimestamp).CompareTo(double.Parse(other.DateTimestamp));
        }

        public int CompareToHash(ServiceBlock other)
        {
            if (other == null) return 1;

            return double.Parse(Hash).CompareTo(double.Parse(other.Hash));

        }


        public static bool operator >(ServiceBlock serviceBlock, string dateTimestamp2)
        {
            return Int64.Parse(serviceBlock.DateTimestamp).CompareTo(Int64.Parse(dateTimestamp2)) > 0;
        }
        public static bool operator <(ServiceBlock serviceBlock, string dateTimestamp2)
        {
            return Int64.Parse(serviceBlock.DateTimestamp).CompareTo(Int64.Parse(dateTimestamp2)) < 0;
        }

        public static bool operator >=(ServiceBlock serviceBlock, string dateTimestamp2)
        {
            return Int64.Parse(serviceBlock.DateTimestamp).CompareTo(Int64.Parse(dateTimestamp2)) >= 0;
        }
        public static bool operator <=(ServiceBlock serviceBlock, string dateTimestamp2)
        {
            return Int64.Parse(serviceBlock.DateTimestamp).CompareTo(Int64.Parse(dateTimestamp2)) <= 0;
        }
        public bool EqualsHash(ServiceBlock other)
        {
            if (other == null) return false;

            if (this.Hash == other.Hash)
                return true;
            else
                return false;
        }
        public bool Equals(ServiceBlock other)
        {
            if (other == null) return false;

            if (this.DateTimestamp == other.DateTimestamp)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            ServiceBlock serviceBlock = obj as ServiceBlock;

            if (serviceBlock == null)
                return false;
            else
                return Equals(serviceBlock);
        }

        public override int GetHashCode()
        {
            return Int32.Parse(this.Hash);
        }

        public static bool operator ==(ServiceBlock serviceBlock, ServiceBlock serviceBlock2)
        {
            if ((object)serviceBlock == null || ((object)serviceBlock2) == null)
            {
                return Object.Equals(serviceBlock, serviceBlock2);
            }

            return serviceBlock.Equals(serviceBlock2);
        }

        public static bool operator !=(ServiceBlock serviceBlock, ServiceBlock serviceBlock2)
        {
            if ((object)serviceBlock == null || ((object)serviceBlock2) == null)
            {
                return !Object.Equals(serviceBlock, serviceBlock2);
            }

            return !(serviceBlock.Equals(serviceBlock2));
        }
    }
}
