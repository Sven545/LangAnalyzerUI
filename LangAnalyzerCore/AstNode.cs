using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangAnalyzerCore
{
    public class AstNode
    {
        public string Type { get; set; }
        public string Value { get; set; }

        //public AstNode Parent { get; set; }
        public List<AstNode> Childs { get; set; } = new List<AstNode>();

        public void AddChild(AstNode node)
        {
            Childs.Add(node);
        }

        public override string ToString()
        {
            return $"Type: {Type}, Value: {Value}, CountChilds: {Childs.Count()}";
        }
    }
}
