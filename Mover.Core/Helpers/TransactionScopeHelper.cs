using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mover.Core.Helpers
{
    public class TransactionScopeHelper
    {

        public static TransactionScope GetInstance()
        {
            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
