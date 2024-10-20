using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClassLibraryTranslator
{
    public static class SyntaxAnalyzer
    {
        /// <summary>
        /// Проверка лексемы
        /// </summary>
        /// <param name="expectLexem">Ожидаемая лексема</param>
        public static void CheckLexem(Lexems expectLexem)
        {
            if (LexicalAnalyzer.CurrentLexem != expectLexem)
                Errors.AddError($"Ожидалось {expectLexem}, а получили {LexicalAnalyzer.CurrentLexem} (Строка:{Reader.LineNumber}, позиция:{Reader.CharPositionInLine}, символ:'{Convert.ToChar(Reader.CurrentChar)}') ");
            else
                LexicalAnalyzer.ParseNextLexem();
        }

        /// <summary>
        /// Метод проверки объявления переменных
        /// </summary>
        public static void ParseDecVar()
        {
            CheckLexem(Lexems.Type);
            if (LexicalAnalyzer.CurrentLexem != Lexems.Name)
                Errors.AddError($"Ожидалось объявление переменной (Строка:{Reader.LineNumber}, позиция:{Reader.CharPositionInLine}, символ:'{Convert.ToChar(Reader.CurrentChar)}') ");
            else
            {
                NameTable.AddIdentifier(LexicalAnalyzer.CurrentName, tCat.Var, tType.Int);
                LexicalAnalyzer.ParseNextLexem();
            }

            while (LexicalAnalyzer.CurrentLexem == Lexems.Comma)
            {
                LexicalAnalyzer.ParseNextLexem();
                if (LexicalAnalyzer.CurrentLexem != Lexems.Name)
                    Errors.AddError($"Ожидалось объявление переменной (Строка:{Reader.LineNumber}, позиция:{Reader.CharPositionInLine}, символ:'{Convert.ToChar(Reader.CurrentChar)}') ");
                else
                {
                    NameTable.AddIdentifier(LexicalAnalyzer.CurrentName, tCat.Var, tType.Int);
                    LexicalAnalyzer.ParseNextLexem();
                }
            }
            CheckLexem(Lexems.Delimiter); //Проверка на перенос строки
        }

        /// <summary>
        /// Метод разбора последоовательности инструкций
        /// </summary>
        private static void ParseSequenceOfInstructions()
        {
            ParseInstruction();
            while (LexicalAnalyzer.CurrentLexem == Lexems.Delimiter)
            {
                LexicalAnalyzer.ParseNextLexem();
                ParseInstruction();
            }
        }

        /// <summary>
        /// Метод для разбора инструкции
        /// </summary>
        private static void ParseInstruction()
        {
            if (LexicalAnalyzer.CurrentLexem == Lexems.Name)
            {
                Identifier? x = NameTable.FindIdentifier(LexicalAnalyzer.CurrentName);
                if (x != null)
                {
                    ParseAssingInstruction(x.Value.type);
                }
                else
                {
                    Errors.AddError($"Не удалось найти переменную с именем {LexicalAnalyzer.CurrentName}. (Строка {Reader.LineNumber}, позиция {Reader.CharPositionInLine}, символ '{Reader.CurrentChar}')");
                }
            }
        }

        /// <summary>
        /// Разбор функции присваивания
        /// </summary>
        /// <param name="varType">Тип переменной</param>
        private static void ParseAssingInstruction(tType varType)
        {
            LexicalAnalyzer.ParseNextLexem();
            if (LexicalAnalyzer.CurrentLexem == Lexems.Assign)
            {
                LexicalAnalyzer.ParseNextLexem();
                tType t = ParseExpression();
                if (varType != t)
                {
                    Errors.AddError($"Несовместимые типы при присваивании.");
                }
            }
            else
            {
                Errors.AddError($"Не удалось распарсить инструкцию присваивания. (Строка {Reader.LineNumber}, позиция {Reader.CharPositionInLine}, символ '{Reader.CurrentChar}')");
            }
        }

        /// <summary>
        /// Разобрать выражение
        /// </summary>
        /// <returns>Тип переменной</returns>
        private static tType ParseExpression()
        { 
        
            while (LexicalAnalyzer.CurrentLexem != Lexems.Delimiter)
            {
                LexicalAnalyzer.ParseNextLexem();
            }
            return tType.Int;


        }

            /// <summary>
            /// Метод компиляции
            /// </summary>
        public static void Compile()
        {
            ParseDecVar();
            if (LexicalAnalyzer.CurrentLexem == Lexems.Begin)
            {
                LexicalAnalyzer.ParseNextLexem();
            }
            LexicalAnalyzer.ParseNextLexem();
            ParseSequenceOfInstructions();
            CheckLexem(Lexems.End);

        }
    }
}
