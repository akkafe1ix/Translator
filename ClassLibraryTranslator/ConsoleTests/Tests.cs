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
            string path = "C:\\Users\\Администратор\\Desktop\\Translator\\TestElseIfEasy.txt";
            //Test1(path);
            //Test2(path);
            Test3(path);
            Console.ReadLine();
        }
        /// <summary>
        /// Тест Лексического Анализатора
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        static public void Test1(string path)
        {
            // Инициализация
            Reader.Initialize(path);

            //Проход по всем лексемам
            while (LexicalAnalyzer.Lexem!=Lexems.EOF)
            {
                Console.WriteLine(LexicalAnalyzer.Lexem);
                LexicalAnalyzer.ParseNextLexem();
            }
        }
        
        /// <summary>
        /// Тест таблицы имён
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        static public void Test2(string path)
        {
            // Инициализация
            Reader.Initialize(path);
            NameTable.Initialize();

            // Обработка лексем
            while (LexicalAnalyzer.Lexem != Lexems.EOF)
            {
                if (LexicalAnalyzer.Lexem == Lexems.Name)
                {
                    string currentName = LexicalAnalyzer.Name;

                    // Попытка найти идентификатор
                    try
                    {
                        Identifier? foundIdentifier = NameTable.FindIdentifier(currentName);
                        // Если идентификатор найден, пропускаем его
                    }
                    catch (KeyNotFoundException)
                    {
                        // Если идентификатор не найден, добавляем его в таблицу имен
                        NameTable.AddIdentifier(currentName, tCat.Var);
                    }
                }

                // Переход к следующей лексеме
                LexicalAnalyzer.ParseNextLexem(); // Разбор следующей лексемы
            }

            // Получение и вывод идентификаторов
            var identifiers = NameTable.GetIdentifiers();
            foreach (var ident in identifiers)
            {
                Console.WriteLine($"Имя идентификатора: {ident.name}"); // Вывод имени идентификатора
                Console.WriteLine($"Категория: {ident.category}");
                Console.WriteLine($"Тип: {ident.type}");
            }
        }

        /// <summary>
        /// Тест компиляции и синтаксического анализатора
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        static public void Test3(string path)
        {
            Reader.Initialize(path);

            SyntaxAnalyzer.Compile();
            foreach (string err in Errors.GetErrors)
            {
                Console.WriteLine(err);
            }
            foreach (string str in CodeGenerator.code)
            {
                Console.WriteLine(str);
            }
        }
    }
}
