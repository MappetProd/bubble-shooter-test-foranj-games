using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleLevel : MonoBehaviour
{
    private static BubbleLevel _instance;
    public static BubbleLevel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("Level").AddComponent<BubbleLevel>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private GameObject bubblePrefab;

    private const int DEFAULT_ROWS = 10;
    private const int DEFAULT_COLS = 12;

    private List<List<Bubble>> field;

    private CoreRow coreRow;

    private Vector2 offset;

    public static Action Updated;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        field = new List<List<Bubble>>();
        offset = Vector2.zero;
    }

    void Start()
    {
        Converter.LevelConvertedFromTxt += Make;
        ShotHandler.ShotHandled += OnLevelChanged;
    }

    private IEnumerator Make()
    {
        transform.position = Converter.Instance.StartSpawnPosition;
        SpawnBubblesByLevel(Converter.Instance.Level);
        coreRow = new CoreRow();
        coreRow.CoreBubbles = field[0];
        coreRow.startAmount = field[0].Count;

        // fix the bug when overlap collider don't return anything
        yield return new WaitForSeconds(0.1f);

        InitBubblesNeighbours();
    }


    private void InitBubblesNeighbours()
    {
        foreach (List<Bubble> row in field)
        {
            foreach (Bubble bubble in row)
            {
                bubble.InitNeighbours();
            }
        }
    }

    private void SpawnBubblesByLevel(List<char[]> levelmap)
    {
        foreach (char[] row in levelmap)
        {
            List<Bubble> newRow = new List<Bubble>();
            bool isRowOffsetChanging = true;

            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == ' ' && isRowOffsetChanging)
                    offset.x += 0.25f;
                else if (row[i] == ' ')
                    offset.x += 0.5f;
                else if (Converter.Instance.BubbletypeByCharcode[row[i]] == Converter.BubbleColor.VOID)
                    continue;
                else
                {
                    isRowOffsetChanging = false;
                    Converter.BubbleColor bubbleColor;
                    if (Converter.Instance.BubbletypeByCharcode.TryGetValue(row[i], out bubbleColor))
                    {
                        if (bubbleColor == Converter.BubbleColor.RANDOM)
                            bubbleColor = Bubble.GetRandomColor();

                        Bubble spawnedBubble = SpawnBubble(bubbleColor, offset);
                        newRow.Add(spawnedBubble);
                    }
                }
            }
            field.Add(newRow);
            offset.y -= 0.5f;
            offset.x = 0f;
        }
    }

    public Bubble SpawnBubble(Converter.BubbleColor bubbleColor, Vector2 pos)
    {
        GameObject bubbleObject = Instantiate(bubblePrefab, transform);
        Bubble bubble = bubbleObject.GetComponent<Bubble>();
        bubble.InitColor(bubbleColor);
        bubbleObject.transform.localPosition = pos;
        bubbleObject.GetComponent<SpringJoint2D>().connectedAnchor = bubbleObject.transform.position;
        return bubble;
    }

    public void ReplaceBubble(Bubble destroyedBubble, Bubble newBubble)
    {
        foreach (List<Bubble> row in field)
        {
            int destroyedBubbleIndex = row.FindIndex(bubble => bubble.id == destroyedBubble.id);
            if (destroyedBubbleIndex == -1)
                continue;
            else
            {
                row.Remove(destroyedBubble);
                row.Insert(destroyedBubbleIndex, newBubble);
                break;
            }
        }

        newBubble.transform.SetParent(transform);
    }

    public void RemoveBubble(Bubble destroyedBubble)
    {
        foreach (List<Bubble> row in field)
        {
            bool result = row.Remove(destroyedBubble);
            if (result)
                break;
        }
    }
    
    private void OnLevelChanged()
    {
        // Mark not core bubbles
        coreRow.MarkBubblesThatHaveWayToCore();

        // Destroy all not core bubbles
        RemoveNotCoreBubbles();

        // check if core row >= 30%;
        bool isCoreRowDestroyed = coreRow.IsCoreRowDestroyed();
        //if (isCoreRowDestroyed)
        //GameManager.Instance.GameOver.Invoke();

        // ResetFields()
        ResetBFSFields();

        // spawn new shooting bubble
        Updated.Invoke();
    }

    private void ResetBFSFields()
    {
        foreach (List<Bubble> row in field)
        {
            foreach (Bubble bubble in row)
            {
                bubble.ResetBFSFields();
            }
        }
    }

    private IEnumerator RemoveNotCoreBubbles()
    {
        foreach (List<Bubble> row in field)
        {
            foreach (Bubble bubble in row)
            {
                if (!bubble.hasWayToCore)
                {
                    bubble.DeclareRemoveToNeighbours();
                    RemoveBubble(bubble);
                    Destroy(bubble);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
}
