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
            Reader.Initialize(codeFile);
            while (Reader.CurrentChar != -1)
            {
                richTextBox2.Text = richTextBox2.Text + Reader.LineNumber + " ";
                richTextBox2.Text = richTextBox2.Text + Reader.CharPositionInLine + " ";
                richTextBox2.Text = richTextBox2.Text + Reader.CurrentChar + " ";
                richTextBox2.Text = richTextBox2.Text + '"' + Convert.ToChar(Reader.CurrentChar) + '"' + " ";
                richTextBox2.Text = richTextBox2.Text + '\n';
                Reader.ReadNextChar();
            }
            Reader.Close();
        }
    }
}
