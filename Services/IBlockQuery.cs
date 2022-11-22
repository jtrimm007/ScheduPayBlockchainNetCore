using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduPayBlockchainNetCore.Services
{
    public interface IBlockQuery<MobelType>
    {
        MobelType Find(MobelType type);
        List<MobelType> FindAll();
    }
}
