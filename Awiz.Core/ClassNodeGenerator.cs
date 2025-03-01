﻿using Awiz.Core.CodeInfo;
using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    internal class ClassNodeGenerator : IClassNodeGenerator
    {
        private Dictionary<string, INode> _nodeMap = new();

        public void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to)
        {
            var node1 = _nodeMap[from.Id];
            var node2 = _nodeMap[to.Id];

            graph.AddEdge(node1, node2);
        }

        public void CreateClassNode(IGraph graph, ClassInfo classInfo)
        {
            var node = graph.AddNode("Class");

            _nodeMap[classInfo.Id] = node;

            node.Grid.FieldText[0][0] = classInfo.Name;

            node.Grid.FieldText[0][1] = classInfo.Properties.Aggregate("", (current, prop) => current + $"{prop}\n");
            node.Grid.FieldText[0][2] = classInfo.Methods.Aggregate("", (current, method) => current + $"{method}\n");

            node.Width = 120;
            node.Height = 160;
        }
    }
}
