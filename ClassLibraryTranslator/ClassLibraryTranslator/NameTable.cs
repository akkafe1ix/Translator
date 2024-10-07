using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryTranslator
{
    // Перечисление, определяющее категории идентификаторов.
    public enum tCat
    {
        Const,  // Константа
        Var,    // Переменная
        Type    // Тип
    }

    // Перечисление, определяющее типы идентификаторов.
    public enum tType
    {
        None,   // Тип не определен
        Int,    // Целочисленный тип
        Bool    // Логический тип (Boolean)
    }

    // Структура для хранения информации об идентификаторе.
    public struct Identifier
    {
        public string name;     // Имя идентификатора
        public tCat category;   // Категория идентификатора (Const, Var, Type)
        public tType type;      // Тип идентификатора (None, Int, Bool)

        // Конструктор для инициализации идентификатора.
        public Identifier(string name, tCat category, tType type)
        {
            this.name = name;               // Устанавливаем имя идентификатора
            this.category = category;       // Устанавливаем категорию
            this.type = type;               // Устанавливаем тип
        }
    }

    // Статический класс для управления таблицей имен идентификаторов.
    public static class NameTable
    {
        // Связный список для хранения всех идентификаторов.
        private static LinkedList<Identifier> identifiers = new LinkedList<Identifier>();

        // Инициализация таблицы имен (очистка списка идентификаторов).
        public static void Initialize()
        {
            identifiers.Clear(); // Очищаем список идентификаторов
        }

        // Добавление идентификатора в таблицу с указанием категории, но без типа.
        public static Identifier AddIdentifier(string name, tCat category)
        {
            // Проверка на существование идентификатора с таким же именем.
            if (identifiers.Any(ident => ident.name == name))
            {
                throw new ArgumentException($"Identifier '{name}' уже существует."); // Выбрасываем исключение при дублировании
            }

            // Создаем новый идентификатор с заданным именем и категорией, но типом по умолчанию (None).
            Identifier newIdent = new Identifier(name, category, tType.None);
            identifiers.AddLast(newIdent); // Добавляем идентификатор в конец списка
            return newIdent;               // Возвращаем созданный идентификатор
        }

        // Добавление идентификатора в таблицу с указанием категории и типа.
        public static Identifier AddIdentifier(string name, tCat category, tType type)
        {
            // Проверка на существование идентификатора с таким же именем.
            if (identifiers.Any(ident => ident.name == name))
            {
                throw new ArgumentException($"Identifier '{name}' уже существует."); // Выбрасываем исключение при дублировании
            }

            // Создаем новый идентификатор с заданным именем, категорией и типом.
            Identifier newIdent = new Identifier(name, category, type);
            identifiers.AddLast(newIdent); // Добавляем идентификатор в конец списка
            return newIdent;               // Возвращаем созданный идентификатор
        }

        // Поиск идентификатора по имени.
        public static Identifier FindIdentifier(string name)
        {
            // Начинаем поиск с первого элемента списка.
            LinkedListNode<Identifier> node = identifiers.First;

            // Проходим по списку, пока не найдем идентификатор с указанным именем.
            while (node != null && node.Value.name != name)
            {
                node = node.Next; // Переход к следующему узлу списка
            }

            // Если идентификатор не найден, выбрасываем исключение.
            if (node == null)
            {
                throw new KeyNotFoundException($"Identifier '{name}' не найден."); // Идентификатор с таким именем не найден
            }

            return node.Value; // Возвращаем найденный идентификатор
        }

        // Получение всех идентификаторов в виде связного списка.
        public static LinkedList<Identifier> GetIdentifiers()
        {
            return identifiers; // Возвращаем текущий список идентификаторов
        }

        // Получение всех идентификаторов в виде списка (List).
        public static List<Identifier> GetListIdentifiers()
        {
            return identifiers.ToList(); // Преобразуем связный список в обычный список и возвращаем его
        }
    }
}
