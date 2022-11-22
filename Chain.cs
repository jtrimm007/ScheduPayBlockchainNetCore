using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ScheduPayBlockchainNetCore.Blocks;
using ScheduPayBlockchainNetCore.Extensions;
using ScheduPayBlockchainNetCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScheduPayBlockchainNetCore
{
    public class Node<T>
    {
        public T Item { get; set; }
        public Node<T> Next { get; set; }
    }
    public class Chain : IEquatable<Chain>
    {
        #region Fields & Properties
        [BsonId]
        public string Id
        {
            get { return Genesis.Hash; }
            set
            {
                if (Head == null)
                {
                    var node = new Node<IBlock>
                    {
                        Item = new GenesisBlock(Timestamp.UnixTimestampToday().ToString(), null, value),
                        Next = null
                    };
                    Head = node;
                }

            }
        }
        private GenesisBlock _genesisBlock;
        public GenesisBlock Genesis
        {
            get
            {
                return Head.Item as GenesisBlock;
            }
            set
            {
                _genesisBlock = value;
            }

        }
        [BsonIgnore]
        public Node<IBlock> Head { get; set; }
        private IBlock[] _chainArray;
        public IBlock[] ChainArray
        {
            get
            {
                if (_chainArray == null)
                    _chainArray = this.ToArray();
                return _chainArray;
            }
            set
            {
                _chainArray = value;
            }
        }


        public ServiceBlock this[int index]
        {
            get
            {
                if (index < 0)
                    return null;
                if (index > this.Count())
                    return null;

                Node<IBlock> node = this.Head;
                for (int i = 0; i < index; i++)
                {

                    node = node.Next;
                }
                return node.Item as ServiceBlock;
            }
            set { this[index] = value; }
        }


        public int this[string hash] => GetIndex(hash);
        #endregion

        #region Constructors 
        public Chain(IBlock genesisBlock)
        {
            var node = new Node<IBlock>()
            {
                Item = genesisBlock,
                Next = null
            };

            Head = node;
        }

        public Chain(string jsonString)
        {

        }

        public Chain()
        {

        }
        #endregion
        public int GetIndex(string hash)
        {
            var count = this.Count();

            Node<IBlock> node = this.Head;
            for (int i = 0; i < count; i++)
            {

                if (node.Item.Hash == hash)
                    return i;

                node = node.Next;
            }

            return -1;
        }
        public void AddRange(List<ServiceBlock> blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (Genesis == null)
                    return;
                if (i == 0)
                    blocks[0].LastHash = Genesis.Hash;

                this.Add(blocks[i]);
            }
        }
        public void Add(IBlock item)
        {
            var node = new Node<IBlock>()
            {
                Item = item,
                Next = null
            };

            if (Head != null)
            {
                Node<IBlock> current = Head;

                while (current.Next != null)
                {
                    current = current.Next;
                }

                current.Next = node;
            }
            else
            {
                Head = node;
            }
        }
        public int Count()
        {
            int counter = 0;
            if (Genesis != null)
            {
                counter++;
                var current = Head;

                while (current.Next != null)
                {
                    counter++;
                    current = current.Next;
                }
            }
            return counter;
        }
        public InvoiceDetails GetInvoiceByHash(string hash)
        {
            var count = this.Count();

            Node<IBlock> node = this.Head.Next;
            for (int i = 0; i < count; i++)
            {
                if (node == null)
                    return null;

                ServiceBlock serviceBlock = node.Item as ServiceBlock;

                var checkInvoices = serviceBlock.Invoices.FirstOrDefault(x => x.ServiceBlockHash == hash);

                if (checkInvoices != null)
                    return checkInvoices;

                node = node.Next;
            }

            return null;

        }
        public InvoiceDetails GenerateInvoice(string hash)
        {
            var blockIndex = this[hash];
            var block = this[blockIndex];

            double invoiceAmount = 0;

            if (block == null)
                return null;

            // Check if mowed
            bool mowed = block.ServiceDetails.Mowed;

            if (mowed)
            {
                invoiceAmount += block.ServiceDetails.MowingRate;
            }

            // Check for other services
            var getServices = block.ServiceDetails.ListOfServices;

            if (getServices != null)
            {
                for (int i = 0; i < getServices.Count; i++)
                {
                    invoiceAmount += getServices[i].Amount;
                }
            }

            return new InvoiceDetails(hash, invoiceAmount);
        }
        /// <summary>
        /// Returns the index location of the sorted array. This search removes the Genesis block.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        private int BinarySearch(string hash)
        {
            int minNum = 0;
            int maxNum = (int)this.Count() - 1;

            while (minNum <= maxNum)
            {
                int mid = (minNum + maxNum) / 2;
                IBlock[] array = this.ToArray();
                array = array.Skip(1).ToArray();

                array = QuickSort(array, 0, array.Length - 1);

                if (Int32.Parse(hash) == array[mid].GetHashCode())
                {
                    return ++mid;
                }
                else if (Int32.Parse(hash) < array[mid].GetHashCode())
                {
                    maxNum = mid - 1;
                }
                else
                {
                    minNum = mid + 1;
                }
            }
            return -1;
        }
        public Node<IBlock> SetNodes()
        {
            GenesisBlock gen = GetGenesisBlock();
            if (gen == null)
                return null;
            IBlock[] newArray = new IBlock[ChainArray.Length];
            var length = ChainArray.Length;
            var toList = ChainArray.ToList();
            newArray[0] = ChainArray.FirstOrDefault(x => x.LastHash == null);
            for (int i = 1; i < length; i++)
            {
                newArray[i] = toList.FirstOrDefault(x => x.LastHash == newArray[i - 1].Hash);
            }

            Node<IBlock> node = new Node<IBlock>
            {
                Item = gen,
                Next = null,
            };

            Head = node;

            for (int a = 1; a < length; a++)
            {
                node.Next = new Node<IBlock> { Item = newArray[a], Next = null };
                node = node.Next;
            }

            return Head;
        }
        private GenesisBlock GetGenesisBlock()
        {
            return ChainArray.FirstOrDefault(x => x.LastHash == null) as GenesisBlock;
        }
        /// <summary>
        /// Quick Sorting based on the hash code. The right side needs to be array.length - 1.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public IBlock[] QuickSort(IBlock[] array, int left, int right)
        {

            if (left < right)
            {
                int pivot = Partition(array, left, right);

                if (pivot > 1)
                {
                    QuickSort(array, left, pivot - 1);
                }
                if (pivot + 1 < right)
                {
                    QuickSort(array, pivot + 1, right);
                }
            }



            return array;
        }

        private int Partition(IBlock[] array, int left, int right)
        {
            var pivot = array[left].GetHashCode();

            while (true)
            {
                while (array[left].GetHashCode() < pivot)
                {
                    left++;
                }

                while (array[right].GetHashCode() > pivot)
                {
                    right--;
                }

                if (left < right)
                {
                    if (array[left].GetHashCode() == array[right].GetHashCode()) return right;

                    IBlock temp = array[left];
                    array[left] = array[right];
                    array[right] = temp;
                }
                else
                {
                    return right;
                }
            }
        }

        public override string ToString()
        {
            List<object> chain = null;
            if (Head != null)
            {
                chain = new List<object>();
                chain.Add(Head.Item as GenesisBlock);
                //string genesisJson = JsonConvert.SerializeObject(Head.Item, Formatting.Indented);
                // var genesisJson = JsonSerializer.Serialize(Head.Item);
                var node = Head.Next;

                if (node == null)
                    return JsonConvert.SerializeObject(chain).ToString();

                while (node.Next != null)
                {
                    //var json = JsonSerializer.Serialize(node.Item);
                    chain.Add(node.Item as ServiceBlock);
                    node = node.Next;
                }

                // Serialize the last item
                //var lastItemJson = JsonSerializer.Serialize(node.Item);
                chain.Add(node.Item as ServiceBlock);


            }

            return chain == null ? "" : JsonConvert.SerializeObject(chain).ToString();
        }

        public Chain ToChain(string jsonString)
        {
            List<object> backToJson = JsonConvert.DeserializeObject<List<object>>(jsonString);


            var genesisString = backToJson[0].ToString();

            var genesisBlock = JsonConvert.DeserializeObject<GenesisBlock>(genesisString);

            // create a new Node<T> to store the blocks in
            Node<IBlock> nodes = new Node<IBlock>
            {
                Item = genesisBlock,
                Next = null
            };

            // put the genBlock in the Head as the Item
            Head = nodes;

            int sizeOfList = backToJson.Count;

            // put the Head in a holder
            var node = Head;

            for (var a = 1; a < sizeOfList; a++)
            {
                string serviceBlockString = backToJson[a].ToString();

                ServiceBlock block = JsonConvert.DeserializeObject<ServiceBlock>(serviceBlockString);
                node.Next = new Node<IBlock>
                {
                    Item = block,
                    Next = null
                };

                // move to the next node
                node = node.Next;
            }

            return this;
        }
        public List<ServiceBlock> GetUnpaidByPropertyId(string propertyId)
        {
            var unpaidBlocks = GetUnpaidServiceBlocks();

            return unpaidBlocks.Where(x => x.ServiceDetails.PropertyId == propertyId).ToList();
        }
        public List<ServiceBlock> GetPaidServiceBlockByPropId(string propertyId)
        {
            var paidBlocks = GetPaidServiceBlocks();

            return paidBlocks.Where(x => x.ServiceDetails.PropertyId == propertyId).ToList();
        }
        public List<ServiceBlock> GetPaidServiceBlocks()
        {
            var unpaidServiceBlocks = GetUnpaidServiceBlocks();
            var chain = this.ToList();

            // remove the genesis block
            chain.RemoveAt(0);

            foreach (var block in unpaidServiceBlocks)
            {
                chain.Remove(block);
            }

            return chain.Cast<ServiceBlock>().ToList();
        }
        public List<ServiceBlock> GetUnpaidServiceBlocks()
        {


            var list = this.ToList();
            list.RemoveAt(0);

            // Get all of the service blocks
            List<ServiceBlock> serviceBlocks = list.Cast<ServiceBlock>().ToList();

            // set a holder for the list to remove blocks from
            List<ServiceBlock> blockHolder = new List<ServiceBlock>(serviceBlocks);

            // Get all of the invoices
            var listOfInvoices = GetInvoices();


            // for each service block check if it has an invoice
            foreach (var serviceBlock in serviceBlocks)
            {
                // check list of invoices for service block hash
                var check = listOfInvoices.FirstOrDefault(x => x.ServiceBlockHash == serviceBlock.Hash);

                // if an invoice is found, remove the service block from the list
                if (check != null)
                    blockHolder.Remove(serviceBlock);

                if (!serviceBlock.ServiceDetails.Mowed && serviceBlock.ServiceDetails.ListOfServices.Count == 0)
                    blockHolder.Remove(serviceBlock);
            }


            return blockHolder;
        }
        public List<ServiceBlock> GetTodaySchedule()
        {
            var listOfServiceBlocks = GetServiceBlockList();
            List<ServiceBlock> todayList = new List<ServiceBlock>();
            foreach (var serviceBlock in listOfServiceBlocks)
            {
                DateTime timestamp = (DateTime)serviceBlock.DateTimestamp.ParseStringTimestamp();

                var newDatetime = timestamp.AddDays((double)serviceBlock.ServiceDetails.Frequency);

                if (newDatetime.Day == DateTime.Today.Day && newDatetime.Year == DateTime.Today.Year && newDatetime.Month == DateTime.Today.Month)
                {
                    todayList.Add(serviceBlock);
                }
            }
            return todayList;
        }

        /// <summary>
        /// The key is the propertyId. This is a list of the paid invoices for each property
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<InvoiceDetails>> PropertyInvoiceDictionary()
        {
            Dictionary<string, List<InvoiceDetails>> invoiceDictionary = new Dictionary<string, List<InvoiceDetails>>();

            List<InvoiceDetails> invoiceDetails = GetInvoices();
            var blockArray = QuickSort(this.ToArray(), 0, this.Count() - 1);


            foreach (var invoice in invoiceDetails)
            {
                // Get the serviceblock by the invoice hash
                var serviceBlock = BinarySearch(invoice.ServiceBlockHash);

                // if it is found
                if (serviceBlock != -1)
                {
                    // Get the block
                    var invoicedServiceBlock = blockArray[serviceBlock] as ServiceBlock;

                    // if the property hasn't been listed in the dictionary
                    if (!invoiceDictionary.ContainsKey(invoicedServiceBlock.ServiceDetails.PropertyId))
                    {


                        // add the property id as a key and the invoice to the list of values
                        invoiceDictionary.Add(invoicedServiceBlock.ServiceDetails.PropertyId, new List<InvoiceDetails>());

                        invoiceDictionary[invoicedServiceBlock.ServiceDetails.PropertyId].Add(invoice);

                    }
                    else
                    {
                        invoiceDictionary[invoicedServiceBlock.ServiceDetails.PropertyId].Add(invoice);
                    }
                }
            }
            return invoiceDictionary;
        }

        /// <summary>
        /// Gets all of the invoices from the chain.
        /// </summary>
        /// <returns></returns>
        public List<InvoiceDetails> GetInvoices()
        {
            List<InvoiceDetails> invoices = new List<InvoiceDetails>();
            var serviceBlocks = GetServiceBlocksWithInvoices();
            foreach (var block in serviceBlocks)
            {
                invoices.AddRange(block.Invoices);
            }

            return invoices;
        }

        /// <summary>
        /// Gets all of the blocks with at least one invoice. Invoice in service block does not reflect payment for services in that specific block. 
        /// </summary>
        /// <returns></returns>
        public List<ServiceBlock> GetServiceBlocksWithInvoices()
        {
            List<ServiceBlock> serviceBlocks = GetServiceBlockList();
            var list = serviceBlocks.Where(x => x.Invoices.Count > 0).ToList();
            return list;
        }

        public List<ServiceBlock> GetServiceBlockList()
        {
            var list = this.ToList();
            list.RemoveAt(0);
            List<ServiceBlock> serviceBlocks = list.Cast<ServiceBlock>().ToList();
            return serviceBlocks;
        }

        public List<IBlock> ToList()
        {
            List<IBlock> list = new List<IBlock>();
            if (Head == null)
                return list;

            list.Add(Head.Item);

            var node = Head.Next;

            if (node == null)
                return list;

            while (node.Next != null)
            {
                list.Add((IBlock)node.Next.Item);
                node = node.Next;
            }

            list.Add(node.Item);

            return list;
        }
        public IBlock[] ToArray()
        {
            var array = new IBlock[this.Count()];

            if (Head != null)
            {
                array[0] = Head.Item as GenesisBlock;
                int counter = 1;

                var node = Head.Next;

                if (node == null)
                    return array;

                while (node.Next != null)
                {
                    array[counter] = (IBlock)node.Item;
                    counter++;
                    node = node.Next;
                }

                // Add the last node
                array[counter] = node.Item as ServiceBlock;

            }
            return array;
        }

        public bool Equals(Chain other)
        {
            if (other == null) return false;

            var itemOne = this.Head.Item as GenesisBlock;
            var itemTwo = other.Head.Item as GenesisBlock;

            if (itemOne.CreationTimestamp != itemTwo.CreationTimestamp && itemOne.Hash != itemTwo.Hash)
                return false;
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            Chain chain = obj as Chain;

            if (chain == null)
                return false;
            else
                return Equals(chain);
        }
    }
}
