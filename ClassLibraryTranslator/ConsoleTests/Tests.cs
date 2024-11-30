using System;
using ClassLibraryTranslator;

namespace ConsoleTests
{
    class Tests
    {
        static void Main(string[] args)
        {
            string path = "C:\\Users\\Администратор\\Desktop\\Translator\\TestWhileEasy.txt";

            // Создание экземпляров зависимых классов
            var reader = new Reader();
            var nameTable = new NameTable();
            var errors = new Errors();
            var lexicalAnalyzer = new LexicalAnalyzer(reader);
            var codeGenerator = new CodeGenerator(nameTable);
            var syntaxAnalyzer = new SyntaxAnalyzer(lexicalAnalyzer, codeGenerator, nameTable, errors, reader);

            // Выполнение тестов
            Test1(path, reader, lexicalAnalyzer);
            //Test2(path, reader, nameTable, lexicalAnalyzer);
            //Test3(path, reader, syntaxAnalyzer, errors, codeGenerator);

            Console.ReadLine();
        }

        /// <summary>
        /// Тест Лексического Анализатора
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public static void Test1(string path, Reader reader, LexicalAnalyzer lexicalAnalyzer)
        {
            // Инициализация
            reader.Initialize(path);

            // Проход по всем лексемам
            while (lexicalAnalyzer.Lexem != Lexems.EOF)
            {
                lexicalAnalyzer.ParseNextLexem();
                Console.WriteLine(lexicalAnalyzer.Lexem);
            }

            reader.Close(); // Закрытие Reader
        }

        /// <summary>
        /// Тест таблицы имён
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public static void Test2(string path, Reader reader, NameTable nameTable, LexicalAnalyzer lexicalAnalyzer)
        {
            // Инициализация
            reader.Initialize(path);
            nameTable.Initialize();
            // Обработка лексем
            while (lexicalAnalyzer.Lexem != Lexems.EOF)
            {
              
                if (lexicalAnalyzer.Lexem == Lexems.Name)
                {
                    string currentName = lexicalAnalyzer.Name;

                    // Попытка найти идентификатор
                    var foundIdentifier = nameTable.FindIdentifier(currentName);
                    if (foundIdentifier == null)
                    {
                        // Если идентификатор не найден, добавляем его в таблицу имен
                        nameTable.AddIdentifier(currentName, tCat.Var, tType.Int);
                    }
                }

                lexicalAnalyzer.ParseNextLexem();

            }

            // Получение и вывод идентификаторов
            var identifiers = nameTable.GetIdentifiers();
            foreach (var ident in identifiers)
            {
                Console.WriteLine($"Имя идентификатора: {ident.name}"); // Вывод имени идентификатора
                Console.WriteLine($"Категория: {ident.category}");
                Console.WriteLine($"Тип: {ident.type}");
            }

            reader.Close(); // Закрытие Reader
        }

        /// <summary>
        /// Тест компиляции и синтаксического анализатора
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public static void Test3(string path, Reader reader, SyntaxAnalyzer syntaxAnalyzer, Errors errors, CodeGenerator codeGenerator)
        {
            // Инициализация
            reader.Initialize(path);

            // Компиляция
            syntaxAnalyzer.Compile();

            // Вывод ошибок
            foreach (string err in errors.GetErrors)
            {
                Console.WriteLine(err);
            }

            // Вывод сгенерированного кода
            foreach (string str in codeGenerator.Code)
            {
                Console.WriteLine(str);
            }

            reader.Close(); // Закрытие Reader
        }
    }
}