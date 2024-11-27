using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClassLibraryTranslator
{
    /// <summary>
    /// Класс посимвольного чтения из файла
    /// </summary>
    public static class Reader
    {
		private static int _lineNumber;
		private static int _PositionInLine;
		private static char _Character;
		private static bool _EOF;
		private static bool _isInitialized;
		private static bool _isClosed;
		private static StreamReader _streamReader;

		public static int LineNumber => _lineNumber;
		public static int PositionInLine => _PositionInLine;
		public static char Character => _Character;
		public static bool EOF => _EOF;
		public static bool IsInitialized => _isInitialized;

		static Reader()
		{
			_lineNumber = 0;
			_PositionInLine = -1;
			_Character = '\0';
			_EOF = true;
			_isInitialized = false;
			_isClosed = true;
			_streamReader = StreamReader.Null;
		}

		public static void Initialize(string path)
		{
			if (File.Exists(path))
			{
				if (_isInitialized)
					_streamReader.Close();

				_lineNumber = 1;
				_PositionInLine = 0;
				_EOF = false;
				_isInitialized = true;
				_streamReader = new StreamReader(path);
				_isClosed = false;

				ReadNextCharacter();
			}
			else
			{
				throw new Exception("Ошибка! Указан некорректный путь к файлу!");
			}
		}

		public static void ReadNextCharacter()
		{
			if (!_isInitialized)
			{
				throw new Exception($"Ошибка! Объект {nameof(Reader)} не был инициализирован!");
			}
			if (_isClosed)
			{
				throw new Exception("Ошибка! Поток чтения уже был закрыт!");
			}
			if (_EOF)
			{
				throw new Exception("Ошибка! Достигнут конец файла!");
			}

			int currentCharacterInt = _streamReader.Read();
			if (currentCharacterInt == -1)
			{
				_EOF = true;
				return;
			}

			_Character = (char)currentCharacterInt;
			if (_Character == '\n')
			{
				_lineNumber++;
				_PositionInLine = 0;
			}
			else if (_Character == '\r' || _Character == '\t')
			{
				ReadNextCharacter();
			}
			else
			{
				_PositionInLine++;
			}
		}

		public static void Close()
		{
			if (!_isInitialized)
			{
				throw new Exception($"Ошибка! Объект {nameof(Reader)} не был инициализирован!");
			}

			_streamReader.Close();
			_isClosed = true;
		}
	}
}
