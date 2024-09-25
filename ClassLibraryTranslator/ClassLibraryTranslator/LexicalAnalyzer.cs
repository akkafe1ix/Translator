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
		//Cтруктура ключевого слова
		private struct Keyword
		{
			public string word;
			public Lexems lex;
		}

		private static int keywordPointer; //Счётчи для массива
		private static Keyword[] keywords ; //Массив соответсвий слова лексеме
		private static Lexems currentLexem; //Текущая лексема
		private static string currentName; //Текущий индетификатор
		private static int currentNumber; //Текучее число

		/// <summary>
		/// Метод инициальзации
		/// </summary>
		static LexicalAnalyzer()
        {
			keywordPointer = 0;
			keywords = new Keyword[200];
			currentLexem = Lexems.None;
			currentName = string.Empty;
			currentNumber = 0;

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

		/// <summary>
		/// Добавление ключевого слова
		/// </summary>
		/// <param name="word">Слово</param>
		/// <param name="lex">Лексема</param>
		static public void AddKeyword(string word, Lexems lex)
        {
			Keyword kw = new Keyword();
			kw.word = word;
			kw.lex = lex;
			keywords[keywordPointer++] = kw;
			
        }

		/// <summary>
		/// Полеучение ключевого слова
		/// </summary>
		/// <param name="word">Слово</param>
		/// <returns></returns>
		static public Lexems GetKeyword(string word)
        {
			foreach (Keyword keyword in keywords)
				if (keyword.word == word)
					return keyword.lex;
			return Lexems.Name;
        }


		/// <summary>
		/// Метод для чтения следующей лексемы
		/// </summary>
		public static void ParseNextLexem()
		{

			if (Reader.CurrentChar==-1)
			{
				currentLexem = Lexems.EOF;
				return;
			}

			while (Reader.CurrentChar == ' ')
			{
				Reader.ReadNextChar();
			}

			if (char.IsLetter(Convert.ToChar(Reader.CurrentChar)))
			{
				ParseName();
			}
			else if (char.IsDigit(Convert.ToChar(Reader.CurrentChar)))
			{
				ParseNumber();
			}
			else if (Reader.CurrentChar == '\n')
			{
				Reader.ReadNextChar();
				currentLexem = Lexems.Delimiter;
			}
			else if (Reader.CurrentChar == ',')
			{
				Reader.ReadNextChar();
				currentLexem = Lexems.Comma;
			}
			else if (Reader.CurrentChar == '(')
			{
				Reader.ReadNextChar();
				currentLexem = Lexems.LeftBracket;
			}
			else if (Reader.CurrentChar == ')')
			{
				Reader.ReadNextChar();
				currentLexem = Lexems.RightBracket;
			}
			else if (Reader.CurrentChar == '=')
			{
				Reader.ReadNextChar();
				if (Reader.CurrentChar == '=')
				{
					Reader.ReadNextChar();
					currentLexem = Lexems.Equal;
				}
				else
				{
					currentLexem = Lexems.Assign;
				}
			}
			else if (Reader.CurrentChar == '!')
			{
				Reader.ReadNextChar();
				if (Reader.CurrentChar == '=')
				{
					Reader.ReadNextChar();
					currentLexem = Lexems.NotEqual;
				}
				else
				{
					throw new Exception("Ошибка! Недопустимый символ!");
				}
			}
			else if (Reader.CurrentChar == '<')
			{
				Reader.ReadNextChar();
				if (Reader.CurrentChar == '=')
				{
					Reader.ReadNextChar();
					currentLexem = Lexems.LessOrEqual;
				}
				else
				{
					currentLexem = Lexems.Less;
				}
			}
			else if (Reader.CurrentChar == '>')
			{
				Reader.ReadNextChar();
				if (Reader.CurrentChar == '=')
				{
					Reader.ReadNextChar();
					currentLexem = Lexems.GreaterOrEqual;
				}
				else
				{
					currentLexem = Lexems.Greater;
				}
			}
			else if (Reader.CurrentChar == '+')
			{
				Reader.ReadNextChar();
				currentLexem = Lexems.Plus;
			}
			else if (Reader.CurrentChar == '-')
			{
				Reader.ReadNextChar();
				currentLexem = Lexems.Minus;
			}
			else if (Reader.CurrentChar == '*')
			{
				Reader.ReadNextChar();
				currentLexem = Lexems.Multiplication;
			}
			else if (Reader.CurrentChar == '/')
			{
				Reader.ReadNextChar();
				currentLexem = Lexems.Division;
			}
			else
			{
				throw new Exception("Ошибка! Недопустимый символ!");
			}
		}


		/// <summary>
		/// Разобрать идентификатор
		/// </summary>
		private static void ParseName()
		{
			string name = string.Empty;

			while (char.IsLetter(Convert.ToChar(Reader.CurrentChar)))
			{
				name += Reader.CurrentChar;
				Reader.ReadNextChar();
			}

			currentName = name;
			currentLexem = GetKeyword(name);
		}

		/// <summary>
		/// Разобрать число
		/// </summary>
		private static void ParseNumber()
		{
			string number = string.Empty;

			while (char.IsDigit(Convert.ToChar(Reader.CurrentChar)))
			{
				number += Reader.CurrentChar;
				Reader.ReadNextChar();
			}

			if (!int.TryParse(number, out int result))
			{
				throw new Exception("Ошибка! Переполнение типа int!");
			}

			currentNumber = result;
			currentLexem = Lexems.Number;
		}


	}
}
