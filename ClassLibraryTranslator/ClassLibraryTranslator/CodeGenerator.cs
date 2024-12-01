using System.Collections.Generic;

namespace ClassLibraryTranslator
{
    public class CodeGenerator
    {
        public List<string> Code { get; private set; }
        private int _countLabels;
        private NameTable _nameTable;

        /// <summary>
        /// Конструктор для инициализации генератора кода.
        /// </summary>
        public CodeGenerator(NameTable nameTable)
        {
            Code = new List<string>();
            _countLabels = 0;
            _nameTable = nameTable;
        }

        /// <summary>
        /// Увеличение количества меток.
        /// </summary>
        public void AddLabel()
        {
            _countLabels++;
        }

        /// <summary>
        /// Получить текущую метку.
        /// </summary>
        /// <returns>Текущая метка.</returns>
        public string ReturnCurrentLabel()
        {
            return "label" + _countLabels;
        }

        /// <summary>
        /// Добавление инструкции.
        /// </summary>
        /// <param name="instruction">Инструкция.</param>
        public void AddInstruction(string instruction)
        {
            Code.Add(instruction);
        }

        /// <summary>
        /// Декларирование сегмента данных.
        /// </summary>
        public void DeclareDataSegment()
        {
            AddInstruction("data segment");
        }

        /// <summary>
        /// Декларирование сегментов стека и кода.
        /// </summary>
        public void DeclareStackAndCodeSegments()
        {
            AddInstruction("PRINT_BUF DB ' ' DUP(10)");
            AddInstruction("BUFEND    DB '$'");
            AddInstruction("data ends");
            AddInstruction("stk segment stack");
            AddInstruction("db 256 dup (\"?\")");
            AddInstruction("stk ends");
            AddInstruction("code segment");
            AddInstruction("assume cs:code,ds:data,ss:stk");
            AddInstruction("main proc");
            AddInstruction("mov ax,data");
            AddInstruction("mov ds,ax");
        }

        /// <summary>
        /// Объявление завершения основной процедуры.
        /// </summary>
        public void DeclareMainProcedureCompletion()
        {
            AddInstruction("mov ax,4c00h");
            AddInstruction("int 21h");
            AddInstruction("main endp");
        }

        /// <summary>
        /// Объявление завершения кода.
        /// </summary>
        public void DeclareCodeCompletion()
        {
            AddInstruction("code ends");
            AddInstruction("end main");
        }

        /// <summary>
        /// Декларирование переменных.
        /// </summary>
        public void DeclareVariables()
        {
            foreach (Identifier identifier in _nameTable.GetListIdentifiers())
            {
                if (identifier.type == tType.Bool)
                {
                    AddInstruction(identifier.name + "  db    0");
                }
                else
                {
                    AddInstruction(identifier.name + "  dw    0");
                }
            }
        }

        /// <summary>
        /// Декларирование процедуры вывода на печать.
        /// </summary>
        public void DeclarePrint()
        {
            AddInstruction("PRINT PROC NEAR");
            AddInstruction("MOV   CX, 10");
            AddInstruction("MOV   DI, BUFEND - PRINT_BUF");
            AddInstruction("PRINT_LOOP:");
            AddInstruction("MOV   DX, 0");
            AddInstruction("DIV   CX");
            AddInstruction("ADD   DL, '0'");
            AddInstruction("MOV   [PRINT_BUF + DI - 1], DL");
            AddInstruction("DEC   DI");
            AddInstruction("CMP   AL, 0");
            AddInstruction("JNE   PRINT_LOOP");
            AddInstruction("LEA   DX, PRINT_BUF");
            AddInstruction("ADD   DX, DI");
            AddInstruction("MOV   AH, 09H");
            AddInstruction("INT   21H");
            AddInstruction("RET");
            AddInstruction("PRINT ENDP");
        }
    }
}
