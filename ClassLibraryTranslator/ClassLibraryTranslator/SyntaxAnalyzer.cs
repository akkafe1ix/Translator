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
                Identifier? x = NameTable.AddIdentifier(LexicalAnalyzer.CurrentName, tCat.Var, tType.Int);
                if(x==null)
                    Errors.AddError($"Переменная с именем {LexicalAnalyzer.CurrentName} уже существет (Строка:{Reader.LineNumber}, позиция:{Reader.CharPositionInLine}, символ:'{Convert.ToChar(Reader.CurrentChar)}') ");
                LexicalAnalyzer.ParseNextLexem();
            }

            while (LexicalAnalyzer.CurrentLexem == Lexems.Comma)
            {
                LexicalAnalyzer.ParseNextLexem();
                if (LexicalAnalyzer.CurrentLexem != Lexems.Name)
                    Errors.AddError($"Ожидалось объявление переменной (Строка:{Reader.LineNumber}, позиция:{Reader.CharPositionInLine}, символ:'{Convert.ToChar(Reader.CurrentChar)}') ");
                else
                {
                    Identifier? x=NameTable.AddIdentifier(LexicalAnalyzer.CurrentName, tCat.Var, tType.Int);
                    if (x == null)
                        Errors.AddError($"Переменная с именем {LexicalAnalyzer.CurrentName} уже существет (Строка:{Reader.LineNumber}, позиция:{Reader.CharPositionInLine}, символ:'{Convert.ToChar(Reader.CurrentChar)}') ");
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
            return ParseAdditionOrSubtraction();
        }

        /// <summary>
        /// Разобрать сложение или вычитание
        /// </summary>
        /// <returns>тип</returns>
        private static tType ParseAdditionOrSubtraction()
        {
            tType t;
            Lexems op;
            if ((LexicalAnalyzer.CurrentLexem == Lexems.Plus) || (LexicalAnalyzer.CurrentLexem == Lexems.Minus))
            {
                op = LexicalAnalyzer.CurrentLexem;
                LexicalAnalyzer.ParseNextLexem();
                t = ParseMultiplicationOrDivision();
            }
            else
                t = ParseMultiplicationOrDivision();
            if ((LexicalAnalyzer.CurrentLexem == Lexems.Plus) || (LexicalAnalyzer.CurrentLexem == Lexems.Minus))
            {
                do
                {
                    op = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.ParseNextLexem();
                    tType t2 = ParseMultiplicationOrDivision();
                    if (t != t2 || t != tType.Int)
                    {
                        Errors.AddError("Несовместимые типы для операции сложения/вычитания.");
                    }

                    switch (op)
                    {
                        case Lexems.Plus:
                            break;
                        case Lexems.Minus:
                            break;
                    }
                } while ((LexicalAnalyzer.CurrentLexem == Lexems.Plus) || (LexicalAnalyzer.CurrentLexem == Lexems.Minus));


            }

            return t;
        }

        /// <summary>
        /// Разобрать умножение или деление
        /// </summary>
        /// <returns>тип</returns>
        private static tType ParseMultiplicationOrDivision()
        {
            tType t= ParseSubexpression();
            Lexems op;
            if ((LexicalAnalyzer.CurrentLexem == Lexems.Multiplication) || (LexicalAnalyzer.CurrentLexem == Lexems.Division))
            {
                do
                {
                    op = LexicalAnalyzer.CurrentLexem;
                    LexicalAnalyzer.ParseNextLexem();
                    tType t2 = ParseMultiplicationOrDivision();
                    if (t != t2 || t != tType.Int)
                    {
                        Errors.AddError("Несовместимые типы для операции сложения/вычитания.");
                    }

                    switch (op)
                    {
                        case Lexems.Multiplication:
                            break;
                        case Lexems.Division:
                            break;
                    }
                } while ((LexicalAnalyzer.CurrentLexem == Lexems.Multiplication) || (LexicalAnalyzer.CurrentLexem == Lexems.Division));
            }
            return t;
        }


        /// <summary>
        /// Разобрать подвыражение
        /// </summary>
        /// <returns>тип</returns>
        private static tType ParseSubexpression()
        {
            Identifier? x;
            tType t = tType.None;
            if (LexicalAnalyzer.CurrentLexem == Lexems.Name)
            {
                x = NameTable.FindIdentifier(LexicalAnalyzer.CurrentName);
                if ((x != null) && (x.Value.category == tCat.Var))
                {
                    LexicalAnalyzer.ParseNextLexem();
                    return x.Value.type;
                }
                else
                {
                    Errors.AddError($"Не удалось найти переменную с именем {LexicalAnalyzer.CurrentName}. (Строка {Reader.LineNumber}, позиция {Reader.CharPositionInLine}, символ '{Reader.CurrentChar}')");
                }
            }
            else if (LexicalAnalyzer.CurrentLexem == Lexems.Number)
            {
                LexicalAnalyzer.ParseNextLexem();
                return tType.Int;
            }
            else if (LexicalAnalyzer.CurrentLexem == Lexems.LeftBracket)
            {
                LexicalAnalyzer.ParseNextLexem();
                t = ParseExpression();
                CheckLexem(Lexems.RightBracket);
            }
            else
            {
                Errors.AddError($"Недопустимое выражение. (Строка {Reader.LineNumber}, позиция {Reader.CharPositionInLine}, символ '{Reader.CurrentChar}')");
            }
            return t;

        }

            /// <summary>
            /// Метод компиляции
            /// </summary>
        public static void Compile()
        {
            CodeGenerator.DeclareDataSegment();
            ParseDecVar();
            CodeGenerator.DeclareVariables();
            CodeGenerator.DeclareStackAndCodeSegments();
            CheckLexem(Lexems.Delimiter);
            if (LexicalAnalyzer.CurrentLexem == Lexems.Begin)
            {
                LexicalAnalyzer.ParseNextLexem();
            }
            LexicalAnalyzer.ParseNextLexem();
            ParseSequenceOfInstructions();
            CheckLexem(Lexems.End);
            CodeGenerator.DeclareMainProcedureCompletion();
            CodeGenerator.DeclareCodeCompletion();
            
        }
    }
}
