using UnityEngine;
using System;
using System.IO;
using System.Reflection;

public class SceneHierarchyInspector : MonoBehaviour
{
    // ��� �����, � ������� ����� ��������� ����������
    private string outputPath = "SceneHierarchy.txt";

    void Start()
    {
        // ������� ���� ����� �������
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // �������� ��� ������� � �������� �����
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

    // ���������� ������� �������� �������� �����
    void PrintHierarchy(Transform parent, int level, StreamWriter writer)
    {
        string indent = new string('-', level * 2);
        writer.WriteLine($"{indent}Object: {parent.gameObject.name}");

        // �������� � ������� ��� ���������� �������
        Component[] components = parent.GetComponents<Component>();
        foreach (Component component in components)
        {
            writer.WriteLine($"{indent}-- Component: {component.GetType().Name}");
            PrintComponentProperties(component, level + 1, writer);
        }

        // ���������� ������������ ����� �������
        foreach (Transform child in parent)
        {
            PrintHierarchy(child, level + 1, writer);
        }
    }

    // �������� � ������� ��� �������� ����������
    void PrintComponentProperties(Component component, int level, StreamWriter writer)
    {
        string indent = new string('-', level * 2);

        // ���������� ��������� ��� ��������� ���� �������
        PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            try
            {
                // �������� �� ����������� ������ ��������
                if (property.CanRead)
                {
                    object value = property.GetValue(component, null);
                    writer.WriteLine($"{indent}---- {property.Name}: {value}");
                }
                else
                {
                    writer.WriteLine($"{indent}---- {property.Name}: (���������� ��� ������)");
                }
            }
            catch (TargetInvocationException e)
            {
                writer.WriteLine($"{indent}---- {property.Name}: (������ �������) {e.InnerException.Message}");
            }
            catch (Exception e)
            {
                writer.WriteLine($"{indent}---- {property.Name}: (�� ������� �������� ��������) {e.Message}");
            }
        }
    }
}
