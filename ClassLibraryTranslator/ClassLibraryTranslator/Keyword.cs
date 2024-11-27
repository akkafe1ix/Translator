using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryTranslator
{
    public struct Keyword
    {
        public string Word { get; }
        public Lexems Lexem { get; }

        public Keyword(string word, Lexems lexem)
        {
            Word = word;
            Lexem = lexem;
        }
    }

}
