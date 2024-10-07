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
        /// Метод компиляции
        /// </summary>
        public static void Compile()
        {
            ParseDecVar();
        }
    }
}
