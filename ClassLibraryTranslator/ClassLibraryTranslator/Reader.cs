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
        static private int lineNumber;  //Номер строки
        static private int charPositionInLine; //Текущая позиция на строке
        static private int currentChar; //Текущий символ

        static StreamReader streamReader;

        /// <summary>
        /// Константы для сравнения
        /// </summary>
        const int endOfFile = -1;
        const int newLine = '\n';
        const int carriageReturn = '\r';
        const int tab = '\t';

        /// <summary>
        /// Метод чтения следующего символа
        /// </summary>
        static public void ReadNextChar()
        {
            currentChar = streamReader.Read();
            if (currentChar == endOfFile)
                currentChar = endOfFile;
            else if (currentChar == newLine)
            {
                lineNumber++;
                charPositionInLine = 0;
            }
            else if (currentChar == carriageReturn || currentChar == tab)
                ReadNextChar();
            else
                charPositionInLine++;
        }

        /// <summary>
        /// Метод инициализации чтения файла
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        static public void Initialize(string filePath)
        {
            if (File.Exists(filePath))
            {
                streamReader = new StreamReader(filePath);
                lineNumber = 1;
                charPositionInLine = 0;
                ReadNextChar();
            }
        }

        /// <summary>
        /// Метод закрытия посимвольного чтения файла
        /// </summary>
        static public void Close()
        {
            streamReader.Close();
        }

        //Получение номера строки
        static public int LineNumber
        {
            get { return lineNumber; }
        }
        //Получение позиции на строке
        static public int CharPositionInLine
        {
            get { return charPositionInLine; }
        }
        //Получение текущего символа 
        static public int CurrentChar
        {
            get { return currentChar; }
        }
    }
}
