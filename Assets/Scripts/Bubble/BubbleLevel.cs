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

    public LevelReference level;

    private Vector2 offset;

    private Converter converter;

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

        level = new LevelReference();
        level.reference = new List<List<Bubble>>();
        level.coreRow = new CoreRow();
        converter = new Converter();

        offset = Vector2.zero;
    }

    void Start()
    {
        //Converter.LevelConvertedFromTxt += Make;
        ShotHandler.ShotHandled += OnLevelChanged;

        string LEVEL_FILE = $"{Application.dataPath}/Levels/1_level.txt";
        ConvertedLevel convertedLevel = converter.ConvertLevelFile(LEVEL_FILE);
        StartCoroutine(Make(convertedLevel));
    }

    private IEnumerator Make(ConvertedLevel lvl)
    {
        transform.position = lvl.startSpawnPosition;
        SpawnBubblesByLevel(lvl);
        level.SetCoreRow();

        // fix the bug when overlap collider don't return anything
        yield return new WaitForSeconds(0.1f);

        InitBubblesNeighbours();
    }

    private void InitBubblesNeighbours()
    {
        foreach (List<Bubble> row in level.reference)
        {
            foreach (Bubble bubble in row)
            {
                bubble.InitNeighbours();
            }
        }
    }

    private void SpawnBubblesByLevel(ConvertedLevel lvl)
    {
        foreach (char[] row in lvl.charLevelMap)
        {
            List<Bubble> newRow = new List<Bubble>();
            bool isRowOffsetChanging = true;

            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == ' ' && isRowOffsetChanging)
                    offset.x += 0.25f;
                else if (row[i] == ' ')
                    offset.x += 0.5f;
                else if (lvl.bubbletypeByCharcode[row[i]] == BubbleColor.VOID)
                    continue;
                else
                {
                    isRowOffsetChanging = false;
                    BubbleColor bubbleColor;
                    if (lvl.bubbletypeByCharcode.TryGetValue(row[i], out bubbleColor))
                    {
                        if (bubbleColor == BubbleColor.RANDOM)
                            bubbleColor = Bubble.GetRandomColor();

                        Bubble spawnedBubble = SpawnBubble(bubbleColor, offset);
                        newRow.Add(spawnedBubble);
                    }
                }
            }

            level.AddRow(newRow);
            offset.y -= 0.5f;
            offset.x = 0f;
        }
    }

    public Bubble SpawnBubble(BubbleColor bubbleColor, Vector2 pos)
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
        level.Replace(destroyedBubble, newBubble);
        newBubble.neighbours = destroyedBubble.neighbours;
        newBubble.DeclareToNeighbours();
        newBubble.SetSpringJointPositionByBubble(destroyedBubble);
        RemoveBubble(destroyedBubble);
        newBubble.transform.SetParent(transform);
    }

    public void AddBubble(Collision2D collision, Bubble newBubble)
    {
        newBubble.SetSpringJointPositionByCollision(collision);
        newBubble.InitNeighbours();
        newBubble.DeclareToNeighbours();
    }

    public void RemoveBubble(Bubble bubbleToDestroy)
    {
        level.Remove(bubbleToDestroy);
        bubbleToDestroy.DeclareRemoveToNeighbours();
        bubbleToDestroy.gameObject.SetActive(false);
    }

    private void OnLevelChanged()
    {
        // Mark not core bubbles
        level.coreRow.MarkBubblesThatHaveWayToCore();

        // Destroy all not core bubbles
        List<Bubble> notAttachedToCoreBubbles = level.GetNotAttachedToCoreBubbles();
        StartCoroutine(RemoveNotCoreBubbles(notAttachedToCoreBubbles));

        // check if core row >= 30%;
        bool isCoreRowDestroyed = level.coreRow.IsCoreRowDestroyed();
        //if (isCoreRowDestroyed)
        //GameManager.Instance.GameOver.Invoke();

        ResetBFSFields();

        // spawn new shooting bubble
        Updated.Invoke();
    }

    private void ResetBFSFields()
    {
        foreach (List<Bubble> row in level.reference)
        {
            foreach (Bubble bubble in row)
            {
                bubble.ResetBFSFields();
            }
        }
    }

    private IEnumerator RemoveNotCoreBubbles(List<Bubble> notAttachedToCoreBubbles)
    {
        foreach (Bubble bubble in notAttachedToCoreBubbles)
        {
            RemoveBubble(bubble);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
