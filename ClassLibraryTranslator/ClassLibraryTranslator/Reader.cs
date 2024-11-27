using System;
using System.IO;

namespace ClassLibraryTranslator
{
    /// <summary>
    /// Класс посимвольного чтения из файла
    /// </summary>
    public class Reader
    {
        private int _lineNumber;
        private int _positionInLine;
        private char _character;
        private bool _eof;
        private bool _isInitialized;
        private bool _isClosed;
        private StreamReader _streamReader;

        public int LineNumber => _lineNumber;
        public int PositionInLine => _positionInLine;
        public char Character => _character;
        public bool EOF => _eof;
        public bool IsInitialized => _isInitialized;

        public Reader()
        {
            _lineNumber = 0;
            _positionInLine = -1;
            _character = '\0';
            _eof = true;
            _isInitialized = false;
            _isClosed = true;
            _streamReader = StreamReader.Null;
        }

        public void Initialize(string path)
        {
            if (File.Exists(path))
            {
                if (_isInitialized)
                    _streamReader.Close();

                _lineNumber = 1;
                _positionInLine = -1;
                _eof = false;
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

        public void ReadNextCharacter()
        {
            if (!_isInitialized)
            {
                throw new Exception($"Ошибка! Объект {nameof(Reader)} не был инициализирован!");
            }
            if (_isClosed)
            {
                throw new Exception("Ошибка! Поток чтения уже был закрыт!");
            }
            if (_eof)
            {
                throw new Exception("Ошибка! Достигнут конец файла!");
            }

            int currentCharacterInt = _streamReader.Read();
            if (currentCharacterInt == -1)
            {
                _eof = true;
                return;
            }

            _character = (char)currentCharacterInt;
            if (_character == '\n')
            {
                _lineNumber++;
                _positionInLine = 0;
            }
            else if (_character == '\r' || _character == '\t')
            {
                ReadNextCharacter();
            }
            else
            {
                _positionInLine++;
            }
        }

        public void Close()
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
