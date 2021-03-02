using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab_2._2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Designations:\n! - not,\n+ - ∨(disjunction),\n* - ∧(conjuction),\n@ - →(Implication),\n= - ⇿(Equivalence),\n^ - XOR,\n0- FALSE, 1-TRUE");
            Console.WriteLine("\nInput variables: \n");
            string result = Console.ReadLine();
            Console.WriteLine("\nInput your formula: \n");
            var expr = Convert.ToString(Console.ReadLine());
            //получаем обратную польскую запись от пользователя
            var rpn = ConvertToPND(expr).ToArray();
            Console.WriteLine("\n" + string.Join(" ", rpn) + "\n");
            //выводим формулу в формате RPN
            var varValues = new Dictionary<char, int>();
            var headerShown = false;
            //строим таблицу
            foreach (var combination in GetAllCombinations(result.ToArray(), 0, varValues))
            {
                //вычисляем значение выражения
                var res = Calculate(rpn, varValues);
                //отображем шапку таблицы
                if (!headerShown)
                {
                    foreach (var var in varValues.Keys)
                    {
                        Console.Write(var + "\t");
                    }
                    Console.WriteLine(expr);
                    headerShown = true;
                }
                //отображем строки таблицы
                foreach (var var in varValues.Values)
                {
                    Console.Write(var + "\t");
                }
                Console.WriteLine(res);
            }
            Console.ReadKey();
        }
        //вычисление выражения на основании обратной польской записи и значений переменных
        static int Calculate(IEnumerable<char> rpn, Dictionary<char, int> variableValue)
        {
            var stack = new Stack<bool>();
            foreach (var token in rpn)
            {
                switch (token)
                {
                    case '!':
                        stack.Push(!stack.Pop());
                        break;
                    case '+':
                        stack.Push(stack.Pop() | stack.Pop());
                        break;
                    case '*':
                        stack.Push(stack.Pop() & stack.Pop());
                        break;
                    case '@':
                        stack.Push(!stack.Pop() | stack.Pop());
                        break;
                    case '=':
                        stack.Push(stack.Pop() == stack.Pop());
                        break;
                    case '^':
                        stack.Push(stack.Pop() ^ stack.Pop());
                        break;
                    case '0':
                        stack.Push(false);
                        break;
                    case '1':
                        stack.Push(true);
                        break;
                    default:
                        stack.Push(variableValue[token] == 1);
                        break;
                }
            }
            return stack.Pop() ? 1 : 0;
        }
        //перебор всевозможных комбинаций переменных
        static IEnumerable GetAllCombinations(IList<char> variables, int index, Dictionary<char, int> varValues)
        {
            //Проверка на существующие элементы, присваиваем в список и словарь значения переменных
            if (index >= variables.Count)
            {
                yield return null;
            }
            else
            {
                foreach (var val in Enumerable.Range(0, 2))
                {
                    varValues[variables[index]] = val;
                    foreach (var temp in GetAllCombinations(variables, index + 1, varValues))
                    {
                        yield return temp;//метод возвращает последовательность IEnumerable, элементами которой являются результаты выражений каждого из yield return
                    }
                }
            }
        }
        static String ConvertToPND(String input)
        {
            Stack<char> stack = new Stack<char>();
            String str = input.Replace(" ", string.Empty);
            StringBuilder formula = new StringBuilder();
            //проходимся по формуле
            for (int i = 0; i < str.Length; i++)
            {
                //заносим то, что в скобочках в строку 
                char x = str[i];
                if (x == '(')
                    stack.Push(x);
                else if (x == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                        formula.Append(stack.Pop());//добавляет к скроке подстроку
                    stack.Pop();
                }
                //все это время проходимся по строке и заносим отдельные элементы в stack и с каждым разом извлекаем и заносим в строку 
                //и разделяем строку на множества и логические операторы
                else if (IsOperandus(x))
                {
                    formula.Append(x);
                }
                else if (IsOperator(x))
                {
                    while (stack.Count > 0 && stack.Peek() != '(' && Prior(x) <= Prior(stack.Peek()))//копирует первый элемент
                        formula.Append(stack.Pop());
                    stack.Push(x);
                }
                else
                {
                    char y = stack.Pop();
                    if (y != '(')
                        formula.Append(y);
                }
            }
            while (stack.Count > 0)
            {
                formula.Append(stack.Pop());
            }
            return formula.ToString();
        }
        //функции созданы для того, чтобы отдельно искать операторы и отдельно множества
        static bool IsOperator(char c)
        {
            return (c == '@' || c == '+' || c == '*' || c == '=' || c == '^');
        }
        static bool IsOperandus(char c)
        {
            return (c >= 'a' && c <= 'z' || c == '.');
        }
        static int Prior(char c)
        {
            switch (c)
            {
                case '!':
                    return 1;
                case '*':
                    return 2;
                case '+':
                    return 3;
                case '@':
                    return 4;
                case '^':
                    return 5;
                case '=':
                    return 6;
                default:
                    throw new ArgumentException("Incorect input");
            }
        }
    }
}
