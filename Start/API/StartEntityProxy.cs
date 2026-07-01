using Start.Interfaces;

namespace Start.API
{
    public readonly struct StartEntityProxy
    {
        public readonly int Index;
        public readonly IStartBaseRoot StartBaseRoot;

        public StartEntityProxy(IStartBaseRoot startBaseRoot, int index)
        {
            StartBaseRoot = startBaseRoot;
            Index = index;
        }

        public void ConnectNodes(params StartEntityProxy[] nodes)
        {
            switch (nodes.Length)
            {
                case 1:
                    StartBaseRoot.SetSNode(nodes[0].Index);
                    break;
                case 2:
                    StartBaseRoot.SetSNode(nodes[0].Index);
                    StartBaseRoot.SetENode(nodes[1].Index);
                    break;
            }

            ConnectObjects(nodes);
        }

        public void ConnectObjects(params StartEntityProxy[] objects)
        {
            foreach (StartEntityProxy @object in objects) StartBaseRoot.SetConnElem(@object.Index);
        }
    }
}