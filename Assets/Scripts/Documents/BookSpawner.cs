using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookSpawner : MonoBehaviour
{
    [Header("Префаб и родитель")]
    [Tooltip("Префаб книги (Book)")]
    public GameObject bookPrefab;
    [Tooltip("Родительский объект для книги (например, Canvas)")]
    public Transform parent;
    [Tooltip("Позиция для спауна (если null — по центру родителя)")]
    public Vector2? spawnPosition;

    [Header("Страницы книги")]
    [Tooltip("Список страниц для создаваемой книги")]
    public List<PageData> pages = new List<PageData>();

    public void SpawnBook()
    {
        if (bookPrefab == null)
        {
            Debug.LogError("BookSpawner: Не назначен префаб книги!");
            return;
        }
        GameObject bookObj = Instantiate(bookPrefab, parent);
        RectTransform rect = bookObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = spawnPosition ?? Vector2.zero;
        }
        Book book = bookObj.GetComponent<Book>();
        if (book != null)
        {
            book.pages = new List<PageData>(pages);
            book.ShowPage(0);
        }
        else
        {
            Debug.LogError("BookSpawner: На префабе отсутствует компонент Book!");
        }
    }
} 