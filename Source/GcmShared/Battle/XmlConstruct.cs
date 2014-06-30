using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GcmShared
{

    public class XmlConstruct
    {
        public StringBuilder sb;

        Stack<string> Tags;

        public XmlConstruct()
        {
            sb = new StringBuilder();
            Tags = new Stack<string>();
        }

        public void Insert(string id, object contents)
        {
            sb.Append('<');
            sb.Append(id);
            sb.Append('>');
            sb.Append(contents.ToString());
            sb.Append("</");
            sb.Append(id);
            sb.Append('>');
        }
        public void Open(string id)
        {
            sb.Append('<');
            sb.Append(id);
            sb.Append('>');

            Tags.Push(id);
        }
        public void Close(string id)
        {
            sb.Append("</");
            sb.Append(id);
            sb.Append('>');
        }

        public void Close()
        {
            sb.Append("</");
            sb.Append(Tags.Pop());
            sb.Append('>');
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
