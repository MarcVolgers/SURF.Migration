using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Database.Models
{
    public enum DeliveryStatus
    {
        Unknown,
        ToDeliver,
        OfferedToDeliverer,
        SendFromDeliverer,
        Unavailable,
        Backorder,
        Failed,
        Cancelled
    }
}
