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
        private static int countLabels;

        static CodeGenerator()
        {
            code = new List<string>();
            countLabels = 0;
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

    }

}
