using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClassLibraryTranslator
{
    public static class SyntaxAnalyzer
    {

        private static string currentLabel = "";
        /// <summary>
        /// Проверка лексемы
        /// </summary>
        /// <param name="expectLexem">Ожидаемая лексема</param>
        public static void CheckLexem(Lexems expectLexem)
        {
            if (LexicalAnalyzer.Lexem != expectLexem)
                Errors.AddError($"Ожидалось {expectLexem}, а получили {LexicalAnalyzer.Lexem} (Строка:{Reader.LineNumber}, позиция:{Reader.PositionInLine}, символ:'{Convert.ToChar(Reader.Character)}') ");
            else
                LexicalAnalyzer.ParseNextLexem();
        }

        /// <summary>
        /// Метод проверки объявления переменных
        /// </summary>
        public static void ParseDecVar()
        {
            CheckLexem(Lexems.Type);
            if (LexicalAnalyzer.Lexem != Lexems.Name)
                Errors.AddError($"Ожидалось объявление переменной (Строка:{Reader.LineNumber}, позиция:{Reader.PositionInLine}, символ:'{Convert.ToChar(Reader.Character)}') ");
            else
            {
                Identifier? x = NameTable.AddIdentifier(LexicalAnalyzer.Name, tCat.Var, tType.Int);
                if(x==null)
                    Errors.AddError($"Переменная с именем {LexicalAnalyzer.Name} уже существет (Строка:{Reader.LineNumber}, позиция:{Reader.PositionInLine}, символ:'{Convert.ToChar(Reader.Character)}') ");
                LexicalAnalyzer.ParseNextLexem();
            }

            while (LexicalAnalyzer.Lexem == Lexems.Comma)
            {
                LexicalAnalyzer.ParseNextLexem();
                if (LexicalAnalyzer.Lexem != Lexems.Name)
                    Errors.AddError($"Ожидалось объявление переменной (Строка:{Reader.LineNumber}, позиция:{Reader.PositionInLine}, символ:'{Convert.ToChar(Reader.Character)}') ");
                else
                {
                    Identifier? x=NameTable.AddIdentifier(LexicalAnalyzer.Name, tCat.Var, tType.Int);
                    if (x == null)
                        Errors.AddError($"Переменная с именем {LexicalAnalyzer.Name} уже существет (Строка:{Reader.LineNumber}, позиция:{Reader.PositionInLine}, символ:'{Convert.ToChar(Reader.Character)}') ");
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
            while (LexicalAnalyzer.Lexem == Lexems.Delimiter)
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
            if (LexicalAnalyzer.Lexem == Lexems.Name)
            {
                Identifier? x = NameTable.FindIdentifier(LexicalAnalyzer.Name);
                if (x != null)
                {
                    ParseAssingInstruction(x.Value.type);
                    CodeGenerator.AddInstruction("pop ax");
                    CodeGenerator.AddInstruction("mov " + x.Value.name + ", ax");
                }
                else
                {
                    Errors.AddError($"Не удалось найти переменную с именем {LexicalAnalyzer.Name}. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
                }
            }
            else if (LexicalAnalyzer.Lexem == Lexems.Print)
            {
                ParsePrint();
            }
            else if (LexicalAnalyzer.Lexem == Lexems.If)
            {
                ParseConditionalStatement();
            }
            else if (LexicalAnalyzer.Lexem == Lexems.While)
            {
                ParseWhileLoop();
            }
        }

        /// <summary>
        /// Разбор функции присваивания
        /// </summary>
        /// <param name="varType">Тип переменной</param>
        private static void ParseAssingInstruction(tType varType)
        {
            LexicalAnalyzer.ParseNextLexem();
            if (LexicalAnalyzer.Lexem == Lexems.Assign)
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
                Errors.AddError($"Не удалось распарсить инструкцию присваивания. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
            }
        }

        /// <summary>
        /// Разобрать выражение
        /// </summary>
        /// <returns>Тип переменной</returns>
        private static tType ParseExpression()
        {
            tType t= ParseAdditionOrSubtraction();
            if(LexicalAnalyzer.Lexem == Lexems.Equal || 
                LexicalAnalyzer.Lexem == Lexems.NotEqual || 
                LexicalAnalyzer.Lexem == Lexems.Less ||
                LexicalAnalyzer.Lexem == Lexems.Greater ||
                LexicalAnalyzer.Lexem == Lexems.LessOrEqual ||
                LexicalAnalyzer.Lexem == Lexems.GreaterOrEqual)
            {
                string transition = "";
                switch (LexicalAnalyzer.Lexem)
                {
                    case Lexems.Equal:
                        transition = "jne";
                        break;
                    case Lexems.NotEqual:
                        transition = "je";
                        break;
                    case Lexems.Greater:
                        transition = "jle";
                        break;
                    case Lexems.GreaterOrEqual:
                        transition = "jl";
                        break;
                    case Lexems.Less:
                        transition = "jge";
                        break;
                    case Lexems.LessOrEqual:
                        transition = "jg";
                        break;
                }
                LexicalAnalyzer.ParseNextLexem();
                tType t2 = ParseAdditionOrSubtraction();
                // Проверка типов для операции сравнения (например, только сравнение числовых типов)
                if (t != t2 || t != tType.Int)
                {
                    Errors.AddError("Несовместимые типы для операции сравнения.");
                }
                CodeGenerator.AddInstruction("pop ax");
                CodeGenerator.AddInstruction("pop bx");
                CodeGenerator.AddInstruction("cmp bx, ax");
                CodeGenerator.AddInstruction(transition + " " + currentLabel);
                currentLabel = "";
                t = tType.Bool;
            }
            return t;
        }

        /// <summary>
        /// Разобрать сложение или вычитание
        /// </summary>
        /// <returns>тип</returns>
        private static tType ParseAdditionOrSubtraction()
        {
            tType t;
            Lexems op;
            if ((LexicalAnalyzer.Lexem == Lexems.Plus) || (LexicalAnalyzer.Lexem == Lexems.Minus))
            {
                op = LexicalAnalyzer.Lexem;
                LexicalAnalyzer.ParseNextLexem();
                t = ParseMultiplicationOrDivision();
            }
            else
                t = ParseMultiplicationOrDivision();
            if ((LexicalAnalyzer.Lexem == Lexems.Plus) || (LexicalAnalyzer.Lexem == Lexems.Minus))
            {
                do
                {
                    op = LexicalAnalyzer.Lexem;
                    LexicalAnalyzer.ParseNextLexem();
                    tType t2 = ParseMultiplicationOrDivision();
                    if (t != t2 || t != tType.Int)
                    {
                        Errors.AddError("Несовместимые типы для операции сложения/вычитания.");
                    }

                    switch (op)
                    {
                        case Lexems.Plus:
                            CodeGenerator.AddInstruction("pop bx");
                            CodeGenerator.AddInstruction("pop ax");
                            CodeGenerator.AddInstruction("add ax, bx");
                            CodeGenerator.AddInstruction("push ax");
                            break;
                        case Lexems.Minus:
                            CodeGenerator.AddInstruction("pop bx");
                            CodeGenerator.AddInstruction("pop ax");
                            CodeGenerator.AddInstruction("sub ax, bx");
                            CodeGenerator.AddInstruction("push ax");
                            break;
                    }
                } while ((LexicalAnalyzer.Lexem == Lexems.Plus) || (LexicalAnalyzer.Lexem == Lexems.Minus));


            }

            return t;
        }

        /// <summary>
        /// Разобрать Ветвление if
        /// </summary>
        private static void ParseConditionalStatement()
        {
            CheckLexem(Lexems.If);
            CodeGenerator.AddLabel();
            string lowLabel = CodeGenerator.ReturnCurrentLabel();
            currentLabel = lowLabel;
            CodeGenerator.AddLabel();
            string exitLabel = CodeGenerator.ReturnCurrentLabel();
            ParseExpression();
            CheckLexem(Lexems.Then);
            ParseSequenceOfInstructions();
            CodeGenerator.AddInstruction("jmp " + exitLabel);
            while (LexicalAnalyzer.Lexem == Lexems.ElseIf)
            {
                CodeGenerator.AddInstruction(lowLabel + ":");
                CodeGenerator.AddLabel();
                lowLabel = CodeGenerator.ReturnCurrentLabel();
                currentLabel = lowLabel;

                LexicalAnalyzer.ParseNextLexem();
                ParseExpression();
                CheckLexem(Lexems.Then);
                ParseSequenceOfInstructions();

                CodeGenerator.AddInstruction("jmp " + exitLabel);
            }
            if (LexicalAnalyzer.Lexem == Lexems.Else)
            {
                CodeGenerator.AddInstruction(lowLabel + ":");
                LexicalAnalyzer.ParseNextLexem();
                ParseSequenceOfInstructions();
            }
            else
            {
                CodeGenerator.AddInstruction(lowLabel + ":");
            }
            CheckLexem(Lexems.EndIf);
            CodeGenerator.AddInstruction(exitLabel + ":");
        }

        /// <summary>
        /// Разобрать цикл while
        /// </summary>
        private static void ParseWhileLoop()
        {
            CheckLexem(Lexems.While);
            CodeGenerator.AddLabel();
            string upLabel = CodeGenerator.ReturnCurrentLabel();
            CodeGenerator.AddLabel();
            string lowLabel = CodeGenerator.ReturnCurrentLabel();
            currentLabel = lowLabel;
            CodeGenerator.AddInstruction(upLabel + ":");
            ParseExpression();
            ParseSequenceOfInstructions();
            CheckLexem(Lexems.EndWhile);
            CodeGenerator.AddInstruction("jmp " + upLabel);
            CodeGenerator.AddInstruction(lowLabel + ":");
        }


            /// <summary>
            /// Разобрать умножение или деление
            /// </summary>
            /// <returns>тип</returns>
            private static tType ParseMultiplicationOrDivision()
        {
            tType t= ParseSubexpression();
            Lexems op;
            if ((LexicalAnalyzer.Lexem == Lexems.Multiplication) || (LexicalAnalyzer.Lexem == Lexems.Division))
            {
                do
                {
                    op = LexicalAnalyzer.Lexem;
                    LexicalAnalyzer.ParseNextLexem();
                    tType t2 = ParseMultiplicationOrDivision();
                    if (t != t2 || t != tType.Int)
                    {
                        Errors.AddError("Несовместимые типы для операции сложения/вычитания.");
                    }

                    switch (op)
                    {
                        case Lexems.Multiplication:
                            CodeGenerator.AddInstruction("pop bx");
                            CodeGenerator.AddInstruction("pop ax");
                            CodeGenerator.AddInstruction("mul bx");
                            CodeGenerator.AddInstruction("push ax");
                            break;
                        case Lexems.Division:
                            CodeGenerator.AddInstruction("pop bx");
                            CodeGenerator.AddInstruction("pop ax");
                            CodeGenerator.AddInstruction("cwd");
                            CodeGenerator.AddInstruction("div bl");
                            CodeGenerator.AddInstruction("push ax");
                            break;
                    }
                } while ((LexicalAnalyzer.Lexem == Lexems.Multiplication) || (LexicalAnalyzer.Lexem == Lexems.Division));
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
            if (LexicalAnalyzer.Lexem == Lexems.Name)
            {
                x = NameTable.FindIdentifier(LexicalAnalyzer.Name);
                if ((x != null) && (x.Value.category == tCat.Var))
                {
                    CodeGenerator.AddInstruction("mov ax, " + LexicalAnalyzer.Name);
                    CodeGenerator.AddInstruction("push ax");
                    LexicalAnalyzer.ParseNextLexem();
                    return x.Value.type;
                }
                else
                {
                    Errors.AddError($"Не удалось найти переменную с именем {LexicalAnalyzer.Name}. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
                }
            }
            else if (LexicalAnalyzer.Lexem == Lexems.Number)
            {
                CodeGenerator.AddInstruction("mov ax, " + LexicalAnalyzer.Number);
                CodeGenerator.AddInstruction("push ax");
                LexicalAnalyzer.ParseNextLexem();
                return tType.Int;
            }
            else if (LexicalAnalyzer.Lexem == Lexems.LeftBracket)
            {
                LexicalAnalyzer.ParseNextLexem();
                t = ParseExpression();
                CheckLexem(Lexems.RightBracket);
            }
            else
            {
                Errors.AddError($"Недопустимое выражение. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
            }
            return t;

        }

        private static void ParsePrint()
        {
            CheckLexem(Lexems.Print);
            if (LexicalAnalyzer.Lexem == Lexems.Name)
            {
                Identifier? x = NameTable.FindIdentifier(LexicalAnalyzer.Name);
                CodeGenerator.AddInstruction("push ax");
                CodeGenerator.AddInstruction("mov ax, " + LexicalAnalyzer.Name);
                CodeGenerator.AddInstruction("CALL PRINT");
                CodeGenerator.AddInstruction("pop ax");
                LexicalAnalyzer.ParseNextLexem();
            }
            else
            {
                Errors.AddError($"Не удалось распарсить выражение вывода. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
            }
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
            if (LexicalAnalyzer.Lexem == Lexems.Begin)
            {
                LexicalAnalyzer.ParseNextLexem();
            }
            LexicalAnalyzer.ParseNextLexem();
            ParseSequenceOfInstructions();
            CheckLexem(Lexems.End);
            CodeGenerator.DeclareMainProcedureCompletion();
            CodeGenerator.DeclarePrint();
            CodeGenerator.DeclareCodeCompletion();
            
        }
    }
}
