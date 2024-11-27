using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryTranslator
{
    public struct Keyword
    {
        public string word;
        public Lexems lexem;

        public Keyword(string word, Lexems lexem)
        {
            this.word = word;
            this.lexem = lexem;
        }
    }
}
