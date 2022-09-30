using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ns.graph
{
    public class ns_latex
    {
        private string latex_fn;
        private Stack<string> EndTags; // Last in first out.
        public ns_latex(string latex_fn, string class_type, string option)
        {
            this.latex_fn = latex_fn;
            this.EndTags = new Stack<string>();
            File.WriteAllText(this.latex_fn, ""); // clear.
            string doc_class = string.Format("\\documentclass[{0}]{{{1}}}\n", option, class_type);
            File.AppendAllText(this.latex_fn, doc_class);
        }
        public ns_latex AddPackage(string package)
        {
            string pk = string.Format("\\usepackage{{{0}}}\n", package);
            this.AppendLatex(pk);
            return this;
        }
        public ns_latex Begin(string tag)
        {
            string begin = string.Format("\\begin{{{0}}} \n", tag);
            string end = string.Format("\\end{{{0}}}\n", tag);
            this.AppendLatex(begin);
            this.EndTags.Push(end);
            return this;
        }
        public ns_latex end(string tag)
        {
            string end = string.Format("\\end{{{0}}}\n", tag);
            this.AppendLatex(end);
            return this;
        }
        public ns_latex end()
        {
            if (this.EndTags.Count == 0) return this;
            string tag = this.EndTags.Pop();
            AppendLatex(tag);
            return this;
        }
        public ns_latex endAll()
        {
            while ( this.EndTags.Count >0)
            {
                string end = this.EndTags.Pop();
                this.AppendLatex(end);
            }
            return this;
        }
        public ns_latex AppendLatex(string content)
        {
            File.AppendAllText(this.latex_fn, content);
            return this;
        }
    }
}
