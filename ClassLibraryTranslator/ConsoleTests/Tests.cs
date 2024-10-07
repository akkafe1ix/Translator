using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryTranslator;



namespace ConsoleTests
{
    class Tests
    {
        static void Main(string[] args)
        {
            string path = "C:\\Users\\Администратор\\Desktop\\Translator\\Test.txt";
            Test1(path);
            Console.ReadLine();
        }

        static public void Test1(string path)
        {
            Reader.Initialize(path);


            while (LexicalAnalyzer.CurrentLexem!=Lexems.EOF)
            {
                Console.WriteLine(LexicalAnalyzer.CurrentLexem);
                LexicalAnalyzer.ParseNextLexem();
            }


        }
    }
}
