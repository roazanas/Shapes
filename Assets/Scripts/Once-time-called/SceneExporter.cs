using UnityEngine;
using System;
using System.IO;
using System.Reflection;

public class SceneHierarchyInspector : MonoBehaviour
{
    // Имя файла, в который будем сохранять информацию
    private string outputPath = "SceneHierarchy.txt";

    void Start()
    {
        // Очищаем файл перед записью
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Получаем все объекты в активной сцене
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        using (StreamWriter writer = new StreamWriter(outputPath, true))
        {
            foreach (GameObject obj in allObjects)
            {
                writer.WriteLine($"GameObject: {obj.name}");
                PrintHierarchy(obj.transform, 0, writer);
            }
        }

        Debug.Log($"Scene hierarchy and components saved to {outputPath}");
    }

    // Рекурсивно выводим иерархию объектов сцены
    void PrintHierarchy(Transform parent, int level, StreamWriter writer)
    {
        string indent = new string('-', level * 2);
        writer.WriteLine($"{indent}Object: {parent.gameObject.name}");

        // Получаем и выводим все компоненты объекта
        Component[] components = parent.GetComponents<Component>();
        foreach (Component component in components)
        {
            writer.WriteLine($"{indent}-- Component: {component.GetType().Name}");
            PrintComponentProperties(component, level + 1, writer);
        }

        // Рекурсивно обрабатываем детей объекта
        foreach (Transform child in parent)
        {
            PrintHierarchy(child, level + 1, writer);
        }
    }

    // Получаем и выводим все свойства компонента
    void PrintComponentProperties(Component component, int level, StreamWriter writer)
    {
        string indent = new string('-', level * 2);

        // Используем рефлексию для получения всех свойств
        PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            try
            {
                // Проверка на возможность чтения свойства
                if (property.CanRead)
                {
                    object value = property.GetValue(component, null);
                    writer.WriteLine($"{indent}---- {property.Name}: {value}");
                }
                else
                {
                    writer.WriteLine($"{indent}---- {property.Name}: (недоступно для чтения)");
                }
            }
            catch (TargetInvocationException e)
            {
                writer.WriteLine($"{indent}---- {property.Name}: (ошибка доступа) {e.InnerException.Message}");
            }
            catch (Exception e)
            {
                writer.WriteLine($"{indent}---- {property.Name}: (не удалось получить значение) {e.Message}");
            }
        }
    }
}
