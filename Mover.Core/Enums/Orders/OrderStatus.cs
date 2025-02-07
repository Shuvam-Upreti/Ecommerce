using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Enums.Orders
{
    public class OrderStatus
    {
        public enum OrderStatusEnums
        {
            Pending = 1,
            Approved = 2,
            Delivered = 3,
            Returned = 4
        }
    }
}
