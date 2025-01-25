using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; } = string.Empty;
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public IEnumerable<HtmlElement> Descendants()
        {
           
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                HtmlElement tmp = queue.Dequeue();
                foreach (HtmlElement child in tmp.Children)
                {
                    queue.Enqueue(child);
                }
                yield return tmp;
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
        public HashSet<HtmlElement> ElementsFitSelector(HtmlElement element, Selector selector, HashSet<HtmlElement> list)
        {
            IEnumerable<HtmlElement> children = element.Descendants();
            foreach (HtmlElement child in children.Skip(1))
            {          
                if (EqualElement(child, selector))
                {
                    if (selector.Child == null)
                    {
                        list.Add(child);
                    }
                    list = ElementsFitSelector(child, selector.Child, list);
                }
            }
            return list;
        }
        public bool EqualElement(HtmlElement element, Selector selector)
        {
            
            if (selector == null)
                return false;
            if (element == null)
                return false;
            if (selector.TagName != string.Empty && selector.TagName != element.Name)
            {
                return false;
            }       
            if (selector.Id != string.Empty && selector.Id != element.Id)
            {
                return false;
            }
            foreach (string c in selector.Classes)
            {
                if (!element.Classes.Contains(c))
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Id: {Id}");
            stringBuilder.AppendLine($"Name: {Name}");
            stringBuilder.AppendLine("Attributes:");

            foreach (var attribute in Attributes)
            {
                stringBuilder.AppendLine($"   {attribute.Key}: {attribute.Value}");
            }

            stringBuilder.AppendLine("Classes:");

            foreach (var className in Classes)
            {
                stringBuilder.AppendLine($"   {className}");
            }

            stringBuilder.AppendLine($"InnerHtml: {InnerHtml}");
            stringBuilder.AppendLine($"Parent: {Parent?.Id ?? Parent?.Name ?? "null"}");
            stringBuilder.AppendLine("Children:");

            foreach (var child in Children)
            {
                stringBuilder.AppendLine($"   {child.Name ?? child.Id}");
            }

            return stringBuilder.ToString();


 
        }

    }
}





      

       



        
