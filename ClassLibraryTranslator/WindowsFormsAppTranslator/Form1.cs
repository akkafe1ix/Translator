using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using ClassLibraryTranslator;

namespace WindowsFormsAppTranslator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Метод открытия и чтения текстового документа
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            string codeFile;
            //Выбор тектового файла
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Clear();
                richTextBox3.Clear();
                //Обычное тение файла
                codeFile = openFileDialog1.FileName.ToString();
                string code = System.IO.File.ReadAllText(codeFile);
                richTextBox1.Text = code;
                richTextBox2.Clear();
                //Посимвольное чтение файла через класс Reader
                ReaderOpen(codeFile);
            }

        }

        //Метод записи в текстовый документ
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            string codeFile;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                codeFile = openFileDialog1.FileName.ToString();
                string code = richTextBox1.Text;
                File.WriteAllText(codeFile, code);
            }

        }

        /// <summary>
        /// Метод вызова посимвольного чтения файла 
        /// </summary>
        /// <param name="codeFile">Путь к файлу</param>
        private void ReaderOpen(string codeFile)
        {
            var reader = new Reader();
            var nameTable = new NameTable();
            var errors = new Errors();
            var lexicalAnalyzer = new LexicalAnalyzer(reader);
            var codeGenerator = new CodeGenerator(nameTable);
            var syntaxAnalyzer = new SyntaxAnalyzer(lexicalAnalyzer, codeGenerator, nameTable, errors, reader);

            // Инициализация
            reader.Initialize(codeFile);

            // Компиляция
            syntaxAnalyzer.Compile();

            // Вывод ошибок
            foreach (string err in errors.GetErrors)
            {
                richTextBox3.Text = richTextBox3.Text +err+ "\n";
            }

            // Вывод сгенерированного кода
            if (errors.GetErrors.Count==0)
            foreach (string str in codeGenerator.Code)
            {
                richTextBox2.Text = richTextBox2.Text + str +"\n";
            }

            reader.Close(); // Закрытие Reader
        }

        private void выполнитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.TextLength != 0)
            {
                richTextBox3.Clear();
                richTextBox2.Clear();
                string fileName = "BufferCode.txt";

                // Путь к корневой папке проекта
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                // Новый текст для записи в файл
                string code = richTextBox1.Text;

                try
                {
                    // Перезапись файла (если файл существует, он будет перезаписан)
                    File.WriteAllText(filePath, code);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Внимание!");
                }
                ReaderOpen(filePath);
            }
            else
            {
                MessageBox.Show("Напишите программный код, а затем запускайте компиляцию!", "Внимание!");
            }
        }

        private void выполнитьКодToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((richTextBox3.TextLength == 0)&(richTextBox2.TextLength != 0))
            {
                string fileName = "C:\\Users\\Администратор\\Desktop\\Translator\\TASM\\Code.asm";
                File.WriteAllText(fileName, richTextBox2.Text);
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\DOSBox-0.74-3\DOSBox.exe", @"C:\Users\Администратор\Desktop\Translator\TASM\RUN.bat -noconsole");
                process.Start();
            }
            else if(richTextBox3.TextLength != 0)
            {
                MessageBox.Show("При компиляции возникли ошибки! Код не может быть запущен!", "Внимание!");
            }
            else if (richTextBox2.TextLength == 0)
            {
                MessageBox.Show("Программа не скомпилирована!", "Внимание!");
            }
        }
    }
}
