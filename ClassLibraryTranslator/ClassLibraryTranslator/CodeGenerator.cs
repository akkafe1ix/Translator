using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryTranslator
{
    public static class CodeGenerator
    {
        static public List<string> code;
        private static int _countLabels;

        static CodeGenerator()
        {
            code = new List<string>();
            _countLabels = 0;
        }

        /// <summary>
        /// Увеличение кол-ва меток
        /// </summary>
        public static void AddLabel()
        {
            _countLabels++;
        }

        /// <summary>
        /// Получить текущую метку
        /// </summary>
        /// <returns>текущая метка</returns>
        public static string ReturnCurrentLabel()
        {
            return "lable" + _countLabels;
        }

        /// <summary>
        /// Добавление инструкции
        /// </summary>
        /// <param name="instruction">инструкция</param>
        public static void AddInstruction(string instruction)
        {
            code.Add(instruction);
        }

        /// <summary>
        /// Декларирование дата сегмента
        /// </summary>
        public static void DeclareDataSegment()
        {
            AddInstruction("data segment");
        }

        /// <summary>
        /// Декларировать  сегменты стека и кода
        /// </summary>
        public static void DeclareStackAndCodeSegments()
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
        /// Объявление замершения основной процедуры
        /// </summary>
        public static void DeclareMainProcedureCompletion()
        {
            AddInstruction("mov ax,4c00h");
            AddInstruction("int 21h");
            AddInstruction("main endp");

        }
        /// <summary>
        /// Объявление завершения кода
        /// </summary>
        public static void DeclareCodeCompletion()
        {
            AddInstruction("code ends");
            AddInstruction("end main");
        }

        /// <summary>
        /// Декларировать переменные
        /// </summary>
        public static void DeclareVariables()
        {
            foreach (Identifier identifier in NameTable.GetListIdentifiers())
                AddInstruction(identifier.name + "  dw    1");
        }

        /// <summary>
        /// Декларировать процедуру вывода на печать
        /// </summary>
        public static void DeclarePrint()
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
