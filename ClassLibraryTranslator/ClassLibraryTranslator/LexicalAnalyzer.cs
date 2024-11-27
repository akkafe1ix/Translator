using System;
using System.Collections.Generic;

namespace ClassLibraryTranslator
{
    // Перечисление всех возможных типов лексем
    public enum Lexems
    {
        None,

        EOF,
        Delimiter,
        Comma,
        Assign,
        LeftBracket,
        RightBracket,

        Number,
        Name,
        Type,
        Int,

        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        Equal,
        NotEqual,

        Plus,
        Minus,
        Multiplication,
        Division,

        Begin,
        End,
        Print,

        If,
        Then,
        ElseIf,
        Else,
        EndIf,

        While,
        EndWhile,
        For,
        To,
        Do,
        Repeat,
        When,
        EndFor,

        Not,
        Or,
        Xor,
        And
    }

    public class LexicalAnalyzer
    {
        private readonly List<Keyword> _keywords;
        private Lexems _lexem;
        private string _name;
        private int _number;
        private readonly Reader _reader;

        public Lexems Lexem => _lexem;
        public string Name => _name;
        public int Number => _number;

        public LexicalAnalyzer(Reader reader)
        {
            _keywords = new List<Keyword>();
            _lexem = Lexems.None;
            _name = string.Empty;
            _number = 0;
            _reader = reader;

            InitializeKeywords();
        }

        private void InitializeKeywords()
        {
            AddKeyword("int", Lexems.Type);
            AddKeyword("begin", Lexems.Begin);
            AddKeyword("end", Lexems.End);
            AddKeyword("print", Lexems.Print);
            AddKeyword("if", Lexems.If);
            AddKeyword("then", Lexems.Then);
            AddKeyword("elseif", Lexems.ElseIf);
            AddKeyword("else", Lexems.Else);
            AddKeyword("endif", Lexems.EndIf);
            AddKeyword("while", Lexems.While);
            AddKeyword("endwhile", Lexems.EndWhile);
            AddKeyword("for", Lexems.For);
            AddKeyword("to", Lexems.To);
            AddKeyword("do", Lexems.Do);
            AddKeyword("repeat", Lexems.Repeat);
            AddKeyword("when", Lexems.When);
            AddKeyword("endfor", Lexems.EndFor);
            AddKeyword("not", Lexems.Not);
            AddKeyword("or", Lexems.Or);
            AddKeyword("xor", Lexems.Xor);
            AddKeyword("and", Lexems.And);
        }

        private void AddKeyword(string word, Lexems lexem)
        {
            _keywords.Add(new Keyword(word, lexem));
        }

        private Lexems GetKeyword(string word)
        {
            foreach (Keyword keyword in _keywords)
                if (keyword.Word == word)
                    return keyword.Lexem;

            return Lexems.Name;
        }

        public void ParseNextLexem()
        {
            if (!_reader.IsInitialized)
            {
                throw new Exception($"Ошибка! Объект {nameof(Reader)} не был инициализирован!");
            }

            if (_reader.EOF)
            {
                _lexem = Lexems.EOF;
                return;
            }

            while (_reader.Character == ' ')
            {
                _reader.ReadNextCharacter();
            }

            if (char.IsLetter(_reader.Character))
            {
                ParseName();
            }
            else if (char.IsDigit(_reader.Character))
            {
                ParseNumber();
            }
            else if (_reader.Character == '\n')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Delimiter;
            }
            else if (_reader.Character == ',')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Comma;
            }
            else if (_reader.Character == '(')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.LeftBracket;
            }
            else if (_reader.Character == ')')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.RightBracket;
            }
            else if (_reader.Character == '=')
            {
                _reader.ReadNextCharacter();
                if (_reader.Character == '=')
                {
                    _reader.ReadNextCharacter();
                    _lexem = Lexems.Equal;
                }
                else
                {
                    _lexem = Lexems.Assign;
                }
            }
            else if (_reader.Character == '!')
            {
                _reader.ReadNextCharacter();
                if (_reader.Character == '=')
                {
                    _reader.ReadNextCharacter();
                    _lexem = Lexems.NotEqual;
                }
                else
                {
                    throw new Exception("Ошибка! Недопустимый символ!");
                }
            }
            else if (_reader.Character == '<')
            {
                _reader.ReadNextCharacter();
                if (_reader.Character == '=')
                {
                    _reader.ReadNextCharacter();
                    _lexem = Lexems.LessOrEqual;
                }
                else
                {
                    _lexem = Lexems.Less;
                }
            }
            else if (_reader.Character == '>')
            {
                _reader.ReadNextCharacter();
                if (_reader.Character == '=')
                {
                    _reader.ReadNextCharacter();
                    _lexem = Lexems.GreaterOrEqual;
                }
                else
                {
                    _lexem = Lexems.Greater;
                }
            }
            else if (_reader.Character == '+')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Plus;
            }
            else if (_reader.Character == '-')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Minus;
            }
            else if (_reader.Character == '*')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Multiplication;
            }
            else if (_reader.Character == '/')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Division;
            }
            else
            {
                throw new Exception("Ошибка! Недопустимый символ!");
            }
        }

        private void ParseName()
        {
            string name = string.Empty;

            while (char.IsLetter(_reader.Character))
            {
                name += _reader.Character;
                _reader.ReadNextCharacter();
            }

            _name = name;
            _lexem = GetKeyword(name);
        }

        private void ParseNumber()
        {
            string number = string.Empty;

            while (char.IsDigit(_reader.Character))
            {
                number += _reader.Character;
                _reader.ReadNextCharacter();
            }

            if (!int.TryParse(number, out int result))
            {
                throw new Exception("Ошибка! Переполнение типа int!");
            }

            _number = result;
            _lexem = Lexems.Number;
        }
    }
}
