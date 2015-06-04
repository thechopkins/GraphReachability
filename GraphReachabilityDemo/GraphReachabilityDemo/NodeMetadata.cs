using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphReachabilityDemo
{

    public class NodeMetadata
    {
        public int NodeIndex { get; set; }
        public bool Visisted { get; set; }
        public double AssignedRandomDepthNumber { get; set; }
        public double MinRandomDepth { get; set; }
    }

    public class NodeMetadataComparer : IComparer<NodeMetadata>
    {
        public int Compare(NodeMetadata x, NodeMetadata y)
        {
            return x.AssignedRandomDepthNumber.CompareTo(y.AssignedRandomDepthNumber);
        }
    }
}
