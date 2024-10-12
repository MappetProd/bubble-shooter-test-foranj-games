using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public int maxTurns = 10;
    public int scoreToWin = 3200;

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
        Updated = null;
        if (_instance == null)
        {
            _instance = this;
        }
        else
            Destroy(this);

        level = new LevelReference();
        level.allBubbles = new List<Bubble>();
        level.coreRow = new CoreRow();
        level.coreRow.CoreBubbles = new List<Bubble>();

        converter = new Converter();
        offset = Vector2.zero;
    }

    void Start()
    {
        ShotHandler.ShotHandled += OnLevelChanged;
        string LEVEL_FILE = "1_level";
        ConvertedLevel convertedLevel = converter.ConvertLevelFile(LEVEL_FILE);
        StartCoroutine(Make(convertedLevel));
    }

    private IEnumerator Make(ConvertedLevel lvl)
    {
        transform.position = lvl.startSpawnPosition;
        SpawnBubblesByLevel(lvl);
        level.coreRow.startAmount = level.coreRow.CoreBubbles.Count;

        // fix the bug when overlap collider don't return anything
        yield return new WaitForSeconds(0.1f);

        InitBubblesNeighbours();
    }

    private void InitBubblesNeighbours()
    {
        foreach (Bubble bubble in level.allBubbles)
        {
            bubble.InitNeighbours();
        }
    }

    private void SpawnBubblesByLevel(ConvertedLevel lvl)
    {
        foreach (char[] row in lvl.charLevelMap)
        {
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
                        level.allBubbles.Add(spawnedBubble);
                        if (lvl.charLevelMap.First() == row)
                            level.coreRow.CoreBubbles.Add(spawnedBubble);
                    }
                }
            }

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
        /*Debug.Log($"Replace bubble check, destroyed: {destroyedBubble}");
        Debug.Log($"Replace bubble check, NEW: {newBubble}");

        Debug.Log($"Replace bubble check, _destroyed NEIGHBOURS: {destroyedBubble.neighbours}");
        Debug.Log($"Replace bubble check, _NEW NEIGHBOURS: {newBubble.neighbours}");*/

        /*destroyedBubble.DeclareRemoveToNeighbours();
        newBubble.neighbours = destroyedBubble.neighbours;
        newBubble.DeclareToNeighbours();
        newBubble.SetSpringJointPositionByBubble(destroyedBubble);*/

        /*level.Add(newBubble);
        if (level.IsBubbleCore(destroyedBubble))
            level.AddCore(newBubble);*/

        AddBubble(destroyedBubble, newBubble);
        RemoveBubble(destroyedBubble);
    }

    public void AddBubble(Collision2D collision, Bubble newBubble)
    {
        newBubble.gameObject.tag = "Bubble";
        level.Add(newBubble);
        newBubble.SetSpringJointPositionByCollision(collision);
        newBubble.InitNeighbours();
        newBubble.DeclareToNeighbours();
    }

    public void AddBubble(Bubble destroyedBubble, Bubble newBubble)
    {
        newBubble.gameObject.tag = "Bubble";
        level.Add(newBubble);
        if (level.IsBubbleCore(destroyedBubble))
            level.AddCore(newBubble);

        newBubble.SetSpringJointPositionByBubble(destroyedBubble);
        newBubble.neighbours = destroyedBubble.neighbours;
        newBubble.DeclareToNeighbours();
        newBubble.transform.SetParent(transform);
    }

    public void RemoveBubble(Bubble bubbleToDestroy)
    {
        level.Remove(bubbleToDestroy);
        bubbleToDestroy.DeclareRemoveToNeighbours();
        Bubble.BubbleDestroyed.Invoke(bubbleToDestroy.points);
        bubbleToDestroy.gameObject.SetActive(false);
    }

    private void OnLevelChanged()
    {
        // Mark not core bubbles
        level.coreRow.MarkBubblesThatHaveWayToCore();

        // Destroy all not core bubbles
        List<Bubble> notAttachedToCoreBubbles = level.GetNotAttachedToCoreBubbles();

        foreach (Bubble b in notAttachedToCoreBubbles)
        {
            Debug.Log($"NOT ATTACHED TO CORE BUBBLE: {b}");
        }

        StartCoroutine(RemoveNotCoreBubbles(notAttachedToCoreBubbles));

        // check if core row >= 30%;
        bool isCoreRowDestroyed = level.coreRow.IsCoreRowDestroyed();
        Debug.Log($"Core row is destroyed?: {isCoreRowDestroyed}");
        Debug.Log($"Core row is destroyed?: {level.coreRow.CoreBubbles.Count}");
        if (isCoreRowDestroyed)
            Player.GameOver.Invoke();

        ResetBFSFields();

        // spawn new shooting bubble
        Updated.Invoke();
    }

    private void ResetBFSFields()
    {
        foreach (Bubble bubble in level.allBubbles)
        {
            bubble.ResetBFSFields();
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

    private void OnDestroy()
    {
        ShotHandler.ShotHandled -= OnLevelChanged;
    }
}
