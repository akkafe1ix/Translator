using System.Collections.Generic;

namespace ClassLibraryTranslator
{
    /// <summary>
    /// Класс для хранения ошибок
    /// </summary>
    public class Errors
    {
        private List<string> _errors; // Список для хранения ошибок

        /// <summary>
        /// Конструктор для инициализации списка ошибок
        /// </summary>
        public Errors()
        {
            _errors = new List<string>();
        }

        /// <summary>
        /// Добавление ошибки
        /// </summary>
        /// <param name="error">Ошибка строковое описание</param>
        public void AddError(string error)
        {
            _errors.Add(error);
        }

        /// <summary>
        /// Получение списка ошибок
        /// </summary>
        public List<string> GetErrors => _errors;
    }
}
