using System;
using System.Collections.Generic;

namespace ClassLibraryTranslator
{
    public class SyntaxAnalyzer
    {
        public readonly LexicalAnalyzer _lexicalAnalyzer;
        public readonly CodeGenerator _codeGenerator;
        public readonly NameTable _nameTable;
        public readonly Errors _errors;
        public readonly Reader _reader;
        public string _currentLabel = "";
        public string _checkbool = "";

        public SyntaxAnalyzer(LexicalAnalyzer lexicalAnalyzer, CodeGenerator codeGenerator, NameTable nameTable, Errors errors, Reader reader)
        {
            _lexicalAnalyzer = lexicalAnalyzer;
            _codeGenerator = codeGenerator;
            _nameTable = nameTable;
            _errors = errors;
            _reader = reader;
        }

        public void CheckLexem(Lexems expectLexem)
        {
            if (_lexicalAnalyzer.Lexem != expectLexem)
            {
                _errors.AddError(
                    $"Ожидалось {expectLexem}, а получили {_lexicalAnalyzer.Lexem} " +
                    $"(Строка:{_reader.LineNumber}, позиция:{_reader.PositionInLine}, " +
                    $"символ:'{Convert.ToChar(_reader.Character)}')");
            }
            else
            {
                _lexicalAnalyzer.ParseNextLexem();
            }
        }

        public void ParseDecVar()
        {
            // Проверяем ключевое слово Var
            CheckLexem(Lexems.Var);

            // Обрабатываем список переменных, разделенных запятыми
            List <string> variableNames = new List <string>();
            while (_lexicalAnalyzer.Lexem == Lexems.Name)
            {
                variableNames.Add(_lexicalAnalyzer.Name);
                _lexicalAnalyzer.ParseNextLexem();

                if (_lexicalAnalyzer.Lexem == Lexems.Comma)
                {
                    _lexicalAnalyzer.ParseNextLexem(); // Пропускаем запятую
                }
                else
                {
                    break; // Заканчиваем список переменных
                }
            }

            // Проверяем наличие двоеточия
            CheckLexem(Lexems.Colon);

            // Определяем тип переменных
            tType varType = tType.None;
            if (_lexicalAnalyzer.Lexem == Lexems.Bool)
            {
                varType = tType.Bool;
                _lexicalAnalyzer.ParseNextLexem();
            }
            else if (_lexicalAnalyzer.Lexem == Lexems.Int)
            {
                varType = tType.Int;
                _lexicalAnalyzer.ParseNextLexem();
            }
            else
            {
                _errors.AddError($"Ожидался тип переменных (Boolean или Int), а получили {_lexicalAnalyzer.Lexem}");
            }

            // Добавляем переменные в таблицу имен
            foreach (var variableName in variableNames)
            {
                var x = _nameTable.AddIdentifier(variableName, tCat.Var, varType);
                if (x == null)
                {
                    _errors.AddError($"Переменная с именем {variableName} уже существует " +
                        $"(Строка:{_reader.LineNumber}, позиция:{_reader.PositionInLine})");
                }
            }

            CheckLexem(Lexems.Semi);
            // Проверяем на конец строки или следующий оператор
            CheckLexem(Lexems.Delimiter);

            // Рекурсивно вызываем метод, если обнаружено новое объявление
            if (_lexicalAnalyzer.Lexem == Lexems.Var)
            {
                ParseDecVar();
            }
        }


        public void ParseSequenceOfInstructions()
        {
            ParseInstruction();
            while (_lexicalAnalyzer.Lexem == Lexems.Delimiter)
            {
                _lexicalAnalyzer.ParseNextLexem();
                ParseInstruction();
            }
        }

        public void ParseInstruction()
        {
            if (_lexicalAnalyzer.Lexem == Lexems.Name)
            {
                var x = _nameTable.FindIdentifier(_lexicalAnalyzer.Name);
                if (x != null)
                {
                    ParseAssignInstruction(x.Value.type);
                    CheckLexem(Lexems.Semi);
                    _codeGenerator.AddInstruction("pop ax");
                    _codeGenerator.AddInstruction("mov " + x.Value.name + ", ax");
                }
                else
                {
                    _errors.AddError(
                        $"Не удалось найти переменную с именем {_lexicalAnalyzer.Name}. " +
                        $"(Строка {_reader.LineNumber}, позиция {_reader.PositionInLine}, " +
                        $"символ '{_reader.Character}')");
                }
            }
            else if (_lexicalAnalyzer.Lexem == Lexems.Print)
            {
                ParsePrint();
            }
            else if (_lexicalAnalyzer.Lexem == Lexems.If)
            {
                ParseConditionalStatement();
            }
            else if (_lexicalAnalyzer.Lexem == Lexems.While)
            {
                ParseWhileLoop();
            }
        }

        public void ParseAssignInstruction(tType varType)
        {
            _lexicalAnalyzer.ParseNextLexem();
            if (_lexicalAnalyzer.Lexem == Lexems.Assign)
            {
                _lexicalAnalyzer.ParseNextLexem();


                tType t = ParseExpressionLogic();

                if (_checkbool == "0" || _checkbool == "1")
                {
                    if (varType == tType.Int)
                        t = tType.Int;
                    if (varType == tType.Bool)
                        t = tType.Bool;
                }
                
                if (varType != t)
                {
                    _errors.AddError($"Несовместимые типы при присваивании.");
                }
               
            }
            else
            {
                _errors.AddError($"Не удалось распарсить инструкцию присваивания. (Строка {_reader.LineNumber}, позиция {_reader.PositionInLine}, символ '{_reader.Character}')");
            }
        }




        public tType ParseExpression()
        {
            tType t = ParseAdditionOrSubtraction();
            if (_lexicalAnalyzer.Lexem == Lexems.Equal ||
                _lexicalAnalyzer.Lexem == Lexems.NotEqual ||
                _lexicalAnalyzer.Lexem == Lexems.Less ||
                _lexicalAnalyzer.Lexem == Lexems.Greater ||
                _lexicalAnalyzer.Lexem == Lexems.LessOrEqual ||
                _lexicalAnalyzer.Lexem == Lexems.GreaterOrEqual)
            {
                string transition = "";
                var oper = _lexicalAnalyzer.Lexem;
                if (t == tType.Bool)
                {
                    if (oper == Lexems.Equal || oper == Lexems.NotEqual)
                    {
                        _lexicalAnalyzer.ParseNextLexem();
                        var type = _lexicalAnalyzer.Lexem;
                        if ((type == Lexems.False || type == Lexems.True))
                        {
                            _codeGenerator.AddInstruction("pop ax"); // Правый операнд
                                                                     //_codeGenerator.AddInstruction("pop bx"); // Левый операнд
                            if (type == Lexems.True)
                            {
                                _codeGenerator.AddInstruction("cmp ax, 1");
                            }
                            else
                            {
                                _codeGenerator.AddInstruction("cmp ax, 0");
                            }
                            if (oper == Lexems.Equal)
                            {
                                _codeGenerator.AddInstruction($"jne {_currentLabel}");
                                _currentLabel = "";
                            }
                            else
                            {
                                _codeGenerator.AddInstruction($"je {_currentLabel}");
                                _currentLabel = "";
                            }
                            _lexicalAnalyzer.ParseNextLexem();

                        }
                    }

                }
                else
                {
                    switch (_lexicalAnalyzer.Lexem)
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

                    _lexicalAnalyzer.ParseNextLexem();
                    tType t2 = ParseAdditionOrSubtraction();
                    if (t != t2)
                    {
                        _errors.AddError("Несовместимые типы для операции сравнения.");
                    }

                    _codeGenerator.AddInstruction("pop ax");
                    _codeGenerator.AddInstruction("pop bx");
                    _codeGenerator.AddInstruction("cmp bx, ax");
                    _codeGenerator.AddInstruction(transition + " " + _currentLabel);
                    _currentLabel = "";     
                }
                 t = tType.Bool;
            }

            return t;
        }
        private tType ParseSubexpression()
        {
            Identifier? x;
            tType t = tType.None;

            if (_lexicalAnalyzer.Lexem == Lexems.Name)
            {
                x = _nameTable.FindIdentifier(_lexicalAnalyzer.Name);
                if (x != null && x.Value.category == tCat.Var)
                {
                    _codeGenerator.AddInstruction("mov ax, " + _lexicalAnalyzer.Name);
                    _codeGenerator.AddInstruction("push ax");
                    _lexicalAnalyzer.ParseNextLexem();
                    return x.Value.type;
                }
                else
                {
                    _errors.AddError($"Не удалось найти переменную с именем {_lexicalAnalyzer.Name}. (Строка {_reader.LineNumber}, позиция {_reader.PositionInLine}, символ '{_reader.Character}')");
                }
            }
            else if (_lexicalAnalyzer.Lexem == Lexems.Number)
            {
                _codeGenerator.AddInstruction("mov ax, " + _lexicalAnalyzer.Number);
                _codeGenerator.AddInstruction("push ax");
                _checkbool = Convert.ToString(_lexicalAnalyzer.Number);
                var cur = _reader.Character;
                _lexicalAnalyzer.ParseNextLexem();
                return tType.Int;
            }
            else if (_lexicalAnalyzer.Lexem == Lexems.True)
            {
                _codeGenerator.AddInstruction("mov ax, 1");
                _codeGenerator.AddInstruction("push ax");
                _checkbool = "1";
                _lexicalAnalyzer.ParseNextLexem();
                return tType.Bool;
            }
            else if (_lexicalAnalyzer.Lexem == Lexems.False)
            {
                _codeGenerator.AddInstruction("mov ax, 0");
                _codeGenerator.AddInstruction("push ax");
                _checkbool = "0";
                _lexicalAnalyzer.ParseNextLexem();
                return tType.Bool;
            }
            else if (_lexicalAnalyzer.Lexem == Lexems.LeftBracket)
            {
                _lexicalAnalyzer.ParseNextLexem();
                t = ParseExpressionLogic();
                CheckLexem(Lexems.RightBracket);
            }
            else
            {
                _errors.AddError($"Недопустимое выражение. (Строка {_reader.LineNumber}, позиция {_reader.PositionInLine}, символ '{_reader.Character}')");
            }

            return t;
        }

        private tType ParseMultiplicationOrDivision()
        {
            tType t = ParseSubexpression();
            Lexems op;

            while (_lexicalAnalyzer.Lexem == Lexems.Multiplication || _lexicalAnalyzer.Lexem == Lexems.Division)
            {
                op = _lexicalAnalyzer.Lexem;
                _lexicalAnalyzer.ParseNextLexem();
                tType t2 = ParseSubexpression();

                if (t != t2 || t != tType.Int)
                {
                    _errors.AddError("Несовместимые типы для операции умножения/деления.");
                }

                _codeGenerator.AddInstruction("pop bx");
                _codeGenerator.AddInstruction("pop ax");

                switch (op)
                {
                    case Lexems.Multiplication:
                        _codeGenerator.AddInstruction("mul bx");
                        break;
                    case Lexems.Division:
                        _codeGenerator.AddInstruction("cwd");
                        _codeGenerator.AddInstruction("div bx");
                        break;
                }

                _codeGenerator.AddInstruction("push ax");
            }

            return t;
        }

        private tType ParseAdditionOrSubtraction()
        {
            tType t = ParseMultiplicationOrDivision();
            while (_lexicalAnalyzer.Lexem == Lexems.Plus || _lexicalAnalyzer.Lexem == Lexems.Minus)
            {
                var op = _lexicalAnalyzer.Lexem;
                _lexicalAnalyzer.ParseNextLexem();
                tType t2 = ParseMultiplicationOrDivision();
                if (t != t2 || t != tType.Int)
                {
                    _errors.AddError("Несовместимые типы для операции сложения/вычитания.");
                }

                _codeGenerator.AddInstruction("pop bx");
                _codeGenerator.AddInstruction("pop ax");

                switch (op)
                {
                    case Lexems.Plus:
                        _codeGenerator.AddInstruction("add ax, bx");
                        break;
                    case Lexems.Minus:
                        _codeGenerator.AddInstruction("sub ax, bx");
                        break;
                }

                _codeGenerator.AddInstruction("push ax");
            }

            return t;
        }

        public void ParseWhileLoop()
        {
            CheckLexem(Lexems.While);  

            _codeGenerator.AddLabel();  
            string upLabel = _codeGenerator.ReturnCurrentLabel();  
            _codeGenerator.AddLabel();  
            string lowLabel = _codeGenerator.ReturnCurrentLabel();  
            _currentLabel = lowLabel;  

            _codeGenerator.AddInstruction(upLabel + ":");
            
            ParseExpressionLogic();  

             _lexicalAnalyzer.ParseNextLexem();
            
            if (_lexicalAnalyzer.Lexem == Lexems.LeftBracketF)
            {
                _lexicalAnalyzer.ParseNextLexem();
                ParseSequenceOfInstructions();  
                CheckLexem(Lexems.RightBracketF);  

            }
            else
            {
                
                ParseInstruction();  
            }
            //_lexicalAnalyzer.ParseNextLexem();

            _codeGenerator.AddInstruction("jmp " + upLabel);  
            _codeGenerator.AddInstruction(lowLabel + ":");  
            //CheckLexem(Lexems.EndWhile);  
        }



        public void ParseConditionalStatement()
        {
            CheckLexem(Lexems.If);
            _codeGenerator.AddLabel();
            string lowLabel = _codeGenerator.ReturnCurrentLabel();
            _currentLabel = lowLabel;
            _codeGenerator.AddLabel();
            string exitLabel = _codeGenerator.ReturnCurrentLabel();

            ParseExpressionLogic();
            CheckLexem(Lexems.Then);
            ParseSequenceOfInstructions();
            _codeGenerator.AddInstruction("jmp " + exitLabel);

            while (_lexicalAnalyzer.Lexem == Lexems.ElseIf)
            {
                _codeGenerator.AddInstruction(lowLabel + ":");
                _codeGenerator.AddLabel();
                lowLabel = _codeGenerator.ReturnCurrentLabel();
                _currentLabel = lowLabel;

                _lexicalAnalyzer.ParseNextLexem();
                ParseExpressionLogic();
                CheckLexem(Lexems.Then);
                ParseSequenceOfInstructions();
                _codeGenerator.AddInstruction("jmp " + exitLabel);
            }

            if (_lexicalAnalyzer.Lexem == Lexems.Else)
            {
                _codeGenerator.AddInstruction(lowLabel + ":");
                _lexicalAnalyzer.ParseNextLexem();
                ParseSequenceOfInstructions();
            }
            else
            {
                _codeGenerator.AddInstruction(lowLabel + ":");
            }

            CheckLexem(Lexems.EndIf);
            _codeGenerator.AddInstruction(exitLabel + ":");
        }

        public void ParsePrint()
        {
            CheckLexem(Lexems.Print);
            if (_lexicalAnalyzer.Lexem == Lexems.Name)
            {
                var x = _nameTable.FindIdentifier(_lexicalAnalyzer.Name);
                if (x != null)
                {
                    _codeGenerator.AddInstruction("push ax");
                    _codeGenerator.AddInstruction("mov ax, " + _lexicalAnalyzer.Name);
                    _codeGenerator.AddInstruction("CALL PRINT");
                    _codeGenerator.AddInstruction("pop ax");
                    _lexicalAnalyzer.ParseNextLexem();
                }
                else
                {
                    _errors.AddError(
                        $"Не удалось найти переменную с именем {_lexicalAnalyzer.Name}. (Строка {_reader.LineNumber}, позиция {_reader.PositionInLine}, символ '{_reader.Character}')");
                }
            }
            else
            {
                _errors.AddError(
                    $"Не удалось распарсить выражение вывода. (Строка {_reader.LineNumber}, позиция {_reader.PositionInLine}, символ '{_reader.Character}')");
            }
        }
        private tType ParseExpressionLogic()
        {
            return ParseLogicalOr();
        }


        private tType ParseLogicalOr()
        {
            tType type;

            if (_lexicalAnalyzer.Lexem == Lexems.Not)
            {
                _lexicalAnalyzer.ParseNextLexem();
                type = ParseLogicalAnd();

                if (type != tType.Bool)
                {
                    _errors.AddError("Операция NOT применима только к логическим типам.");
                }

                // Генерация кода для операции NOT
                _codeGenerator.AddInstruction("pop ax"); // Извлекаем значение из стека
                _codeGenerator.AddInstruction("not ax"); // Инвертируем значение
                _codeGenerator.AddInstruction("and ax, 1"); // Ограничиваем результат до булевого значения (0 или 1)
                _codeGenerator.AddInstruction("push ax"); // Возвращаем результат в стек
            }
            else
            {
                type = ParseLogicalAnd();
            }

            while (_lexicalAnalyzer.Lexem == Lexems.Or)
            {
                // Генерация новой метки для перехода
                _codeGenerator.AddLabel();
                string exitLabel = _codeGenerator.ReturnCurrentLabel();

                _lexicalAnalyzer.ParseNextLexem();
                tType rightType = ParseLogicalAnd();
                if (type != rightType || type != tType.Bool)
                {
                    _errors.AddError("Несовместимые типы для логической операции ИЛИ.");
                }

                // Генерация кода для операции OR
                _codeGenerator.AddInstruction("pop ax"); // Правый операнд
                _codeGenerator.AddInstruction("pop bx"); // Левый операнд
                _codeGenerator.AddInstruction("or ax, bx");
                _codeGenerator.AddInstruction("push ax");
                _codeGenerator.AddInstruction("cmp ax, 0");
                _codeGenerator.AddInstruction($"jnz {exitLabel}"); // Если результат не ноль, пропускаем jump
                _codeGenerator.AddInstruction($"jmp {_currentLabel}"); // Если результат ноль, переходим на метку условия
                _codeGenerator.AddInstruction(exitLabel + ":");

                type = tType.Bool;
            }

            while (_lexicalAnalyzer.Lexem == Lexems.Xor)
            {
                // Генерация новой метки для перехода
                _codeGenerator.AddLabel();
                string exitLabel = _codeGenerator.ReturnCurrentLabel();

                _lexicalAnalyzer.ParseNextLexem();
                tType rightType = ParseLogicalAnd();
                if (type != rightType || type != tType.Bool)
                {
                    _errors.AddError("Несовместимые типы для логической операции XOR.");
                }

                // Генерация кода для операции XOR
                _codeGenerator.AddInstruction("pop ax"); // Правый операнд
                _codeGenerator.AddInstruction("pop bx"); // Левый операнд
                _codeGenerator.AddInstruction("xor ax, bx");
                _codeGenerator.AddInstruction("push ax");
                _codeGenerator.AddInstruction("cmp ax, 1");
                _codeGenerator.AddInstruction($"jnz {_currentLabel}"); // Если результат не ноль, пропускаем jump
                _codeGenerator.AddInstruction($"jmp {exitLabel}"); // Если результат ноль, переходим на метку условия
                _codeGenerator.AddInstruction(exitLabel + ":");

                type = tType.Bool;
            }

            return type;
        }


        private tType ParseLogicalAnd()
        {
            tType type;

            if (_lexicalAnalyzer.Lexem == Lexems.Not)
            {
                _lexicalAnalyzer.ParseNextLexem();
                type = ParseExpression();

                if (type != tType.Bool)
                {
                    _errors.AddError("Операция NOT применима только к логическим типам.");
                }

                // Генерация кода для операции NOT
                _codeGenerator.AddInstruction("pop ax"); // Извлекаем значение из стека
                _codeGenerator.AddInstruction("not ax"); // Инвертируем значение
                _codeGenerator.AddInstruction("and ax, 1"); // Ограничиваем результат до булевого значения (0 или 1)
                _codeGenerator.AddInstruction("push ax"); // Возвращаем результат в стек
            }
            else
            {
                type = ParseExpression();
            }

            while (_lexicalAnalyzer.Lexem == Lexems.And)
            {
                _lexicalAnalyzer.ParseNextLexem();

                // Сохраняем текущую метку, так как ParseComparison может её изменить
                string savedLabel = _currentLabel;

                tType rightType = ParseExpression();
                if (type != rightType || type != tType.Bool)
                {
                    _errors.AddError("Несовместимые типы для логической операции И.");
                }

                // Генерация кода для операции AND  
                _codeGenerator.AddInstruction("pop ax"); // Правый операнд
                _codeGenerator.AddInstruction("pop bx"); // Левый операнд
                _codeGenerator.AddInstruction("and ax, bx");
                _codeGenerator.AddInstruction("push ax");
                _codeGenerator.AddInstruction("cmp ax, 0");
                _codeGenerator.AddInstruction($"jz {savedLabel}"); // Если результат ноль, переходим на метку условия

                type = tType.Bool;
            }
            return type;
        }



        public void Compile()
        {
            _lexicalAnalyzer.ParseNextLexem();
            _codeGenerator.DeclareDataSegment();
            ParseDecVar();
            _codeGenerator.DeclareVariables();
            _codeGenerator.DeclareStackAndCodeSegments();
            if (_lexicalAnalyzer.Lexem == Lexems.Begin)
            {
                _lexicalAnalyzer.ParseNextLexem();
            }

            ParseSequenceOfInstructions();
            CheckLexem(Lexems.End);
            _codeGenerator.DeclareMainProcedureCompletion();
            _codeGenerator.DeclarePrint();
            _codeGenerator.DeclareCodeCompletion();
        }
    }

}
