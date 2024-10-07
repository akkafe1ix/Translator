using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryTranslator
{
	/// <summary>
	/// Класс для хранения ошибок
	/// </summary>
    public static class Errors
    {

		private static List<string> errors; //Список для хранения ошибок

		/// <summary>
		/// Инициализация класса
		/// </summary>
		static Errors()
		{
			errors = new List<string>();
		}

		/// <summary>
		/// Доабвление ошибки 
		/// </summary>
		/// <param name="error">Ошибка строковое описание</param>
		public static void AddError(string error)
		{
			errors.Add(error);
		}

		/// <summary>
		/// Вывод списка ошибок
		/// </summary>
		public static List<string> GetErrors
		{
			get{return errors;}
		}
	}
}
