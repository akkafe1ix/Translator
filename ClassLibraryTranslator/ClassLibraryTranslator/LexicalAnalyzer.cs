using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryTranslator
{
	//Перечисление всех возможных типов лексем
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


	public static class LexicalAnalyzer
    {
		private static List<Keyword> _keywords;
		private static Lexems _lexem;
		private static string _name;
		private static int _number;

		static LexicalAnalyzer()
		{
			_keywords = new List<Keyword>();
			_lexem = Lexems.None;
			_name = string.Empty;
			_number = 0;

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

			ParseNextLexem();
		}

		public static Lexems Lexem => _lexem;
		public static string Name => _name;
		public static int Number => _number;

		private static void AddKeyword(string word, Lexems lexem)
		{
			_keywords.Add(new Keyword(word, lexem));
		}

		private static Lexems GetKeyword(string word)
		{
			foreach (Keyword keyword in _keywords)
				if (keyword.word == word)
					return keyword.lexem;

			return Lexems.Name;
		}

		public static void ParseNextLexem()
		{
			if (!Reader.IsInitialized)
			{
				throw new Exception($"Ошибка! Объект {nameof(Reader)} не был инициализирован!");
			}

			if (Reader.EOF)
			{
				_lexem = Lexems.EOF;
				return;
			}

			while (Reader.Character == ' ')
			{
				Reader.ReadNextCharacter();
			}

			if (char.IsLetter(Reader.Character))
			{
				ParseName();
			}
			else if (char.IsDigit(Reader.Character))
			{
				ParseNumber();
			}
			else if (Reader.Character == '\n')
			{
				Reader.ReadNextCharacter();
				_lexem = Lexems.Delimiter;
			}
			else if (Reader.Character == ',')
			{
				Reader.ReadNextCharacter();
				_lexem = Lexems.Comma;
			}
			else if (Reader.Character == '(')
			{
				Reader.ReadNextCharacter();
				_lexem = Lexems.LeftBracket;
			}
			else if (Reader.Character == ')')
			{
				Reader.ReadNextCharacter();
				_lexem = Lexems.RightBracket;
			}
			else if (Reader.Character == '=')
			{
				Reader.ReadNextCharacter();
				if (Reader.Character == '=')
				{
					Reader.ReadNextCharacter();
					_lexem = Lexems.Equal;
				}
				else
				{
					_lexem = Lexems.Assign;
				}
			}
			else if (Reader.Character == '!')
			{
				Reader.ReadNextCharacter();
				if (Reader.Character == '=')
				{
					Reader.ReadNextCharacter();
					_lexem = Lexems.NotEqual;
				}
				else
				{
					throw new Exception("Ошибка! Недопустимый символ!");
				}
			}
			else if (Reader.Character == '<')
			{
				Reader.ReadNextCharacter();
				if (Reader.Character == '=')
				{
					Reader.ReadNextCharacter();
					_lexem = Lexems.LessOrEqual;
				}
				else
				{
					_lexem = Lexems.Less;
				}
			}
			else if (Reader.Character == '>')
			{
				Reader.ReadNextCharacter();
				if (Reader.Character == '=')
				{
					Reader.ReadNextCharacter();
					_lexem = Lexems.GreaterOrEqual;
				}
				else
				{
					_lexem = Lexems.Greater;
				}
			}
			else if (Reader.Character == '+')
			{
				Reader.ReadNextCharacter();
				_lexem = Lexems.Plus;
			}
			else if (Reader.Character == '-')
			{
				Reader.ReadNextCharacter();
				_lexem = Lexems.Minus;
			}
			else if (Reader.Character == '*')
			{
				Reader.ReadNextCharacter();
				_lexem = Lexems.Multiplication;
			}
			else if (Reader.Character == '/')
			{
				Reader.ReadNextCharacter();
				_lexem = Lexems.Division;
			}
			else
			{
				throw new Exception("Ошибка! Недопустимый символ!");
			}
		}

		private static void ParseName()
		{
			string name = string.Empty;

			while (char.IsLetter(Reader.Character))
			{
				name += Reader.Character;
				Reader.ReadNextCharacter();
			}

			_name = name;
			_lexem = GetKeyword(name);
		}

		private static void ParseNumber()
		{
			string number = string.Empty;

			while (char.IsDigit(Reader.Character))
			{
				number += Reader.Character;
				Reader.ReadNextCharacter();
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
