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
        LeftBracketF,
        RightBracketF,
        Semi,
        Colon,

        Number,
        Name,
        Type,
        Int,
        Bool,
        Var,

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
        And,
        True,
        False
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
            AddKeyword("Integer", Lexems.Int);
            AddKeyword("Boolean", Lexems.Bool);
            AddKeyword("Var", Lexems.Var);
            AddKeyword("Begin", Lexems.Begin);
            AddKeyword("End", Lexems.End);
            AddKeyword("Print", Lexems.Print);
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
            AddKeyword("NOT", Lexems.Not);
            AddKeyword("OR", Lexems.Or);
            AddKeyword("XOR", Lexems.Xor);
            AddKeyword("AND", Lexems.And);
            AddKeyword("True", Lexems.True);
            AddKeyword("False", Lexems.False);

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
            else if (_reader.Character == ';')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Semi;
            }
            else if (_reader.Character == ':')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Colon;
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
            else if (_reader.Character == '{')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.LeftBracketF;
            }
            else if (_reader.Character == '}')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.RightBracketF;
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
            else if (_reader.Character == '&')
            {
                _reader.ReadNextCharacter();
                if (_reader.Character == '&')
                {
                    _reader.ReadNextCharacter();
                    _lexem = Lexems.And;
                }
                else
                {
                    throw new Exception("Invalid symbol '&'");
                }
            }
            else if (_reader.Character == '|')
            {
                _reader.ReadNextCharacter();
                if (_reader.Character == '|')
                {
                    _reader.ReadNextCharacter();
                    _lexem = Lexems.Or;
                }
                else
                {
                    throw new Exception("Invalid symbol '|'");
                }
            }
            else if (_reader.Character == '.')
            {

                _reader.ReadNextCharacter();
                if (_reader.Character == 'N')
                {
                    _reader.ReadNextCharacter();
                    if (_reader.Character == 'O')
                    {
                        _reader.ReadNextCharacter();
                        if (_reader.Character == 'T')
                        {
                            _reader.ReadNextCharacter();
                            if (_reader.Character == '.')
                            {
                                _lexem = Lexems.Not;
                            }
                        }
                    }
                }
                if (_reader.Character == 'X')
                {
                    _reader.ReadNextCharacter();
                    if (_reader.Character == 'O')
                    {
                        _reader.ReadNextCharacter();
                        if (_reader.Character == 'R')
                        {
                            _reader.ReadNextCharacter();
                            if (_reader.Character == '.')
                            {
                                _lexem = Lexems.Xor;
                            }
                        }
                    }
                }
                if (_reader.Character == 'A')
                {
                    _reader.ReadNextCharacter();
                    if (_reader.Character == 'N')
                    {
                        _reader.ReadNextCharacter();
                        if (_reader.Character == 'D')
                        {
                            _reader.ReadNextCharacter();
                            if (_reader.Character == '.')
                            {
                                _lexem = Lexems.And;
                            }
                        }
                    }
                }

                if (_reader.Character == 'O')
                {
                    _reader.ReadNextCharacter();
                    if (_reader.Character == 'R')
                    {
                        _reader.ReadNextCharacter();
                        if (_reader.Character == '.')
                        {
                            _lexem = Lexems.Or;
                        }
                    }
                }

            }
            else if (_reader.Character == '^')
            {
                _reader.ReadNextCharacter();
                _lexem = Lexems.Xor;
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
